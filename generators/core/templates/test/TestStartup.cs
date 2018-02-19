using System;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
<%_ if(authentication) { _%>
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
<%_ } _%>
<%_ if(addDatabase && database === 'dynamodb') { _%>
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Moq;
<%_ } _%>
<%_ if(addDatabase && database !== 'dynamodb') { _%>
using Microsoft.EntityFrameworkCore;
<%_ } _%>

namespace <%= namespace %>.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory) : base(configuration, env, loggerFactory)
        {
            Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();

        }

        <%_ if(addDatabase) { _%>
        <%_ if(database === 'dynamodb') { _%>
        protected void RunMigrationAndSeed(ProvisionDynamodb provisionDynamodb<%_ if(createModel) { _%>, I<%= modelName %>Repository <%= modelNameCamel %>Repository<%_ } _%>)
        {
          <%_ if(createModel) { _%>
          <%= modelNameCamel %>Repository.SaveAsync(BogusData.Get<%= modelName %>());
          <%= modelNameCamel %>Repository.SaveAsync(BogusData.Get<%= modelName %>());
          <%= modelNameCamel %>Repository.SaveAsync(BogusData.Get<%= modelName %>());
          <%_ } _%>
        }

        protected override void AddAmazonDynamoDB(IServiceCollection services)
        {
            services.AddSingleton<IDynamoDBContext>(new Mock<IDynamoDBContext>().Object);
            services.AddSingleton<IAmazonDynamoDB>(new Mock<IAmazonDynamoDB>().Object);
        }
        <%_ } else { _%>
        /// <summary>
        /// Runs the code first migration on startup of service
        /// </summary>
        /// <param name="db">The database contect object.</param>
        protected void RunMigrationAndSeed(<%= appname %>Context db)
        {
            <%_ if(createModel) { _%>
            db.<%= modelName %>s.Add(BogusData.Get<%= modelName %>());
            db.<%= modelName %>s.Add(BogusData.Get<%= modelName %>());
            db.<%= modelName %>s.Add(BogusData.Get<%= modelName %>());
            db.SaveChanges();
            <%_ } _%>
        }

        protected override void AddEntityFramework(IServiceCollection services)
        {
            var _serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<<%= appname %>Context>(options => options.UseInMemoryDatabase("<%= appname %>").UseInternalServiceProvider(_serviceProvider));
        }
        <%_ } _%>
        <%_ } _%>

        protected override void AddExternalServices(IServiceCollection services)
        {
            // Configuration for AutoMapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<I<%= appname %>Service, <%= appname %>Service>();
            <%_ if(kafkaConsumer || createService) { _%>
            services.AddSingleton<HttpClient>(new HttpClient(new FakeResponseHandler()));
            <%_ } _%>
            
            <%_ if(createService) { _%>
            //External Services
            <%_ for(var i = 0; i < externalServices.length; i++) { _%>
            var <%= externalServices[i].serviceName.toLowerCase() %>ServiceConfig = new <%= externalServices[i].serviceName %>ServiceConfig();
            Configuration.GetSection("ExternalServices:<%= externalServices[i].serviceName %>").Bind(<%= externalServices[i].serviceName.toLowerCase() %>ServiceConfig);
            services.AddSingleton<<%= externalServices[i].serviceName %>ServiceConfig>(<%= externalServices[i].serviceName.toLowerCase() %>ServiceConfig);
            services.AddScoped<I<%= externalServices[i].serviceName %>Service, <%= externalServices[i].serviceName %>Service>();

            <%_ } _%>
            <%_ } _%>

            <%_ if(createModel) { _%>
            <%_ if(addDatabase && database === 'dynamodb') { _%>
            services.AddSingleton<I<%= modelName %>Repository, <%= modelName %>RepositoryStub>();
            <%_ } else { _%>
            services.AddScoped<I<%= modelName %>Repository, <%= modelName %>Repository>();
            <%_ } _%>
            <%_ } _%>
            <%_ if(addDatabase && database === 'dynamodb') { _%>
            services.AddScoped<ProvisionDynamodb, ProvisionDynamodb>();
            <%_ } _%>
            <%_ if(kafka) { _%>

            //Kafka
            var kafkaConfig = new KafkaConfig();
            Configuration.GetSection("Kafka").Bind(kafkaConfig);
            services.AddSingleton<KafkaConfig>(kafkaConfig);
            services.AddSingleton<IKafkaProducer, KafkaProducerStub>();
            <%_ } _%>
            <%_ if(kafkaConsumer) { _%>
            services.AddSingleton<IKafkaConsumer, KafkaConsumerStub>();
            services.AddSingleton<IKafkaConsumerManager, KafkaConsumerManagerStub>();
            <%_ } _%>
        }
        <%_ if(authentication) { _%>

        /// <summary>
        /// Adds auth specific services.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        public override IServiceCollection AddJwtAuthServices(IServiceCollection services)
        {
            //  Add Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Create", policy => policy.RequireClaim(ClaimTypes.Groups, new[] { GroupTypes.Admin }));
                options.AddPolicy("Update", policy => policy.RequireClaim(ClaimTypes.Groups, new[] { GroupTypes.Admin }));
                options.AddPolicy("View", policy => policy.RequireClaim(ClaimTypes.Groups, new[] { GroupTypes.Admin }));
                options.AddPolicy("Delete", policy => policy.RequireClaim(ClaimTypes.Groups, new[] { GroupTypes.Admin }));
            });

            //  Add JWT token validation and configs
            var jwtConfig = new JwtConfig();
            Configuration.GetSection("JWTSettings").Bind(jwtConfig);
            services.AddSingleton<JwtConfig>(jwtConfig);

            // Setup Jwt options
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidIssuer = jwtConfig.OrganizationUrl,

                ValidateAudience = true,
                ValidAudience = jwtConfig.ClientId,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthTokenHelper.SigningKey,

                ClockSkew = TimeSpan.Zero
            };

            var jwt = new JwtBearerOptions
            {
                SaveToken = true,
                TokenValidationParameters = tokenValidationParameters
            };
            services.AddSingleton<JwtBearerOptions>(jwt);


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = jwt.SaveToken;
                options.MetadataAddress = jwt.MetadataAddress;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }
        <%_ } _%>

        protected override void ConfigureExternal(IApplicationBuilder app)
        {
            <%_ if(addDatabase) { _%>
            <%_ if(database === 'dynamodb') { _%>
            var provisionDynamodb = app.ApplicationServices.GetService<ProvisionDynamodb>();
            var repo = app.ApplicationServices.GetService<I<%= modelName %>Repository>();
            RunMigrationAndSeed(provisionDynamodb, repo);
            <%_ } else { _%>
            var context = app.ApplicationServices.GetService<MyTestContext>();
            RunMigrationAndSeed(context);
            <%_ } _%>
            <%_ } _%>
        }
    }
}
