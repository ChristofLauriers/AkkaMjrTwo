using Akka.Actor;
using AkkaMjrTwo.Infrastructure.Akka;
using AkkaMjrTwo.UI.Actor;
using AkkaMjrTwo.UI.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace AkkaMjrTwo.UI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSignalR();

            // Register ActorSystem
            services.AddSingleton(_ => ConfigureActorSystem());

            services.AddSingleton<EventHubHelper, EventHubHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<EventHub>("/hub/event");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.ApplicationServices.GetService<EventHubHelper>()
                                   .StartAsync(CancellationToken.None);

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

            actorSystem.ActorOf(Props.Create<EventSubscriberActor>(), "UIEventSubscriber");

            return actorSystem;
        }
    }
}
