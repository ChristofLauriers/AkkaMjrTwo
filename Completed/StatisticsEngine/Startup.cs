using Akka.Actor;
using AkkaMjrTwo.Infrastructure.Akka;
using AkkaMjrTwo.StatisticsEngine.Actor;
using AkkaMjrTwo.StatisticsEngine.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AkkaMjrTwo.StatisticsEngine
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StatisticsEngine", Version = "v1" });
            });

            // Register ActorSystem
            services.AddSingleton(_ => ConfigureActorSystem());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            //Create readmodel database
            using (var db = new GameStatisticsContext())
            {
                db.Database.EnsureCreated();
            }

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
            var actorSystem = ActorSystem.Create("DiceGameSystem", ConfigurationLoader.Load());

            actorSystem.ActorOf(Props.Create<EventSubscriberActor>(), "StatisticsEventSubscriber");

            return actorSystem;
        }
    }
}
