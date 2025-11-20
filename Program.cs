using E_TestHub.Services;
using QuestPDF.Infrastructure;

namespace E_TestHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure QuestPDF license (Community license for non-commercial use)
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add session support with larger size for import
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IOTimeout = TimeSpan.FromMinutes(30);
            });
            
            // Configure session cache with larger size
            builder.Services.AddDistributedMemoryCache();

            // Register HttpContextAccessor (needed for ApiService to access Session)
            builder.Services.AddHttpContextAccessor();

            // Register HttpClient for API calls
            builder.Services.AddHttpClient<IApiService, ApiService>();
            
            // Register API services (for MongoDB integration)
            builder.Services.AddScoped<IUserApiService, UserApiService>();
            builder.Services.AddScoped<ISubjectApiService, SubjectApiService>();
            builder.Services.AddScoped<IClassApiService, ClassApiService>();
            builder.Services.AddScoped<IQuestionApiService, QuestionApiService>(); // Phase 2
            builder.Services.AddScoped<IExamApiService, ExamApiService>();         // Phase 2
            builder.Services.AddScoped<IExamScheduleApiService, ExamScheduleApiService>(); // Phase 2 - Option 2
            builder.Services.AddScoped<ISubmissionApiService, SubmissionApiService>(); // Phase 3 - Student Take Exam

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

            // Add session middleware (must be before UseAuthorization)
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
