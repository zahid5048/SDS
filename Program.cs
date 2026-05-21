using Microsoft.EntityFrameworkCore;
using ChemicalSDS.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ChemicalSDS.Services.SdsWizardService>();
builder.Services.AddScoped<ChemicalSDS.Services.ChemicalListService>();
builder.Services.AddScoped<ChemicalSDS.Services.ChemicalDeletionService>();
builder.Services.AddScoped<ChemicalSDS.Services.UserAuthService>();
builder.Services.AddSingleton<ChemicalSDS.Services.SdsPdfGenerator>(sp =>
    new ChemicalSDS.Services.SdsPdfGenerator(sp.GetRequiredService<IWebHostEnvironment>()));

// Session for login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    var auth = scope.ServiceProvider.GetRequiredService<ChemicalSDS.Services.UserAuthService>();
    await auth.EnsureAdminSeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();