﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
 using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Count4U.Service.Common.Filter.ActionFilterFactory;
using Count4U.Service.Core.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Monitor.Service.Model;
using Monitor.Service.Urls;
using System.Collections.Generic;
using Monitor.Service.Shared;
using Count4U.Model.SelectionParams;

namespace Count4U.Service.WebAPI.Authentication.Controllers
{
 //   [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ServiceFilter(typeof(ControllerTraceServiceFilter))]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountsController> _logger;
        private readonly IEmailSender _emailSender;
        //private IUserValidator<ApplicationUser> userValidator;
        //private IPasswordValidator<ApplicationUser> passwordValidator;
        //private IPasswordHasher<ApplicationUser> passwordHasher;

        public AdminController(ILoggerFactory loggerFactory
            , IConfiguration configuration
            , UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender
            //, IUserValidator<ApplicationUser> userValid
            //, IPasswordValidator<ApplicationUser> passValid
            //, IPasswordHasher<ApplicationUser> passwordHash
            )
        {
            this._logger = loggerFactory.CreateLogger<AccountsController>();
            this._configuration = configuration ??
                              throw new ArgumentNullException(nameof(configuration));
            this._userManager = userManager ??
                              throw new ArgumentNullException(nameof(userManager));
            this._roleManager = roleManager ??
                            throw new ArgumentNullException(nameof(roleManager));
            this._signInManager = signInManager ??
                            throw new ArgumentNullException(nameof(signInManager));
			this._emailSender = emailSender ??
							  throw new ArgumentNullException(nameof(emailSender));
        }

        //public ViewResult Index() => View(_userManager.Users);



        [HttpPost(WebApiAuthenticationAdmin.Delete)]
        public async Task<DeleteResult> Delete([FromBody] DeleteModel deleteModel)
        {
            if (deleteModel == null)
            {
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = " DeleteModel is null " };
            }

             ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(deleteModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(deleteModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(deleteModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(deleteModel.Email);
                }
            }

			if (user == null)
			{
				return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = "Can't get user from db " };
			}

			IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(x => x.Description);
                var error = string.Join(" .", errors);
                return new DeleteResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
            }

            return new DeleteResult { Successful = SuccessfulEnum.Successful };
        }

        [HttpPost(WebApiAuthenticationAdmin.PostChangePassword)]
        public async Task<ChangePasswordResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            if (changePasswordModel == null)
            {
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = "changePasswordModel is null " };
            }
            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(changePasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(changePasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(changePasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(changePasswordModel.Email);
                }
            }

            if (user == null)
            {
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = "User not find " };
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(x => x.Description);
                var error = string.Join(" .", errors);
                return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            return new ChangePasswordResult { Successful = SuccessfulEnum.Successful };

        }

       	[HttpPost(WebApiAuthenticationAdmin.ForgotPassword)]
		public async Task<ForgotPasswordResult> ForgotPassword([FromBody]ForgotPasswordModel forgotPasswordModel)
		{
            if (forgotPasswordModel == null)
            {
               	return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error ="can't find user by email" };
            }

            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(forgotPasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(forgotPasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                }
            }

            if (user == null)
            {
                	return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error ="can't find user" };
            }
            try
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordModel.NewPassword) == true)
                    forgotPasswordModel.NewPassword = RandomGenerator.RandomString(6, true);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, forgotPasswordModel.NewPassword);
                if (result.Succeeded)
                {
                     string content = $"Hello {user.Email}, <br> Your password was changed to new : <br> <p> {user.Email} </p><p> {forgotPasswordModel.NewPassword} </p>";
           			var message = new EmailMessage(new string[] {user.Email }, "Password changed", content, null);
					await _emailSender.SendEmailAsync(message);
					return new ForgotPasswordResult { Successful = SuccessfulEnum.Successful, Token = token, Email = user.Email };
                }
                else
                {
                    string errorResult = "";
                    foreach (var error in result.Errors)
                    {
                       errorResult +=  error.Description + "; " ;
                    }
                    return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = errorResult};
                }
            }
            catch (Exception exc)
            {
                return new ForgotPasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = exc.Message };
            }

		}
        
        // ===================== User ========================
        [HttpGet(WebApiAuthenticationAdmin.GetUsers)]
        public async Task<List<UserViewModel>> GetUsers()
        {

            List<UserViewModel> result = new List<UserViewModel>();
            try
            {
                foreach (ApplicationUser user in this._userManager.Users)
                {
                    if (user != null)
                    {
                        result.Add(new UserViewModel { UserID = user.Id, Email = user.Email, Description = user.FistName, CustomerCode = user.CustomerCode });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }


        [HttpPost(WebApiAuthenticationAdmin.GetUsersWithSelectCustomerCode)]
        public async Task<List<UserViewModel>> GetUsersWithSelectCustomerCode([FromBody] string customerCode)
        {

            List<UserViewModel> result = new List<UserViewModel>();
            List<ApplicationUser> users = this._userManager.Users.Where(x => x.CustomerCode.ToLower().Contains(customerCode.ToLower())).Select(x => x).ToList();
            try
            {
                foreach (ApplicationUser user in users)
                {
                    if (user != null)
                    {
                        result.Add(new UserViewModel { UserID = user.Id, Email = user.Email, Description = user.FistName, CustomerCode = user.CustomerCode });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }


        [HttpPost(WebApiAuthenticationAdmin.GetUsersWithSelectEmail)]
        public async Task<List<UserViewModel>> GetUsersWithSelectEmail([FromBody] string email)
        {

            List<UserViewModel> result = new List<UserViewModel>();
            List<ApplicationUser> users = this._userManager.Users.Where(x => x.Email.ToLower().Contains(email.ToLower())).Select(x => x).ToList();
            try
            {
                foreach (ApplicationUser user in users)
                {
                    if (user != null)
                    {
                        result.Add(new UserViewModel { UserID = user.Id, Email = user.Email, Description = user.FistName, CustomerCode = user.CustomerCode });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }


        [HttpPost(WebApiAuthenticationAdmin.GetUser)]
        public async Task<UserViewModel> GetUser([FromBody] UserViewModel userViewModel)
        {
            if (userViewModel == null)
            {
                return new UserViewModel { Successful = SuccessfulEnum.NotSuccessful, Error = "userViewModel is null " };
            }

            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(userViewModel.UserID) == false)
            {
                user = await _userManager.FindByIdAsync(userViewModel.UserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(userViewModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(userViewModel.Email);
                }
            }

            if (user == null)
            {
                return new UserViewModel { Successful = SuccessfulEnum.UserNotFound, Error = "User not find " };
            }


            //var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
            //if (result.Succeeded == false)
            //{
            //    var errors = result.Errors.Select(x => x.Description);
            //    var error = string.Join(" .", errors);
            //    return new ChangePasswordResult { Successful = SuccessfulEnum.NotSuccessful, Error = error };
            //}

            //await _signInManager.SignInAsync(user, isPersistent: false);
            //_logger.LogInformation("User changed their password successfully.");
            //return new ChangePasswordResult { Successful = SuccessfulEnum.Successful };


           UserViewModel result = new UserViewModel();
            result.UserID = user.Id;
            result.Email = user.Email;
            result.CustomerCode = user.CustomerCode != null ? user.CustomerCode : "";
            result.Description = user.FistName != null ? user.FistName : "";
            result.Error = "";
            result.Message = "";
            result.Successful = SuccessfulEnum.Successful;
            result.DateCreated = user.DateCreated;

            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            result.InRoles = new List<string>();
            //роли пользователя
            foreach (string role in roles)
            {
                result.InRoles.Add(role);
                if (role == "Owner") result.IsOwner = true;
                else if (role == "Worker") result.IsWorker = true;
                else if (role == "Manager") result.IsManager = true;
            }
            return result;
        }


   
        [HttpPost(WebApiAuthenticationAdmin.GetUserWithPassword)]
        public async Task<UserWithPasswordModel> GetUserWithPassword([FromBody] UserWithPasswordModel userWithPasswordModel)
        {

             if (userWithPasswordModel == null)
            {
                return userWithPasswordModel;
            }
            ApplicationUser user = null;
            if (string.IsNullOrWhiteSpace(userWithPasswordModel.ApplicationUserID) == false)
            {
                user = await _userManager.FindByIdAsync(userWithPasswordModel.ApplicationUserID);           //try by ID first
            }

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(userWithPasswordModel.Email) == false)
                {
                    user = await _userManager.FindByEmailAsync(userWithPasswordModel.Email);
                }
            }

            if (user == null)
            {
                return userWithPasswordModel;
            }

            return userWithPasswordModel;
        }

        // ==================  Role   ========================
        [HttpGet(WebApiAuthenticationAdmin.GetRoles)]
        public async Task<List<RoleModel>> GetRoles()
        {
            List<RoleModel> result = new List<RoleModel>();
            try
            {
                foreach (IdentityRole role in _roleManager.Roles)
                {
                    if (role != null)
                    {
                        List<UserViewModel> members = new List<UserViewModel>();
                        foreach (ApplicationUser user in _userManager.Users)
                        {
                            if (user != null)
                            {
                                if (await _userManager.IsInRoleAsync(user, role.Name))
                                {
                                    members.Add(new UserViewModel { UserID = user.Id, Email = user.Email, Description = user.FistName, CustomerCode = user.CustomerCode });
                                }
                            }
                        }
                        result.Add(new RoleModel { RoleID = role.Id, RoleName = role.Name, Members = members });
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }



        //Заполняем форму редактирования 
        [HttpPost(WebApiAuthenticationAdmin.RoleWithUsers)]
        public async Task<RoleModel> RoleWithUsers([FromBody] string roleId)
        {
            RoleModel result = new RoleModel();
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(roleId);
                List<UserViewModel> members = new List<UserViewModel>();
                List<UserViewModel> nonMembers = new List<UserViewModel>();
                foreach (ApplicationUser user in _userManager.Users)
                {
                    var list = await _userManager.IsInRoleAsync(user, role.Name)
                        ? members : nonMembers;
                    list.Add(new UserViewModel { UserID = user.Id, Email = user.Email, Description = user.FistName, CustomerCode = user.CustomerCode });
                }

                result = new RoleModel
                {
                    RoleID = roleId,
                    RoleName = role.Name,
                    Members = members,
                    NonMembers = nonMembers
                };
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }


        //[HttpPost(WebApiAuthenticationAdmin.UserWithRoles)]
        //public async Task<UserViewModel> UserWithRoles([FromBody] ApplicationUser user)
        //{

        //    IList<string> rolseIst = await _userManager.GetRolesAsync(user);
 
        //}


        [HttpPost(WebApiAuthenticationAdmin.UpdateUsersInRole)]
        public async Task<RoleResult> UpdateUsersInRole([FromBody] RoleModel roleModel)
        {
            List<RoleModificationResult>  resultError = new List<RoleModificationResult>();
        	if (roleModel == null)
			{
				return new RoleResult { Successful = SuccessfulEnum.NotSuccessful, Error = "RoleModel is null " };
			}

            IdentityResult result;
            //IdsToAdd из UI
            //foreach (string userId in model.IdsToAdd ?? new string[] { })
            foreach (UserViewModel userMember in roleModel.NonMembers)
            {
                if (userMember.ToAdd == true)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userMember.UserID);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, roleModel.RoleName);
                        if (!result.Succeeded)
                        {
                            resultError.Add(AddErrorsFromResult(result, roleModel.RoleID, userMember.UserID));
                        }
                    }
                }
            }
            //IdsToDelete из UI
            // foreach (string userId in model.IdsToDelete ?? new string[] { })
            foreach (UserViewModel userMember in roleModel.Members)
            {
                if (userMember.ToDelete == true)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userMember.UserID);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, roleModel.RoleName);
                        if (!result.Succeeded)
                        {
                            resultError.Add(AddErrorsFromResult(result, roleModel.RoleID, userMember.UserID));
                        }
                    }
                }
            }


            if (resultError.Count == 0)
            {
                return new RoleResult { Successful = SuccessfulEnum.Successful };
            }
            else
            {
                return new RoleResult { Successful = SuccessfulEnum.Successful };
            }
        }

        [NonAction]
        private RoleModificationResult AddErrorsFromResult(IdentityResult result, string roleId, string userId)
        {
            var errors = result.Errors.Select(x => x.Description);
            var error = string.Join(" .", errors);
            return new RoleModificationResult { Successful = SuccessfulEnum.NotSuccessful, Error = error, RoleID = roleId, ApplicationUserID = userId };

        }
    }
}
