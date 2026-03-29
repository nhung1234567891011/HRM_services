using HRM_BE.Api.Filter;
using HRM_BE.Api.Hubs;
using HRM_BE.Api.Managers;
using HRM_BE.Api.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Serilog;
using Microsoft.AspNetCore.Mvc.Formatters;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Đặt mức log tối thiểu
    .WriteTo.Console()    // Ghi log ra console
    .WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Day) // Ghi log ra file hàng ngày
    .CreateLogger();

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);


// Provider
builder.Services.AddHangfire( config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddSignalR();
builder.Services.AddAppProvider();
builder.Services.AddEntityFrameworkProvider(builder);
builder.Services.AddIdentityProvider(builder);
builder.Services.AddFluentValidationProvider();
builder.Services.AddDependencyInjectionProvider();
builder.Services.AddSwaggerProvider();
builder.Services.AddAutoMapperProvider();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScopedProvider();




builder.Services.AddControllers(
    options =>
    {
        // handle exceptions thrown by an action
        options.Filters.Add(new ApiExceptionFilterAttribute());

        options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    }
  ).AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region ignore
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
#endregion

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EUCHOICE API V1");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard();
app.MapControllers();
app.MapHub<RefreshTokenHub>("/hubs/refresh-token-hub");
app.MapHub<RemindWorkHub>("/hubs/remind-work-notification-hub");


//app.MigrateDatabase();

app.Run();
// cấu hình path 
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();
