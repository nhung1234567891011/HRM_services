namespace HRM_BE.Api.Providers
{
    public static class AppProvider
    {
        public static IServiceCollection AddAppProvider(this IServiceCollection services)
        {



            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379"; // Địa chỉ Redis server 
            //    options.InstanceName = "SampleInstance";
            //});


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    //.AllowAnyOrigin()
                    //.AllowAnyMethod()
                    //.AllowAnyHeader());
                    .WithOrigins("http://localhost:4200", "https://localhost:4200", "http://103.153.69.217:6009",
                    "http://103.153.69.217:7009", "https://103.153.69.217:7009", "https://103.153.69.217:6009", "http://hrm.smomedia.vn", "https://hrm.smomedia.vn", "https://hrm-web-git-master-mtuans-projects.vercel.app/", "http://chiasecode.net", "https://chiasecode.net", "https://hrm-web-iota.vercel.app/")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None; 
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;


            });
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpClient();


            //services.AddMemoryCache(); //  IMemoryCache


            return services;
        }
    }
}


