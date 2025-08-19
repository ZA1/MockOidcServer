using System.Text;
using MockOidcServer.Certificates;
using MockOidcServer.Configurations;
using MockOidcServer.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Configuration.AddJsonFile("users.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonConfig(Environment.GetEnvironmentVariable("USERS"));

var usersBase64 = Environment.GetEnvironmentVariable("USERS_BASE64");
if (usersBase64 != null)
{
    builder.Configuration.AddJsonConfig(Encoding.UTF8.GetString(Convert.FromBase64String(usersBase64)));
}

builder.Services.Configure<UsersOptions>(builder.Configuration.GetSection(UsersOptions.SectionName));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = CertificateGenerator.HttpsCert;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders();
app.UseForceHttpsScheme();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
