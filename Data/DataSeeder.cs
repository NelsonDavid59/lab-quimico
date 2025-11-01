using AgroLaboratorio.Models;
using Microsoft.AspNetCore.Identity;

namespace AgroLaboratorio.Data
{
    /**
     * Esta clase es utilizada para agregar datos que deben existir
     * inicialmente en la base de datos
     */
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider services)
        {
            var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Perfil>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

            //Perfil Admin si es que no existe
            const string roleName = "ADMIN";
            var role = await roleManager.FindByNameAsync(roleName);

            //Si no existe, se crea
            if (role == null) 
            {
                role = new Perfil
                {
                    Name = roleName,
                    Descripcion = roleName,
                    CodPerfil = roleName,
                    NormalizedName = roleName.ToUpperInvariant(),
                    CodUserAlta = "admin"
                };

                var r = await roleManager.CreateAsync(role);
                if (!r.Succeeded) throw new Exception(string.Join(" | ", r.Errors.Select(e => e.Description)));
            }

            //Usuario inicial
            const string adminId = "admin";
            const string adminPassword = "admin123";

            var user = await userManager.FindByIdAsync(adminId);

            if(user == null) 
            {
                user = new Usuario()
                {
                    CodUsuario = adminId,
                    EmailConfirmed = true,
                    Nombre = "ADMINISTRADOR",
                    UserName = adminId,
                    CodUserAlta = "admin"
                };

                var r = await userManager.CreateAsync(user, adminPassword);
                if (!r.Succeeded) throw new Exception(string.Join(" | ", r.Errors.Select(e => e.Description)));
            }

            //Asignar rol al usuario si aún no lo tiene
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var res = await userManager.AddToRoleAsync(user, roleName);
                if (!res.Succeeded) throw new Exception(string.Join(" | ", res.Errors.Select(e => e.Description)));
            }
        }
    }
}
