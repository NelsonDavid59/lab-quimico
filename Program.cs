using System.Globalization;
using AgroLaboratorio.Controllers.ActionFilters;
using AgroLaboratorio.Data;
using AgroLaboratorio.Models;
using AgroLaboratorio.Repository;
using AgroLaboratorio.Repository.Personas;
using AgroLaboratorio.Repository.SolsAnalisis;
using AgroLaboratorio.Services;
using AgroLaboratorio.Services.Lovs;
using AgroLaboratorio.Services.Personas;
using AgroLaboratorio.Services.SolsAnalisis;
using AgroLaboratorio.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//REPOSITORIES
builder.Services.AddScoped<IElemQuimicoRep, ElemQuimicoRep>();
builder.Services.AddScoped<IPersonaRep, PersonaRep>();
builder.Services.AddScoped<ISolAnalisisRep, SolAnalisisRep>();

//SERVICES
builder.Services.AddScoped<IElemQuimicoServ, ElemQuimicoServ>();
builder.Services.AddScoped<IPersonaServ, PersonaServ>();
builder.Services.AddScoped<ISolAnalisisServ, SolAnalisisServ>();
builder.Services.AddScoped<LovServ>();

//UNIT OF WORK
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Filtros de Validación
builder.Services.AddScoped<ValidateSolAnalisisFilter>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (value, fieldName) => $"'{value}' no es un valor válido para el campo {fieldName}");
});

//Configuracion de Paginas
builder.Services.Configure<PageConfig>(builder.Configuration.GetSection("PageConfig"));

/* Configuración de los DBCONTEXT */
builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AuthPostgresConnection")));
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppPostgresConnection")));

//Configuración de Identity
builder.Services.AddIdentity<Usuario, Perfil>(options => {
    options.Password.RequiredLength = 8; //Tamaño de contraseña esperada
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

//Esquema de Autenticación con Cookies
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Users/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
});

//Definir la cultura por defecto
var defaultCulture = new CultureInfo("es-PY");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

Thread.CurrentThread.CurrentCulture = defaultCulture;
Thread.CurrentThread.CurrentUICulture = defaultCulture;

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Se utiliza el seeder para cargar los datos iniciales
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedData(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
