using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Command.AggregateFactories;
using BusinessLayer.Command.CommandHandlers;
using BusinessLayer.Command.CommandHandlers.Test;
using BusinessLayer.Command.Commands.Test;
using BusinessLayer.Command.DomainEvent;
using BusinessLayer.Query.Queries;
using Common;
using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Dapper;
using Data.Reader.Readers.Generic;
using Data.Writer;
using Data.Writer.Writers.Generic;
using Domain.SharedKernel.DomainEvents;
using Message;
using Message.Dispatchers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using Resume.Api.Middlewares;
using Resume.Api.Middlewares.HealthCheck;
using Resume.Api.Workers;

namespace Resume.Api
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
            services.AddDefaultCorrelationId();
            services.AddHostedService<SubscriptionWorker>();
            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "Resume Api", Version = "v1" }));
           
            services
                .AddSingleton<IConfigManager, ConfigManager>()
                .AddSingleton<IMessageBus, InMemoryMessageBus>()
                .AddSingleton<IDispatcher,LocalDispatcher>()
                .AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            services
                .AddScoped<ILogsQuery, LogsQuery>()
                .AddScoped(typeof(IReader<,>), typeof(Reader<,>))
                .AddScoped(typeof(ITpyeWriter<,>), typeof(TypeWriter<,>))
                .AddScoped<IDbConnection>(x=>new NpgsqlConnection(x.GetService<IConfigManager>().AppConfig.ConnectionString))
                .AddScoped<IDbContext>(x=>new DbContext(x.GetService<IConfigManager>().AppConfig.ConnectionString))
                .AddScoped<DomainEventActionFilter>()
                .AddScoped<ITestAggregateFactory, TestAggregateFactory>()
                .AddScoped<ICommandHandler<int, TestFailCommand>, TestFailCommandHandler>()
                .AddScoped<ICommandHandler<int, TestPassCommand>, TestPassCommandHandler>();

            services
                .AddHttpClient(Constants.DefaultClient)
                .AddCorrelationIdForwarding();

            services
                .ConfigureAuthentication()
                .ConfigureAuthorization()
                .ConfigureHealthCheck()
                .ConfigureLogging()
                .ConfigureCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId();
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Resume Api v1"));
            app.UseCorrelationId();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => x.AllowAnyOrigin());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ConfigureCustomHandler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.ConfigureHealthCheckEndpoint();
            });
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
    }
}
