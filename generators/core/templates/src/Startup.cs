using System;
using System.IO;
using System.Reflection;
using System.Net.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AutoMapper;
using Serilog;
using Serilog.Core;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
<%_ if(authentication) { _%>
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
<%_ } _%>
<%_ if(addDatabase && database === 'dynamodb') { _%>
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
<%_ } _%>
<%_ if(addDatabase && database !== 'dynamodb') { _%>
using Microsoft.EntityFrameworkCore;
<%_ } _%>

namespace <%= namespace %>
{
    /// <summary>
    /// The initializaton file for ASP.NET MVC web applications.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        private readonly string _apiVersion;

        private IHostingEnvironment _currentEnvironment { get; set; }

        private readonly ILoggerFactory _loggerFactory;

        <%_ if(database === 'dynamodb') { _%>
        private readonly string _tablePrefix;
        <%_ } _%>

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The environment configuration object.</param>
        /// <param name="env">The environment configuration object.</param>
        /// <param name="loggerFactory">The default logger factory</param>
        public Startup(
            IConfiguration configuration,
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _currentEnvironment = env;
            _loggerFactory = loggerFactory;
            _apiVersion = CreateAPIVersion(Configuration["VERSION_NUMBER"]);
            <%_ if(database === 'dynamodb') { _%>
            _tablePrefix = $"{_currentEnvironment.EnvironmentName.ToLower()}-<%= appname.toLowerCase() %>-";
            <%_ } _%>
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            
            services.AddSingleton(appSettings);
            
            services.AddMvcCore(opt =>
            {
                if (appSettings.GlobalExceptionFilterEnabled)
                {
                    opt.Filters.Add(new GlobalExceptionFilter(_loggerFactory));
                }
                opt.Filters.Add(new ValidateModelStateAttribute()); // Add global model state validation
            })
                .AddDataAnnotations()
                .AddJsonFormatters(j => j.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddApiExplorer();

            <%_ if(authentication) { _%>
            // Setup authentication
            AddJwtAuthServices(services);

            <%_ } _%>
            // Add CORS
            services.AddCors(options =>
            {
                // Define one or more CORS policies
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://example.com");
                    });

                options.AddPolicy("AllowAny",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            <%_ if(addDatabase && database === 'dynamodb') { _%>
            AddAmazonDynamoDB(services);
            <%_ } _%>
            <%_ if(addDatabase && database !== 'dynamodb') { _%>
            AddEntityFramework(services);
            <%_ } _%>
            AddExternalServices(services);
            services.AddMvc();
        }
        <%_ if(authentication) { _%>

        /// <summary>
        /// Adds auth specific services.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        public virtual IServiceCollection AddJwtAuthServices(IServiceCollection services)
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

                ClockSkew = TimeSpan.Zero
            };

            var jwt = new JwtBearerOptions
            {
                SaveToken = true,
                MetadataAddress = jwtConfig.OrganizationUrl + "/.well-known/openid-configuration",
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
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application configuration object.</param>
        /// <param name="env">The environment configuration object.</param>
        /// <param name="loggerFactory">The logger factory object.</param>
        <%_ if(authentication) { _%>
        /// <param name="jwtOptions">The jwt options settings.</param>
        <%_ } _%>
        <%_ if(createModel && database === 'dynamodb') { _%>
        /// <param name="<%= modelNameCamel %>Repository">The <see cref="<%= modelName %>"/> db Repository.</param>
        <%_ } _%>
        <%_ if(addDatabase) { _%>
        <%_ if(database === 'dynamodb') { _%>
        /// <param name="db">Used for creating the dynamodb table.</param>
        <%_ } else { _%>
        /// <param name="db">The database contect object.</param>
        <%_ } _%>
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
        <%_ if(authentication) { _%>
            JwtBearerOptions jwtOptions,
        <%_ } _%>
        <%_ if(database === 'dynamodb') { _%>
            <%_ if(createModel) { _%>
            I<%= modelName %>Repository <%= modelNameCamel %>Repository,
            <%_ } _%>
            ProvisionDynamodb db)
        <%_ } _%>
        <%_ if(database !== 'dynamodb') { _%>
            <%= appname %>Context db)
        <%_ } _%>
        <%_ } else { _%>
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
        <%_ if(kafkaConsumer) { _%>
            IKafkaConsumerManager kafkaConsumerManager,
            KafkaConfig kafkaConfig,
        <%_ } _%>
            ILoggerFactory loggerFactory)
        <%_ } _%>
        {
            loggerFactory.AddSerilog();
            loggerFactory.AddDebug();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            <%_ if(authentication) { _%>

            app.UseAuthentication();
            <%_ } _%>

            app.UseMvc();

            ConfigureExternal(app);
            <%_ if(kafkaConsumer) { _%>

            app.StartKafkaConsumer();
            <%_ } _%>
        }
        <%_ if(addDatabase) { _%>
        <%_ if(database === 'dynamodb') { _%>

        //See https://aws.amazon.com/articles/2790257258340776
        //Why it should be a singleton
        /// <summary>
        /// Configures AmazoneDynamoDb.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        protected virtual void AddAmazonDynamoDB(IServiceCollection services)
        {
            var dynamoDBContextConfig = new DynamoDBContextConfig
            {
                TableNamePrefix = _tablePrefix
            };
            services.AddSingleton<DynamoDBContextConfig>(dynamoDBContextConfig);
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
        }

        <%_ } else { _%>

        /// <summary>
        /// Configures EntityFrameworkCore.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        protected virtual void AddEntityFramework(IServiceCollection services)
        {
            var connection = Configuration["DEFAULT_CONNECTION"] ?? Configuration["Data:DefaultConnection:DefaultConnection"];
            <%_ if (database === 'mysql') { _%>
            services.AddDbContext<<%= appname %>Context>(options => options.UseMySql(connection));
            <%_ } _%>
            <%_ if (database === 'sqlserver') { _%>
            services.AddDbContext<<%= appname %>Context>(options => options.UseSqlServer(connection));
            <%_ } _%>
            <%_ if (database === 'postgresql') { _%>
            services.AddDbContext<<%= appname %>Context>(options => options.UseNpgsql(connection));
            <%_ } _%>
        }

        <%_ } _%>
        <%_ } _%>
        /// <summary>
        /// Configures external services.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        protected virtual void AddExternalServices(IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("apidoc",
                    new Info
                    {
                        Version = $"{_apiVersion}",
                        Title = "<%= appname %> Service",
                        Description = $"{_currentEnvironment.EnvironmentName.ToUpper()} Build v{Configuration["VERSION_NUMBER"]}",
                    }
                );
                config.IncludeXmlComments(Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "xml"));
                config.DescribeAllEnumsAsStrings();
            });

            // Configuration for AutoMapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<I<%= appname %>Service, <%= appname %>Service>();
            services.AddTransient<HttpClient, HttpClient>();
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
            services.AddScoped<I<%= modelName %>Repository, <%= modelName %>Repository>();
            <%_ } _%>
            <%_ if(addDatabase && database === 'dynamodb') { _%>
            <%_ if(createModel) { _%>
            services.AddScoped<ProvisionDynamodb, ProvisionDynamodb>();
            <%_ } _%>
            <%_ } _%>
            <%_ if(kafka && !kafkaConsumer) { _%>

            //Kafka
            var kafkaConfig = new KafkaConfig();
            Configuration.GetSection("Kafka").Bind(kafkaConfig);
            services.AddSingleton<KafkaConfig>(kafkaConfig);
            services.AddSingleton<IKafkaProducer, KafkaProducer>();
            <%_ } _%>
            <%_ if(kafkaConsumer) { _%>
            services.AddKafkaServices<<%= appname %>KafkaConsumer>(Configuration);
            <%_ } _%>
        }

        /// <summary>
        /// Configure external services. Everything not needed or
        /// need to be overridden by Test project goes here.
        /// </summary>
        /// <param name="app">An instance of <see cref="IApplicationBuilder"/> interface.</param>
        protected virtual void ConfigureExternal(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint
            string baseDirectory = Configuration["NGINX_REQUEST_URI"] ?? "/";

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.BasePath = baseDirectory);
            });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{baseDirectory}swagger/apidoc/swagger.json", $"V{_apiVersion.ToUpper() ?? "?"} API Docs");
            });
        }

        private string CreateAPIVersion(string version)
        {
            if (!string.IsNullOrWhiteSpace(version))
            {
                return $"v{version[0].ToString()}";
            }
            return string.Empty;
        }
    }
}
