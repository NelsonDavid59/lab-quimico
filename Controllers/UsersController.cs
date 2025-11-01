using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AgroLaboratorio.Models;
using Microsoft.AspNetCore.Authorization;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.UsersVMS;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AgroLaboratorio.Data;

namespace AgroLaboratorio.Controllers
{
    public class UsersController : Controller
    {
        /* Clases de Identity para el manejo de logueo, usuario y perfil */
        private SignInManager<Usuario> _signInManager;
        private UserManager<Usuario> _userManager;
        private RoleManager<Perfil> _roleManager; 
        private AuthDbContext _authDbContext;

        public UsersController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, 
                               RoleManager<Perfil> roleManager, AuthDbContext authDbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _authDbContext = authDbContext;
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            //Obtención de la lista de usuarios
            var usuarios = await _userManager.Users
                .Select(u => new UsuarioVM { 
                    CodUsuario = u.CodUsuario, 
                    Username = u.UserName, 
                    CodUserAlta = u.CodUserAlta,
                    FechaAlta = u.FechaAlta,
                    CodUserModif = u.CodUserModif,
                    FechaModif = u.FechaModif
                }).ToListAsync();

            return View(new UserListVM { Usuarios = usuarios});
        }

        //GET: Users/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //POST: Users/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            //Fallo de autenticación retorna la misma vista
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByNameAsync(model.Username);

            if ( user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return View(model);
            }

            //Se trata de iniciar una sesion con el usuario y password recibido
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

            //Éxito
            if (result.Succeeded)
            {
                //Redirección a la acción a la que el usuario intenta ir
                //if(!string.IsNullOrWhiteSpace(model.Return))
                return RedirectToAction("Index", "Home");
            }

            //Error
            ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            return View(model);
        }

        //POST: Users/Logout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //GET: Users/RegisterForm
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RegisterForm()
        {
            //Obtenemos la lista de Perfiles que deberán ser seleccionados
            var listaPerfiles = await _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Descripcion, Value = r.CodPerfil })
                .ToListAsync();

            var model = new RegistrationVM { Perfiles = listaPerfiles };

            //Es retornado el formulario en un PARTIAL VIEW para renderizarlo
            return PartialView("_Create", model);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Register(RegistrationVM model)
        {
            if (!ModelState.IsValid)
            {
                //Recargamos la lista de perfiles
                await LoadSelectItemsPerfiles(model);

                return PartialView("_Create", model);
            }

            var nuevoUsuario = new Usuario
            {
                UserName = model.Username,
                CodUsuario = model.Username,
                Nombre = model.Username,
                CodUserAlta = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            //Se procede a guardar el recurso
            var result = await _userManager.CreateAsync(nuevoUsuario, model.Password);

            if (result.Succeeded) 
            {
                //Asignamos el rol
                var perfil = await _roleManager.FindByIdAsync(model.CodPerfil);

                if (perfil != null) 
                {
                    await _userManager.AddToRoleAsync(nuevoUsuario, perfil.Name);
                }

                //Flag de éxito
                return Json(new { success = true });
            }
            else
            {
                //Recargamos la lista de perfiles
                await LoadSelectItemsPerfiles(model);

                //Errores
                ModelState.AddModelError("", ErrorMessages.Save);

                return PartialView("_Create", model);
            }
                
        }

        //GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _userManager.FindByIdAsync(id);

            if (result != null)
            {
                //Obtenemos la lista de Perfiles que deberán ser seleccionados
                var listaPerfiles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Descripcion, Value = r.CodPerfil })
                    .ToListAsync();

                var model = new RegistrationVM
                {
                    CodUsuario = result.CodUsuario,
                    Username = result.UserName,
                    Perfiles = listaPerfiles
                };

                return PartialView("_Edit", model);
            }

            return NotFound();
        }

        //POST: Users/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(string id, RegistrationVM model) 
        {
            using var transaction = _authDbContext.Database.BeginTransaction();

            if (!ModelState.IsValid)
            {
                await transaction.RollbackAsync();

                //Recarga de listado de perfiles
                await LoadSelectItemsPerfiles(model);

                return PartialView("_Edit", model);
            }

            //Buscamos el recurso
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null) return NotFound();

            //Modificamos los datos
            usuario.UserName = model.Username;
            usuario.CodUserModif = User.FindFirstValue(ClaimTypes.NameIdentifier); //Usuario actual
            usuario.FechaModif = DateTime.Now; //Fecha de modificación

            var updResult = await _userManager.UpdateAsync(usuario);

            if(!updResult.Succeeded)
            {
                await transaction.RollbackAsync();

                //Recargamos la lista de perfiles
                await LoadSelectItemsPerfiles(model);

                //Errores
                ModelState.AddModelError("", ErrorMessages.Update);

                return PartialView("_Edit", model);
            }

            //Asignamos el rol
            var perfil = await _roleManager.FindByIdAsync(model.CodPerfil);

            if (perfil != null)
            {
                // Obtener los roles actuales del usuario
                var currentRoles = await _userManager.GetRolesAsync(usuario);

                // Eliminar todos los roles actuales
                await _userManager.RemoveFromRolesAsync(usuario, currentRoles);

                var rolAsignResult = await _userManager.AddToRoleAsync(usuario, perfil.Name);
                if (!rolAsignResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError("", "No ha podido asignarse el Perfil al usuario.");
                    return PartialView("_Edit", model);
                }
            }

            //Modificamos la contraseña en caso de existir.
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                //Con Identity es necesario generar un token para resetear las contraseñas
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(usuario);

                var resetResult = await _userManager.ResetPasswordAsync(usuario, resetToken, model.Password);

                if (!resetResult.Succeeded) 
                {
                    await transaction.RollbackAsync();

                    foreach (var err in resetResult.Errors) ModelState.AddModelError("", err.Description);
                    await LoadSelectItemsPerfiles(model);
                    return PartialView("_Edit", model);
                }
            }

            await transaction.CommitAsync();

            //Éxito
            return Json(new { success = true }); 
        }

        //GET: SHOW
        [HttpGet]
        public async Task<IActionResult> Show(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var result = await _userManager.FindByIdAsync(id);

            if(result != null)
            {
                var usuario = new UsuarioVM
                {
                    CodUsuario = result.CodUsuario,
                    Username = result.UserName,
                    CodUserAlta = result.CodUserAlta,
                    FechaAlta = result.FechaAlta,
                    CodUserModif = result.CodUserModif,
                    FechaModif = result.FechaModif
                };

                return View(usuario);
            }

            return NotFound();
        }

        private async Task LoadSelectItemsPerfiles(RegistrationVM model)
        {
            model.Perfiles = await _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Descripcion, Value = r.CodPerfil })
                .ToListAsync();
        }
    }
}
