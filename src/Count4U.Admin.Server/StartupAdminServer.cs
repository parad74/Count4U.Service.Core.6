using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Count4U.Service.Common.Filter;
using Count4U.Service.Common;
using Monitor.Service.Model.Settings;
using Microsoft.Extensions.Options;
//using Unity;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Monitor.Service.Shared;
using Monitor.Service.Model;
using Service.Filter;

namespace Count4U.Admin.Server
{
	public class StartupAdminServer
    {
        public StartupAdminServer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();   //��������� ��� ������ ������ ������������ ����������
			//services.AddControllers().
			services.AddControllersWithViews().
				AddNewtonsoftJson(); 
			services.AddRazorPages();


			//services.AddMvc()			 //add
			//.AddNewtonsoftJson(); //add
			// services.AddResponseCompression(opts =>
			//         {
			//             opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
			//                 new[] { "application/octet-stream" });
			//         });
			//services.AddControllersWithViews().AddNewtonsoftJson();

			services.AddMvc()
			.AddNewtonsoftJson()
			.AddControllersAsServices(); // IMPORTANT: Adding this line instructs WebHost to resolve Controllers from DI (Unity)


			services.AddHttpClient();
			services.AddSignalR();

			services.AddDistributedMemoryCache();      //https://metanit.com/sharp/aspnet5/2.11.php
			services.AddSession();

			services.AddHttpContextAccessor();      // �� ������ ��������� ������ IHttpContextAccessor � ������������. ���� ������ IHttpContextAccessor �� ����� �������� User, �� ���� ��� ������ � ������� HttpContext, ������� ����� �������� User. 

			//services.AddSingleton(new LoggerFactory()
			//   .AddNLog()
			//   );
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

			//�o��������� ������ � ������� � ������
			//var authConnection = SetupSqliteInMemoryConnection();
			//services.AddDbContext<ExtraAuthorizeDbContext>(options => options.UseSqlite(authConnection));

        

			services.AddOptions();    //			��������� ������� ��������� ��� ��������� IOptions<T>�������� �� ������ � ��� ���, ����������� ������� ������������ �� ��������.����� �� ������������� ���� ����������� ����� ������������ � ������������� ��� � �������� ������������, ������� �� ������ ������������ ��� ������ ������.
  			services.AddSingleton<IConfiguration>(Configuration);

			//https://blog.bredvid.no/validating-configuration-in-asp-net-core-e9825bd15f10
			//// Bind the configuration using IOptions   https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/
			services.Configure<WebAPISettings>(Configuration.GetSection("WebAPISettings"));
			// Explicitly register the settings object so IOptions not required (optional)
			services.AddSingleton(resolver =>
				resolver.GetRequiredService<IOptions<WebAPISettings>>().Value);            //IOptionsSnapshot
																						   // Register as an IValidatable
			services.AddSingleton<IValidatable>(resolver =>
				resolver.GetRequiredService<IOptions<WebAPISettings>>().Value);

			services.AddSingleton<IWebAPISettings>(resolver =>
		resolver.GetRequiredService<IOptions<WebAPISettings>>().Value);

			services.AddScoped<IJwtService, JwtService>();
			//services.AddTransient<HubConnectionBuilder>();
			// �������� ������� ������� ��� ���������������� ������ 400 �������� �������� 
			services.EnableLoggingForBadRequests();
	
			MapDependencyInjection(services);

			this.ConfigureHubSignalRClient(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env , ILoggerFactory loggerFactory)
        {
			//app.UseResponseCompression();
			Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<StartupAdminServer>();

			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")))
			//{
			//	// Needed because the test server runs on a different port than the client app,
			//	// and we want to test sending/receiving cookies underling this config
			//	WebAssemblyHttpMessageHandler.DefaultCredentials = FetchCredentialsOption.Include;
			//}

			//app.UseBlockingDetection();    // Blocking calls can lead to ThreadPool starvation. Ouputs a warning to the log when blocking calls are made on the ThreadPool.
			if (env.IsDevelopment())
            {
				logger.LogInformation("Is Development enviroment");
				app.UseDeveloperExceptionPage();
                //app.UseBlazorDebugging();
				 app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");

				//app.UseDatabaseErrorPage();
				// ��������� ������ HTTP
				//app.UseStatusCodePages();
				//app.UseStatusCodePages("text/plain", "Error. Status code : {0}");
				//app.UseHsts();
			}

			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();
	
			//app.UseClientSideBlazorFiles<Count4U.Admin.Client.Blazor.Program>();
			//app.UseHttpsRedirection(); //add
			//

			app.UseRouting();
			app.UseCors(policy =>
		policy.AllowAnyOrigin() //WithOrigins("http://localhost:5000", "http://localhost:27515"
		.AllowAnyMethod()
		.AllowAnyHeader()
		.WithExposedHeaders());//.AllowCredentials());

		//	app.UseStaticFiles();

			app.UseAuthentication();	  //???
            app.UseAuthorization();

			app.UseSession(); //�� ������ ������������ ������ � ������� HttpContext //context.Session.Keys.Contains("name")
			app.UseMailAsCorrelationId();
			//app.UseLoggerDebugPath();
			//app.UseLoggerConsolePath();
			app.UsePCBIContext();     //Always!!! must use 	 ��������� PCBIContext �� httpContext.Claims			   ?? �� ������� ���� ����� ��� ��� 

			//app.UseLoggerDebugPCBIContext();       //����� 	 app.UsePCBIContext();
			//app.UseLoggerInformationPathAndPCBIContext();	  //����� 	 app.UsePCBIContext();
			//app.UseLoggerConsolePathAndPCBIContext ();	  //����� 	 app.UsePCBIContext();
			app.UseLoggerDebugPathAndPCBIContext();       //����� 	 app.UsePCBIContext();

			app.UseEndpoints(endpoints =>
            {
				//endpoints.MapControllers(); // Map attribute-routed API controllers
				endpoints.MapDefaultControllerRoute();        // Map conventional MVC controllers using the default route
															  //endpoints.MapBlazorHub() ? 
															  //endpoints.MapFallbackToClientSideBlazor<Count4U.Service.Client.Blazor.StartupClientBlazor>("index.html");
															  //endpoints.MapHub<BlazorChatSample.Server.Hubs.ChatHub>("/chathub");
															  //endpoints.MapFallbackToClientSideBlazor<Count4U.Admin.Client.Blazor.Program>("index.html");
				endpoints.MapHub<ChatHub>(SignalRHub.ChatHub);
				endpoints.MapHub<CommandHub>(SignalRHub.CommandHub);
				endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
			});
        }


		private void MapDependencyInjection(IServiceCollection services)
		{
			services.AddScoped<ControllerTraceServiceFilter>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IPCBIContext, PCBIContext>();
			

			services.AddControllers()
				.AddControllersAsServices(); // Add the controllers to DI

		}

		public void ConfigureHubSignalRClient(IServiceCollection services)
		{
			services.AddSingleton<IHubChatSignalRRepository, HubChatSignalRRepository>();
			services.AddSingleton<IHubCommandSignalRRepository, HubCommandSignalRRepository>();
		}

	}

}

//app.UseHttpsRedirection();
//        app.UseStaticFiles();
//        app.UseAuthentication()


//======
//Handler-� �������� �������� ��� ����� AND, ��� � ����� OR.
//���, ��� ����������� ���������� ����������� AuthorizationHandler<FooRequirement>, ��� ��� ����� �������.
//	��� ���� ����� context.Succeed() �� �������� ������������,
//	� ����� context.Fail() �������� � ������ ������ � ����������� ��� ����������� �� ���������� ������ handler.
//	�����, �� ����� ������������� ����� ����� ������������� ��������� ������� ��������� �������:
//Policy: AND
//Requirement: AND
//Handler: AND / OR.
//services.AddAuthorization(options =>
//{
//     options.AddPolicy("BadgeEntry", policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim(c =>
//                (c.Type == "BadgeId" ||
//                 c.Type == "TemporaryBadgeId") &&
//                 c.Issuer == "https://microsoftsecurity")));
//});

//services.AddAuthorization(options =>
//{
//    options.AddPolicy("Over18", policy =>
//    {
//        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
//        policy.RequireAuthenticatedUser();
//        policy.Requirements.Add(new MinimumAgeRequirement());
//    });
//});
//==========
//public void ConfigureServices(IServiceCollection services)
//{
//	//�������� Identity
//	services.AddIdentity<IdentityUser, IdentityRole>();

//	//������������ ���������
//	services.AddTransient<IUserStore<IdentityUser>, FakeUserStore>();
//	services.AddTransient<IRoleStore<IdentityRole>, FakeRoleStore>();
//}

//	 ������� MVC ����� ��������� ����� �������, ����� ��� �������������� � ����������� �� ��������� ����� �������. � ������� ������������ ���� ���� ����������� ������ MVC � ������ SomeMvcFilter. 
//���� ������ ������������ � �������� ��������� MVC, ������ ���� ���� FeatureA �������.
//services.AddMvc(options => {
//        options.Filters.AddForFeature<SomeMvcFilter>(nameof(MyFeatureFlags.FeatureA));
//    });

//����� ����, ����� ������� ��������� ��������� ����� ���������� � �� �������������� ���� � ������ ���������� �������.
//� ������� ������������ ���� ���� ��������� �� �������������� ���� ����������� � �������� �������, ������ ���� ���� FeatureA �������.
//app.UseMiddlewareForFeature<ThirdPartyMiddleware>(nameof(MyFeatureFlags.FeatureA));

//	private static SqliteConnection SetupSqliteInMemoryConnection()
//	{
//		//var connectionStringBuilder = new SQLiteConnectionStringBuilder { DataSource = ":memory:" };
//		//var connectionString = connectionStringBuilder.ToString();
//		//SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
//		//{
//		//	DataSource = ":memory:",
//		//};
////		var connection = new SQLiteConnection("Data Source=:memory:;mode=memory;cache=shared");

//		//var connectionString = builder.ConnectionString + ";mode=memory;cache=shared";
//		var connection = new SqliteConnection("Data Source =:memory:");
//		connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
//		return connection;
//	}

//https://blog.stevensanderson.com/2019/09/13/blazor-inputfile/
//https://remibou.github.io/Upload-file-with-Blazor/

//admin@admin.com
//next123
// inv.wp-funnel.com