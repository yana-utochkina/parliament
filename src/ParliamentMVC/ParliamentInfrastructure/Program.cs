using ParliamentInfrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.Models.QRCodePOC;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/{0}.cshtml");
    });

builder.Services.AddDbContext<ParliamentDbContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDbContext<IdentityContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("IdentityConnection")
    ));

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<DefaultUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<DefaultUser>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var parliamentDbContext = services.GetRequiredService<ParliamentDbContext>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager, parliamentDbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database. " + DateTime.Now.ToString());
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/GenerateQRCode", (string text) =>
{
    //generate guid
    var guid = Guid.NewGuid().ToString();
    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    QRCodeData qrCodeData = qrGenerator.CreateQrCode(text + "\r\n" + guid, QRCodeGenerator.ECCLevel.Q);
    QRCode qrCode = new QRCode(qrCodeData);
    Bitmap qrCodeImage = qrCode.GetGraphic(20);
    // use this when you want to show your logo in middle of QR Code and change color of qr code
    Bitmap logoImage = new Bitmap(@"wwwroot/img/aircodlogo.jpg");
    // Generate QR Code bitmap and convert to Base64
    using (Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.WhiteSmoke, logoImage))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            qrCodeAsBitmap.Save(ms, ImageFormat.Png);
            string base64String = Convert.ToBase64String(ms.ToArray());
            var data = "data:image/png;base64," + base64String;
            return data;
        }
    }
});

app.Run();
