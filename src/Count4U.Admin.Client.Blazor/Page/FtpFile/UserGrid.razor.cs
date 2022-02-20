﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Count4U.Model.Count4U;
using Count4U.Service.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Monitor.Service.Model;
using Monitor.Service.Urls;
using Count4U.Admin.Client.Blazor.I18nText;
using Count4U.Model.SelectionParams;

namespace Count4U.Admin.Client.Page
{
	public class UserGridBase : ComponentBase
	{
		[Parameter]
		public string customerCode { get; set; }
		protected List<UserViewModel> _users { get; set; }
		//protected string _userID { get; set; } = "";
		public string PingServer { get; set; }
		public string SessionStorageMode { get; set; }
		public string SessionAuthenticationServer { get; set; }
	  	protected GetResources LocalizationResources { get; set; }

		protected FilterModel _filterUserModel { get; set; }
		protected FilterResult _filterResult { get; set; }

		[Inject]
		protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

		[Inject]
		protected IProfileModel _profileModel { get; set; }

		[Inject]
		protected ISessionStorageService _sessionStorage { get; set; }

		[Inject]
		protected ILocalStorageService _localStorage { get; set; }

		[Inject]
		protected IJwtService _jwtService { get; set; }

		[Inject]
		protected IAdminService _adminService { get; set; }

		//[Inject]
		//protected HttpClient Http { get; set; }

		[Inject]
		protected NavigationManager _navigationManager { get; set; }

		[Inject]
		protected Toolbelt.Blazor.I18nText.I18nText I18nText { get; set; }

		protected bool IsSearching { get; set; }

		public int OnPageNumber { get; set; }
		public UserGridBase()
		{
			this._users = null;
			this._filterUserModel = new FilterModel();
			this._filterUserModel.InitUserFilter();
		}

		protected async Task GetUsers()
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.GetUsers() : start");

			if (this._adminService == null)
			{
				Console.WriteLine($"Client.UserGridBase.GetUsers() : _adminService is null");
				this._users = new List<UserViewModel>();
			}
			else
			{
				try
				{
					this._users = await this._adminService.GetUsers();
					}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.UserGridBase.GetUsers() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.UserGridBase.GetUsers() : end");
			}
		}


		protected async Task GetUsersWithCustomerCode(string customerCode)
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.GetUsersWithCustomerCode() : start");

			if (this._adminService == null)
			{
				Console.WriteLine($"Client.UserGridBase.GetUsersWithCustomerCode() : _adminService is null");
				this._users = new List<UserViewModel>();
			}
			else
			{
				try
				{
					this._users = await this._adminService.GetUsersWithSelectCustomerCode(customerCode);
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.UserGridBase.GetUsersWithCustomerCode() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.UserGridBase.GetUsersWithCustomerCode() : end");
			}
		}

		protected async Task GetUsersWithEmail(string email)
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.GetUsersWithEmail() : start");

			if (this._adminService == null)
			{
				Console.WriteLine($"Client.UserGridBase.GetUsersWithEmail() : _adminService is null");
				this._users = new List<UserViewModel>();
			}
			else
			{
				try
				{
					this._users = await this._adminService.GetUsersWithSelectEmail(email);
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.UserGridBase.GetUsersWithEmail() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.UserGridBase.GetUsersWithEmail() : end");
			}
		}

		//https://blazor-university.com/javascript-interop/calling-javascript-from-dotnet/ - dialog
		//https://chrissainty.com/using-javascript-interop-in-razor-components-and-blazor/ JS
		public async Task UserDelete(string Email)
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.UserDelete() : start");

			if (this._adminService == null)
			{
				Console.WriteLine($"Client.UserGridBase.UserDelete() : _adminService is null");
			}
			else
			{
				try
				{
					var result = await this._adminService.Delete(new DeleteModel() { Email = Email });
					this._users = await this._adminService.GetUsers();
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.UserGridBase.UserDelete() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.UserGridBase.UserDelete() : end");
				//this.StateHasChanged();
			}
		}

		public async Task UserEdit(string email)
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.UserEdit() : start");

			if (this._adminService == null)
			{
				Console.WriteLine($"Client.UserGridBase.UserEdit() : _adminService is null");
			}
			else
			{
				try
				{
					this._navigationManager.NavigateTo("/useredit/" + email);
				}
				catch (Exception ecx)
				{
					Console.WriteLine("Client.UserGridBase.UserEdit() Exception : ");
					Console.WriteLine(ecx.Message);
				}
				Console.WriteLine($"Client.UserGridBase.UserEdit() : end");
				//this.StateHasChanged();
			}
		}


	
		public async Task UserChangePassword(string email)
		{
            this._navigationManager.NavigateTo("/userchangepassword/" + email);
		}

		public async Task UserAdd()
		{
			
			if (this._filterUserModel.FilterSelectByField == FilterUserSelectParam.CustomerCode
				&& string.IsNullOrWhiteSpace(this._filterUserModel.FilterValue)== false)
			{
				this._navigationManager.NavigateTo("/useradd/" + this._filterUserModel.FilterValue);
			}
			else
			{
				this._navigationManager.NavigateTo("/useradd");
			}
		}

		protected async Task SetAuthenticationServer()
		{
			try
			{
				if (this._localStorage != null)
				{
					string authenticationWebapiUrl = await this._localStorage.GetItemAsync<string>(SessionStorageKey.authenticationWebapiUrl);
					this.SessionAuthenticationServer = authenticationWebapiUrl;
				}
			}
			catch (Exception ecx)
			{
				Console.WriteLine("Client.UserGridBase.SetAuthenticationServer() Exception : ");
				Console.WriteLine(ecx.Message);
			}
		}

		public async Task OnSearchAsync()
		{
			IsSearching = true;
			StateHasChanged();
			Console.WriteLine("Client.UserGridBase.OnSearchAsync() start ");
			this._filterResult = new FilterResult();
			try
			{
				if (string.IsNullOrWhiteSpace(this._filterUserModel.FilterValue) == false)
				{
					Console.WriteLine($"Client.UserGridBase.OnSearchAsync() FilterValue : {this._filterUserModel.FilterValue}");
					Console.WriteLine($"Client.UserGridBase.OnSearchAsync() FilterSelectByField : {this._filterUserModel.FilterSelectByField}");
					await this._localStorage.SetItemAsync(SessionStorageKey.filterUser, _filterUserModel.FilterSelectByField);
					await this._localStorage.SetItemAsync(SessionStorageKey.filterValueUser, _filterUserModel.FilterValue);

					//if (this._filterUserModel.FilterSelectByField == FilterUserSelectParam.All)
					//{
					//	await GetUsers();
					//}
					//else 
					if (this._filterUserModel.FilterSelectByField == FilterUserSelectParam.CustomerCode)
					{
						await GetUsersWithCustomerCode(this._filterUserModel.FilterValue);
					}
					else if (this._filterUserModel.FilterSelectByField == FilterUserSelectParam.Email)
					{
						await GetUsersWithEmail(this._filterUserModel.FilterValue);
					}
					else
					{
						await GetUsers();
					}

					
					this._filterResult.Successful = SuccessfulEnum.Successful;
				}
				else
				{
					//await this._localStorage.SetItemAsync(SessionStorageKey.filterUser, "");
					await GetUsers();
					this._filterResult.Successful = SuccessfulEnum.Successful;
				}
			}
			catch (Exception exc)
			{
				this._filterResult.Successful = SuccessfulEnum.NotSuccessful;
				this._filterResult.Error = exc.Message;
			}
			IsSearching = false;
			StateHasChanged();
		}

		public async Task OnClearAsync()
		{
			this._filterUserModel.Clear();
			Console.WriteLine($"Client.UserGridBase.OnClearAsync() : start");
			await this._localStorage.SetItemAsync(SessionStorageKey.filterUser, "");
			await this._localStorage.SetItemAsync(SessionStorageKey.filterValueUser, "");
			await OnSearchAsync();
		}

		public async Task OnChangePageNumber(ChangeEventArgs args)
		{
			Console.WriteLine($"OnChangePageNumber: {args.Value}");
			string perPage = args.Value as string;
			await OnChangePageSet(perPage);
		}
		//public async Task OnChangePageNumber()
		//{
		//	Console.WriteLine($"OnChangePageNumber: {OnPageNumber}");
		//	//string perPage = args.Value as string;
		//	await OnChangePageSet(OnPageNumber.ToString());
		//}

		public async Task OnChangePageSet(string perPage)
		{
			int perPageInt = 15;
			this.OnPageNumber = 15;
			try
			{
				bool ret = int.TryParse(perPage, out perPageInt);
				Console.WriteLine($"Client.CustomerProfileGridBase.OnChangePageSet() : try perPageInt to  {perPageInt}");
				this.OnPageNumber = perPageInt;
				Console.WriteLine($"Client.CustomerProfileGridBase.OnChangePageSet() : OnPageNumber to  {OnPageNumber}");
			}
			catch { }
			if (this._localStorage != null)
			{
				await this._localStorage.SetItemAsync(SessionStorageKey.onPageUserNumber, this.OnPageNumber);
			}
		}
		protected override async Task OnInitializedAsync()
		{
			Console.WriteLine();
			Console.WriteLine($"Client.UserGridBase.OnInitializedAsync() : start");
			try
			{
				if (string.IsNullOrWhiteSpace(customerCode) == false)
				{
					this._filterUserModel.FilterSelectByField = FilterUserSelectParam.CustomerCode;
					this._filterUserModel.FilterValue = customerCode;
					await this._localStorage.SetItemAsync(SessionStorageKey.filterUser, _filterUserModel.FilterSelectByField);
					await this._localStorage.SetItemAsync(SessionStorageKey.filterValueUser, _filterUserModel.FilterValue);
				}

				this.LocalizationResources = await this.I18nText.GetTextTableAsync<GetResources>(this);


				if (this._localStorage != null)
				{
					string perPageString = await this._localStorage.GetItemAsync<string>(SessionStorageKey.onPageUserNumber);
					int perPageInt = 15;
					this.OnPageNumber = 15;
					try
					{
						bool ret = int.TryParse(perPageString, out perPageInt);
						Console.WriteLine($"Client.UserEditBase.OnInitializedAsync() : try perPageInt to  {perPageInt}");
						this.OnPageNumber = perPageInt;
					}
					catch { }
					Console.WriteLine($"Client.UserEditBase.OnInitializedAsync() : GetonPageNumber {this.OnPageNumber}");

					var filterUser = await this._localStorage.GetItemAsync<string>(SessionStorageKey.filterUser);
					this._filterUserModel.FilterSelectByField = filterUser != null ? filterUser : "";
					var filterValueUser = await this._localStorage.GetItemAsync<string>(SessionStorageKey.filterValueUser);
					this._filterUserModel.FilterValue = filterValueUser != null ? filterValueUser : "";

					Console.WriteLine($"Client.UserEditBase.OnInitializedAsync() : FilterSelectByField {this._filterUserModel.FilterSelectByField}");
					await this.OnSearchAsync();
				}

				Console.WriteLine($"Client.UserGridBase.OnInitializedAsync() : GetAuthenticationUrls");
			}
			catch (Exception exc)
			{
				Console.WriteLine($"Client.UserGridBase.OnInitializedAsync() Exception :");
				Console.WriteLine(exc.Message);
			}
			Console.WriteLine($"Client.UserGridBase.OnInitializedAsync() : end");
		}


		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				Console.WriteLine();
				Console.WriteLine($"Client.UserGridBase.OnAfterRenderAsync() : start");
				try
				{
					if (this._sessionStorage != null)
					{
						this.SessionStorageMode = await this._sessionStorage.GetItemAsync<string>(SessionStorageKey.hostMode);
						this.SessionAuthenticationServer = await this._localStorage.GetItemAsync<string>(SessionStorageKey.authenticationWebapiUrl);

						await this.SetAuthenticationServer();
						Console.WriteLine($"Client.UserGridBase.OnAfterRenderAsync() : got AuthenticationServer Adress");
					}

					//if (this._localStorage != null)
					//{
					//	string perPageString = await this._localStorage.GetItemAsync<string>(SessionStorageKey.onPageUserNumber);
					//	int perPageInt = 15;
					//	this.OnPageNumber = 15;
					//	try
					//	{
					//		bool ret = int.TryParse(perPageString, out perPageInt);
					//		Console.WriteLine($"Client.UserEditBase.OnAfterRenderAsync() : try perPageInt to  {perPageInt}");
					//		this.OnPageNumber = perPageInt;
					//	}
					//	catch { }
					//	Console.WriteLine($"Client.UserEditBase.OnAfterRenderAsync() : GetonPageNumber {this.OnPageNumber}");

					//	this._filterUserModel.FilterSelectByField = await this._localStorage.GetItemAsync<string>(SessionStorageKey.filterUser);
					//	this._filterUserModel.FilterValue = await this._localStorage.GetItemAsync<string>(SessionStorageKey.filterValueUser);

					//	Console.WriteLine($"Client.UserEditBase.OnAfterRenderAsync() : FilterSelectByField {this._filterUserModel.FilterSelectByField}");
					//	await this.OnSearchAsync();
					//}

					Console.WriteLine($"Client.UserGridBase.OnAfterRenderAsync() : GetAuthenticationUrls");
				}
				catch (Exception exc)
				{
					Console.WriteLine($"Client.UserGridBase.OnAfterRenderAsync() Exception :");
					Console.WriteLine(exc.Message);
				}
				Console.WriteLine($"Client.UserGridBase.OnAfterRenderAsync() : end");
			}
		}



				//public async Task AuthenticationServerClicked()
				//{
				//	try
				//	{
				//		if (this._localStorage != null)
				//		{
				//			await this._localStorage.SetItemAsync(SessionStorageKey.authenticationWebapiUrl, this.SessionAuthenticationServer);
				//			this._navigationManager.NavigateTo("/Logout");
				//		}
				//	}
				//	catch (Exception ecx)
				//	{
				//		Console.WriteLine("Client.RoleGridBase.AuthenticationServerClicked() Exception : ");
				//		Console.WriteLine(ecx.Message);
				//	}

				//}
			}
}

