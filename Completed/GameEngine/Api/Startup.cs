using System;
using Akka.Actor;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Amqp;
using Akka.Streams.Amqp.Dsl;
using Akka.Streams.Dsl;
using AkkaMjrTwo.GameEngine.Actor;
using AkkaMjrTwo.GameEngine.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AkkaMjrTwo.GameEngine.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private ILogger Logger;


        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register ActorSystem
            services.AddSingleton(_ => ConfigureActorSystem());

            services.AddSingleton<GameManagerActorProvider>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();
                var gameManagerActor = actorSystem.ActorOf(Props.Create<GameManager>());
                return () => gameManagerActor;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            lifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>(); // start Akka.NET
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });
        }

        private ActorSystem ConfigureActorSystem()
        {
            try
            {
                var actorSystem = ActorSystem.Create("DiceGameSystem", ConfigurationLoader.Load());

                //var materializer = ActorMaterializer.Create(actorSystem);

                //var connectionSettings = AmqpConnectionDetails.Create("", 1234)
                //    .WithAutomaticRecoveryEnabled(true)
                //    .WithNetworkRecoveryInterval(TimeSpan.FromSeconds(1));

                ////Get queuename from settings
                //var queueName = "";
                //var queueDeclaration = QueueDeclaration.Create(queueName).WithDurable(false).WithAutoDelete(true);

                //var amqpSink = AmqpSink.CreateSimple(
                //    AmqpSinkSettings.Create(connectionSettings)
                //                    .WithRoutingKey(queueName)
                //                    .WithDeclarations(queueDeclaration));

                //var amqpSource = AmqpSource.AtMostOnceSource(
                //    NamedQueueSourceSettings.Create(DefaultAmqpConnection.Instance, queueName)
                //                            .WithDeclarations(queueDeclaration), bufferSize: 10);

                //var result =
                    //Source.ActorPublisher<GameEvent>(EventPublisherActor.GetProps())
                          //.Select(ev => ToByteString(ev))
                          //.RunWith(amqpSink, materializer);

                return actorSystem;
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
        }

        private static ByteString ToByteString(GameEvent @event)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(@event);
            return ByteString.FromString(json);
        }
    }
}
