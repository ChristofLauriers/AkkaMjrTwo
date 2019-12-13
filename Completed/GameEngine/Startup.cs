using Akka.Actor;
using AkkaMjrTwo.GameEngine.Actor;
using AkkaMjrTwo.GameEngine.Infrastructure;
using AkkaMjrTwo.Infrastructure;
using AkkaMjrTwo.Infrastructure.Akka;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AkkaMjrTwo.GameEngine
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register ActorSystem
            services.AddSingleton(_ => ConfigureActorSystem());

            services.AddSingleton<GameManagerActorProvider>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();
                var gameManagerActor = actorSystem.ActorOf(GameManagerActor.GetProps());
                return () => gameManagerActor;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameEngine", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors();

            //ActorSystem lifetime management
            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>(); // start Akka.NET
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>()?.Terminate().Wait();
            });
        }

        private static ActorSystem ConfigureActorSystem()
        {
            return ActorSystem.Create("DiceGameSystem", ConfigurationLoader.Load());
        }
    }
}
