using FileProcessor.Services;
using DalSoft.Hosting.BackgroundQueue.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace FileProcessor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var redisConnectionString = Configuration.GetConnectionString("redis");
            var redisHostAndPort = redisConnectionString
                .Substring(0, redisConnectionString.IndexOf(','));

            services.AddControllers(); 
            services.AddBackgroundQueue(onException: exception => { });
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = redisConnectionString;
            });
            services.AddSingleton(ConnectionMultiplexer
                .Connect(redisConnectionString)
                .GetServer(redisHostAndPort));
            services.AddTransient(typeof(IFileService), typeof(FileService));
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
