using System;
using CommonServiceLocator;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Common;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.ProcessC4U;
using Count4U.Model.ProcessC4U;
using Count4U.Model.Transfer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;
using Count4U.Service.Common;
using Monitor.Service.Model.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Count4U.Service.Shared;
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Monitor.Service.Urls;
using System.Text;
using Count4U.Model.ExportImport.Service;
using Monitor.Service.Shared;
using Monitor.Service.Model;
using Service.Filter;

namespace Count4U.Service.WebAPI.Model
{
	//[ApiConventionType(typeof(DefaultApiConventions))]
	public class StartupModelWebAPI
	{
		//http://qaru.site/questions/69404/resolving-instances-with-aspnet-core-di
		//����� ���������� ����� �������� ������ � ����������� ������ Startup, 
		//����� ��� IHostingEnvironment, IConfiguration � IServiceProvider. 
		//�������� ��������, ��� ���� ��������� ����� �������� �����������, ��������� �� ������ ��������, 
		//� �������� ������ ������ ��� ������� ����������.
		//public Startup(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment,	ILoggerFactory loggerFactory)
		//{
		//	var hostingEnv = serviceProvider.GetService<IHostingEnvironment>();
		//}
		public StartupModelWebAPI(IConfiguration configuration, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			Configuration = configuration;
		}


		public IConfiguration Configuration { get; }

		//���� ���� ���������� ������
		//public void ConfigureServices(IServiceCollection services)
		//{
		//	services.AddSingleton<IFooService, FooService>();

		//	// Build the intermediate service provider
		//	var sp = services.BuildServiceProvider();
		//	var fooService = sp.GetService<IFooService>();
		//}
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();   //��������� ��� ������ ������ ������������ ����������
			services.AddControllers().
				AddNewtonsoftJson(); 

			services.AddMvc()
				.AddNewtonsoftJson()
				.AddControllersAsServices(); // IMPORTANT: Adding this line instructs WebHost to resolve Controllers from DI (Unity)

			 services.AddHttpClient();
			 services.AddSignalR();

			services.AddSession();

			services.AddLogging();
			services.AddLogging(config =>
			{
				// clear out default configuration
				config.ClearProviders();

				config.AddConfiguration(Configuration.GetSection("Logging"));
				config.AddDebug();
				config.AddEventSourceLogger();

				if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ==
				Microsoft.Extensions.Hosting.Environments.Development)
				{
					config.AddConsole();
				}
			});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Count4U.Service.WebAPI.Model",
					Description = "COUNT4U.SERVICE WEB API "
				});

				//// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
				c.CustomSchemaIds(i => i.FullName);
			});

			services.AddHttpContextAccessor();// �� ������ ��������� ������ IHttpContextAccessor � ������������ �������. ���� ������ IHttpContextAccessor �� ����� �������� User, �� ���� ��� ������ � ������� HttpContext, ������� ����� �������� User. 

			services.AddOptions();    //			��������� ������� ��������� ��� ��������� IOptions<T>�������� �� ������ � ��� ���, ����������� ������� ������������ �� ��������.����� �� ������������� ���� ����������� ����� ������������ � ������������� ��� � �������� ������������, ������� �� ������ ������������ ��� ������ ������.
			
			// Bind the configuration using IOptions   https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/
			services.Configure<Count4uSettings>(Configuration.GetSection("Count4USettings"));
			// Explicitly register the settings object so IOptions not required (optional)
			services.AddSingleton(resolver =>
				resolver.GetRequiredService<IOptions<Count4uSettings>>().Value);            //IOptionsSnapshot
																							 // Register as an IValidatable
			services.AddSingleton<IValidatable>(resolver =>
				resolver.GetRequiredService<IOptions<Count4uSettings>>().Value);

			//services.AddSingleton<ICount4USettings>(resolver =>
			//resolver.GetRequiredService<IOptions<Count4USettings>>().Value);


			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IIturService, IturService>();
			//services.AddScoped<IAdapterInitialazeService, AdapterInitialazeService>();
			services.AddScoped<IImportFromPdaService, ImportFromPdaService>();
			//services.AddHttpClient<IImportPdaNativPlusSqliteService, ImportPdaNativPlusSqliteService>(client =>
		 //   {	   		client.Timeout = TimeSpan.FromMinutes(30);
			//});
			 services.AddScoped<ControllerDb3ContextServiceFilter>();
		//	services.AddScoped<ControllerSignalRChatHubFilter>();
			//	 services.AddHostedService<MyService>();
			//https://docs.microsoft.com/ru-ru/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice
			//https://developpaper.com/the-correct-way-to-use-rabbitmq-in-net-core/
			//https://www.roundthecode.com/dotnet/how-to-use-signalr-when-receiving-a-message-from-a-rabbitmq-queue-in-net-core
			//https://altkomsoftware.pl/en/blog/building-microservices-6/
			//https://medium.com/trimble-maps-engineering-blog/getting-started-with-net-core-docker-and-rabbitmq-part-3-66305dc50ccf
			//https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio
			//https://docs.microsoft.com/ru-ru/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice

			//services.Configure<KestrelServerOptions>(options =>
			//{
			//  options.AllowSynchronousIO = true;
			//});
			//services.Configure<IISServerOptions>(options =>
			//{
			//  options.AllowSynchronousIO = true;
			//});
			// �������� ������� ������� ��� ���������������� ������ 400 �������� �������� 
			services.EnableLoggingForBadRequests();

			ConfigureHubSignalRClient(services);
		}

		//http://qaru.site/questions/69404/resolving-instances-with-aspnet-core-di
		// ������ ����� ����� ���� ��������� � ����� Configure(). 
		//�� ������ �������� ������������ ������ ���������� ����� ��������� IApplicationBuilder.
		//�� ����� ������ �������� ���� ����������� �������, ������� ���������������� � ������ ConfigureServices() 
		//����� ��� ����� �������� �� ����������� ����� ��������, � ����������� ����� ����������.
		//public void Configure(IApplicationBuilder app)
		//{
		//	var serviceProvider = app.ApplicationServices;
		//	var hostingEnv = serviceProvider.GetService<IHostingEnvironment>();
		//}
		//public void Configure(
		//IApplicationBuilder application,
		//IHostingEnvironment hostingEnvironment,
		//IServiceProvider serviceProvider,
		//ILoggerFactory loggerfactory,
		//IApplicationLifetime applicationLifetime)
		//{
		//}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<StartupModelWebAPI>();
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			if (env.IsDevelopment())
			{
				logger.LogInformation("Is Development enviroment");
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core 3.0 web API v1");
				//c.RoutePrefix = string.Empty;
			});

			app.UseRouting();

			app.UseCors(policy =>
			policy.AllowAnyOrigin() //WithOrigins("http://localhost:5000", "https://localhost:5001"
			.AllowAnyMethod()
			//.AllowAnyHeader()
			.WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization));//.AllowCredentials());

			app.UseAuthorization();

			app.UseSession(); //�� ������ ������������ ������ � ������� HttpContext //context.Session.Keys.Contains("name")
							  //app.UseMailAsCorrelationId();

			app.UseCount4uHeaderFromJWT();       //Always!!! must use 
			app.UseCount4uContextFromHeader();       //����� 	 app.UseCount4uHeaderFromJWT();

			app.UseLoggerDebugPathAndCount4uContext();

			app.UseStatusCodePagesWithReExecute("/error/{0}");      //https://www.devtrends.co.uk/blog/handling-errors-in-asp.net-core-web-api
			app.UseExceptionHandler("/error/500");
			
			//string chatHubAddress = Opetarion.UrlCombine(Monitor.Service.Model.SignalRHub.HostHub, Monitor.Service.Model.SignalRHub.ChatHub);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			//	endpoints.MapHub<Count4U.Service.Model.ChatHub>(chatHubAddress); ��� ������ ������� � �� �������
			});
		}

		public void ConfigureContainer(IUnityContainer container)
		{
			//https://unitycontainer.github.io/tutorials/lifetime/hierarchical.html
			//https://github.com/unitycontainer/unity/wiki/Unity-Lifetime-Managers
			//container.RegisterInstance(typeof(IService), "xyz", instance)

			container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(container));
			


			//Test
			container.RegisterType<IWeatherForecastTest, WeatherForecastTest1>("WeatherForecastTest1", TypeLifetime.Scoped);
			container.RegisterType<IWeatherForecastTest, WeatherForecastTest>("WeatherForecastTest", TypeLifetime.Scoped);
			container.RegisterType<IWeatherForecastWebApi, WeatherForecastWebApi>(TypeLifetime.Scoped);
			//=======================  Init Count4U  ===============================
			try
			{
				//container.RegisterType<IUserSettingsManager, UserSettingsManager>( TypeLifetime.Scoped);
				container.RegisterType<ISettingsRepository, SettingsRepository>( TypeLifetime.Scoped);
				container.RegisterType<IDBSettings, DBSettings>( TypeLifetime.Scoped);
				container.RegisterType<IConnectionDB, ConnectionDB>( TypeLifetime.Scoped);
				container.RegisterType<IZip, Zip>( TypeLifetime.Scoped);
			//	container.RegisterType<IApplicationVersion, ApplicationVersion>( TypeLifetime.Scoped);
				container.RegisterType<IAuditConfigRepository, AuditConfigEFRepository>( TypeLifetime.Scoped);
				container.RegisterType<IContextCBIRepository, ContextCBIRepository>(TypeLifetime.Scoped);
				
				container.RegisterType<IProcessRepository, ProcessEFRepository>( TypeLifetime.Scoped);

				container.RegisterType<ICount4uContext, Count4uContext>(TypeLifetime.Scoped);
				container.RegisterType<ICommandResultService, CommandResultService>(TypeLifetime.Scoped);  
				container.RegisterType<IProcessService, ProcessService>(TypeLifetime.Scoped);  
				container.RegisterType<ISettingsdb3Repository, Settingsdb3Repository>(TypeLifetime.Scoped);
				container.RegisterType<DataflowImportQueue, DataflowImportQueue>(new ContainerControlledLifetimeManager());  
				


				//container.RegisterType<IConnectionMonitorDB, ConnectionMonitorDB> (TypeLifetime.Scoped);
				//container.RegisterType<ICommandResultRepository, CommandResultEFRepository>( TypeLifetime.Scoped);
			
				//container.RegisterType<IInit1ImportPdaNativPlusSqliteAdapterHandler, Init1ImportPdaNativPlusSqliteAdapterHandler>(TypeLifetime.Scoped);
				//IHubContext context = Startup.ConnectionManager.GetHubContext<SomeHub>();

				//context.Clients.All.someMethod();
					//container.Resolve<ISettingsRepository>().LogPath = "";//App.LogPath;  TO DO
				//container.Resolve<ISettingsRepository>().StartupArgumentDictionary = new Dictionary<string, string>(); // TO DO//App.StartupArgumentDictionary;
				string ProcessCode = "";


				try
				{
					IServiceLocator serviceLocator = container.Resolve<IServiceLocator>();
					IOptions<Count4uSettings> count4USettings = serviceLocator.GetInstance<IOptions<Count4uSettings>>();
					FileSystem._count4USettings = count4USettings.Value;
					//IProcessRepository processRepository = serviceLocator.GetInstance<IProcessRepository>();
					//ProcessCode = processRepository.GetProcessCode_InProcess(count4USettings.Value);
					//container.Resolve<ISettingsRepository>().ProcessCode = ProcessCode;
				}
				catch (Exception exc)
				{
					throw exc;
				}
				//container.Resolve<ISettingsRepository>().ProcessCode = ProcessCode;

				DBModuleInit.InitializeStatic(container);
				ExportImportModuleInit.InitializeStatic(container);
			}
			catch (Exception exc)
			{
				//_logger.DebugException("InitializeShell error", exc);
			}

		
		}
		public void ConfigureHubSignalRClient(IServiceCollection services)
		{
			//HubSignalRRepository _hubSignalRRepository = 	new HubSignalRRepository();
			//string chatHubAddress = Opetarion.UrlCombine(Monitor.Service.Model.SignalRHub.HostHub, Monitor.Service.Model.SignalRHub.ChatHub);
			//try
			//{
			//	if (_hubSignalRRepository.HubChatConnection == null)
			//	{
			//		_hubSignalRRepository.HubChatConnection = new HubConnectionBuilder()
			//		  .WithUrl(chatHubAddress)
			//		  .WithAutomaticReconnect()
			//		  .Build();
			//		Console.WriteLine("webAPI ConfigureHubSignalRClient");
			//	}

			//	_hubSignalRRepository.HubChatConnection.On<string, string>(SignalRHubFunction.ReceiveMessage, (user, message) =>
			//	{
			//		Console.WriteLine(user + " says " + message);
			//	});
			//}
			//catch (Exception ex	)
			//{
			//		 	Console.WriteLine("ERROR ConfigureHubSignalRClient  " + ex.Message);
			//	}
			services.AddSingleton<IHubChatSignalRRepository, HubChatSignalRRepository>();
			services.AddSingleton<IHubCommandSignalRRepository, HubCommandSignalRRepository>();
		}

		//case ServiceLifetime.Scoped:
  //                  return new HierarchicalLifetimeManager();
  //              case ServiceLifetime.Singleton:
  //                  return new InjectionSingletonLifetimeManager(lifetime);
  //              case ServiceLifetime.Transient:
  //                  return new InjectionTransientLifetimeManager();
	}
}
