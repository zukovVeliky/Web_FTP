using FileManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var connect = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<CKEditorB.DB_Context.AdamekContext>(options => options.UseSqlServer(connect));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WebApplication1.Services.ComponentSettings.IComponentSettingsProvider, WebApplication1.Services.ComponentSettings.JsonComponentSettingsProvider>();
builder.Services.AddScoped<WebApplication1.Services.CustomFileManagerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseWebSockets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddlewareFileManager();

app.MapRazorPages();
app.MapControllers();

app.Run();
