﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Count4U.Admin.Client.Blazor.I18nText;
using Count4U.Service.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Monitor.Service.Model;

namespace Count4U.Admin.Client.Page
{
	public class ChangePasswordBase : ComponentBase
	{
		protected ChangePasswordModel _changePasswordModel { get; set; }
		protected ChangePasswordResult _changePasswordResult { get; set; }
		//protected bool? _showErrors;

		protected RegisterResult _registerResult { get; set; }

		//protected bool? _showSuccessful;
		//protected List<string> _errors { get; set; }

		protected GetResources LocalizationResources;

		[Inject]
		protected NavigationManager _navigationManager { get; set; }

		[Inject]
		protected IAuthService _authService { get; set; }

		[Inject]
		protected IAdminService _adminService { get; set; }


		[Inject]
		protected HttpClient Http { get; set; }

		[Inject]
		protected Toolbelt.Blazor.I18nText.I18nText I18nText { get; set; }
		
		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }



		public ChangePasswordBase()
		{
			this._changePasswordModel = new ChangePasswordModel();
			//this._errors = new List<string>();
		}



		//Todo
		protected async Task ChangePasswordAsync()
		{
			Console.WriteLine();
			Console.WriteLine($"Client.ChangePasswordBase.ChangePasswordAsync() : start");

			this._changePasswordResult = null;

			if (this._authService == null)
			{
				Console.WriteLine($"Client.ChangePasswordBase.ChangePasswordAsync() : _authService is null");
			}
			else
			{
				try
				{
					this._changePasswordResult = await this._adminService.ChangePasswordAsync(this._changePasswordModel);            //TODO

					if (this._changePasswordResult != null)
					{
						if (this._changePasswordResult.Successful == SuccessfulEnum.Successful)
						{
							Console.WriteLine($"Client.ChangePasswordBase.ChangePasswordAsync() : Successful");
							//this._showSuccessful = true;
							await this._authService.LogoutAaync();
							this._navigationManager.NavigateTo("/");
						}
						else
						{
							Console.WriteLine($"Client.ChangePasswordBase.ChangePasswordAsync() : Errors");
							Console.WriteLine($"{this._changePasswordResult.Error}");
							//this._errors.Add(result.Error);
							//this._showErrors = true;
						}
					}
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.ChangePasswordBase.RegistrationAsync() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.ChangePasswordBase.RegistrationAsync() : end");
			}
		}

		protected async Task<string> LogUsername()
		{
			Console.WriteLine();
			Console.WriteLine($"Client.InfoContextBase.LogUsername() : start");
			if (this.AuthenticationStateProvider == null)
			{
				Console.WriteLine($"Client.InfoContextBase.LogUsername() : AuthenticationStateProvider is null");
			}
			else
			{
				try
				{
					var authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
					if (authState != null)
					{
						var user = authState.User;
						if (user != null)
						{
							if (user.Identity != null)
							{
								if (user.Identity.IsAuthenticated)
								{
									Console.WriteLine($"Client.InfoContextBase.LogUsername() : {user.Identity.Name} is authenticated.");
									Console.WriteLine($"Client.InfoContextBase.LogUsername() : end");
									//		_logger.LogInformation($"{user.Identity.Name} is authenticated.");
									return user.Identity.Name;
								}
							}
						}
					}
					Console.WriteLine("Client.InfoContextBase.LogUsername() : the user is NOT authenticated.");
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.InfoContextBase.LogUsername() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.InfoContextBase.LogUsername() : end");
			}
			return "";
		}
		protected override async Task OnInitializedAsync()
		{
			Console.WriteLine();
			Console.WriteLine($"Client.ChangePasswordBase.OnInitializedAsync() : start");
			try
			{
				this.LocalizationResources = await this.I18nText.GetTextTableAsync<GetResources>(this);
				this._changePasswordModel.Email = await LogUsername();
				//var authstate = await _authenticationStateProvider.GetAuthenticationStateAsync();
				//var user = authstate.User;
				//var name = user.Identity.Name;
				//this._changePasswordModel.Email = authstate.User.Identity.Name ;
			}
			catch (Exception ecx)
			{
				Console.WriteLine("Client.ChangePasswordBase.OnInitializedAsync() Exception : ");
				Console.WriteLine(ecx.Message);
			}
			Console.WriteLine($"Client.ChangePasswordBase.OnInitializedAsync() : end");
		}
	}
}

