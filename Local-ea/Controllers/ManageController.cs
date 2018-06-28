using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project_2_Common.Models;
using Project_2_Common.Models.ManageViewModels;
using Project_2_Common.Services;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Models.MapViewModels;

namespace Project_2_Common.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IStoreRatings _ratingstore;
        

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        private readonly IHostingEnvironment he;  
        
        public ManageController(
            IStoreRatings ratingstore,
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IHostingEnvironment ihostingEnvironment)
        {
            _ratingstore = ratingstore;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            he = ihostingEnvironment;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
				Id=user.Id,
                UserNameStr = user.UserNameStr,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage,
                showMeOnMap = user.showMeOnMap,
                lat = user.lat,
                lon = user.lon,
                //File = user.File
                Avatar = user.Avatar,
                AboutMe = user.Description
            };
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }
            //var setEmailResult = await _userManager.SetEmailAsync(user, model.File);
            //if (ModelState.IsValid)
            //{
            //    var file = (user.File);
            //    var parsedContentDisposition =
            //        Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            //    var filename = Path.Combine(_hostingEnvironment.WebRootPath,
            //        "\\img\\avatars\\", parsedContentDisposition.FileName.ToString());  // .Trim('"'));
            //    using (var stream = System.IO.File.OpenWrite(filename))
            //    {
            //        await file.CopyToAsync(stream);
            //    }
            //}
            string AvatarSource = null;

            if (model.File != null)
            {
                AvatarSource = user.Id + "_Avatar_" + model.File.FileName;

				var fileName = Path.Combine(he.WebRootPath, "img","Avatars", AvatarSource);
				model.File.CopyTo(new FileStream(fileName, FileMode.Create));
                ViewData["fileLocation"] = fileName;
            }

            var UserNameStr = user.UserNameStr;
            //var Avatar = user.Avatar;
			var showMe = user.showMeOnMap;
			var lat = user.lat;
            var lon = user.lon;
			var AboutMe = user.Description;
			if (model.UserNameStr != UserNameStr || model.showMeOnMap!=showMe || model.lat != lat || model.lon!= lon || model.AboutMe != AboutMe)
            {
				user.showMeOnMap = model.showMeOnMap;
				user.lat = model.lat;
				user.lon = model.lon;
				user.Description = model.AboutMe;
				user.UserNameStr = model.UserNameStr;
                
				var updateUserInfoResult = await _userManager.UpdateAsync(user);
				//var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserNameStr);
				if (!updateUserInfoResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred updating users info for user with ID '{user.Id}'.");
                }
            }
            
            if (!string.IsNullOrWhiteSpace(AvatarSource) && AvatarSource != user.Avatar)
            {
                user.Avatar = AvatarSource;
                var updateUserInfoResult = await _userManager.UpdateAsync(user);
                //var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserNameStr);
                if (!updateUserInfoResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred updating Avatar image for user with ID '{user.Id}'.");
                }
            }



			StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

		public async Task<IActionResult> LocaleActivate(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			await _userManager.AddToRoleAsync(user, "Locale");

			return RedirectToAction(nameof(Index)); 
		}

		public async Task<IActionResult> LocaleDeactivate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
			user.lat = "0";
			user.lon = "0";
			user.showMeOnMap = false;
			await _userManager.UpdateAsync(user);
            await _userManager.RemoveFromRoleAsync(user, "Locale");

            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public ActionResult UploadFile()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<ActionResult> UploadFileAsync(IFormFile file)
        //{
        //    try
        //    {
        //        FileDetails fileDetails;
        //        using (var reader = new StreamReader(file.OpenReadStream()))
        //        {
        //            var fileContent = reader.ReadToEnd();
        //            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
        //            var fileName = parsedContentDisposition.FileName;
        //        }

        //        if (fileName.EndsWith(".txt"))
        //        {
        //            var filePath = IHostingEnvironment.ApplicationBasePath + "\\wwwroot\\img\\Avatars\\" + fileName;
        //            await file.SaveAsAsync(filePath);
        //        }

        //        ViewBag.Message = "File Uploaded Successfully!!";
        //        return View();
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "File upload failed!!";
        //        return View();
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        public async Task<IActionResult> VisitorView(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
			{
				return View("StatusCode/Index");
			}
			var model = new IndexViewModel
            {
				Id = user.Id,
                UserNameStr = user.UserNameStr,
                Email = user.Email, 
                showMeOnMap = user.showMeOnMap,
                lat = user.lat,
                lon = user.lon,
                //File = user.File
                Avatar = user.Avatar,
				AboutMe = user.Description,
                ReceivedRatings = user.ReceivedRatings
                
            };

            return View(model);
        }
        public async Task<IActionResult> VisitorRatingsView(string id)
        {
            var visitorRatings = await _ratingstore.VisitorWindowsHasRatings(id);
            if (visitorRatings.Count() == 0)
            {
                return PartialView("_NoRatingsVisitor");

            }
            else
            {
                return View("_ReviewsView", visitorRatings);
            }
        }

        public async Task<IActionResult> ReviewsView()
        {
            var meId = _userManager.GetUserId(HttpContext.User);
           
            var myratings = await _ratingstore.ReadmyRatings(meId);
            if (myratings.Count()==0)
            {
                return PartialView("_NoRatings");

            }
            else
            {
                return View("_ReviewsView",myratings);
            }

        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Project_2_Common"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
        #endregion
    }
}
