
using HospitalManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var razorpayConfig = new HospitalManagementSystem.Models.RazorpayConfig();
            builder.Configuration.GetSection("Razorpay").Bind(razorpayConfig);
            builder.Services.AddSingleton(razorpayConfig);

            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IBedsRepository, BedsRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IBillingRepository, BillingRepository>();
            builder.Services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<IReportsAnalyticsRepository, ReportsAnalyticsRepository>();

            // Load configuration from user secrets (only for development environment)
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>(); // Use your entry class (Program)
            }


            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use Session Middleware
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=DashBoard}/{action=adminDashBoard}/{id?}");

            app.Run();
        }
    }
}
