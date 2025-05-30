using Microsoft.EntityFrameworkCore;
using Projekt001.Data;
using Projekt001.Repositories;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
 
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString)); 
        builder.Services.AddScoped<IContactFormRepository, ContactFormRepository>();

       
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error"); // Configure a generic error handler page
            app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        }
        else
        {
            app.UseDeveloperExceptionPage(); 
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles(); // To serve static files like CSS, JavaScript, images

        app.UseRouting(); // Enables routing

        app.UseAuthentication(); // If you add authentication
        app.UseAuthorization(); // If you add authorization

        // Define default route and other routes
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}