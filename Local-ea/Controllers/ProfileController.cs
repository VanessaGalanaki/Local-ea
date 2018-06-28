using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Project_2_Common.Models;
using Project_2_Common.Models.ManageViewModels;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Models.MapViewModels;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_2_Common.Controllers
{
    public class ProfileController : Controller
    {
		private readonly UserManager<ApplicationUser> _manager;
        private readonly IStoreRatings _ratingstore;
		private readonly IStoreMessages _messagesStore;
		private readonly IUserRepo _userRepo;
        
        public ProfileController(UserManager<ApplicationUser> manager,IStoreMessages messagesStore, IStoreRatings ratingstore, IUserRepo userRepo)
        {
            _ratingstore = ratingstore;
			_manager = manager;
			_messagesStore = messagesStore;
			_userRepo = userRepo;         
		}
        
        
        public async Task<IActionResult> ViewUser(string id)
        {
			var user = await _manager.FindByIdAsync(id);
			var model = new IndexViewModel
            {
                UserNameStr = user.UserNameStr,
                Email = user.Email,
 
                showMeOnMap = user.showMeOnMap,
                lat = user.lat,
                lon = user.lon,
                AboutMe = user.Description,
                //File = user.File
                Avatar = user.Avatar
            };
			
			return View(model);
        }

        public async Task<IActionResult> ReviewsView(string meId)
        {
            var myratings = await _ratingstore.ReadmyRatings(meId);
            if (myratings==null)
            {
                return PartialView("NoRatingsView");

            }
            else
            {
                return PartialView("_ReviewsView");
            }

        }

        [Authorize(Roles ="Admin,God")]
        public async Task<IActionResult> AdminView()
        {
			var god = await _manager.FindByEmailAsync("god@gmail.com");
			var users = _manager.Users.ToList();
			users.Remove(god);
            return View(users);
        }

		[Authorize(Roles = "God")]
        public async  Task<IActionResult> GodMessageView(string id)
        {
			var selectedUser = await _manager.FindByIdAsync(id);

			var buddiesIDs = _messagesStore.myChatBuddies(id);
			var buddiesAU = _manager.Users.Where(x => buddiesIDs.Contains(x.Id)).ToList();
            var buddies = new List<MapUserModel>();
            foreach (var u in buddiesAU)
            {
                buddies.Add(
                    new MapUserModel
                    {
                        Id = u.Id,
                        UserNameStr = u.UserNameStr,
                        Avatar = (u.Avatar == null ? "Avatar_Default1.jpg" : u.Avatar)

                    });
            }

            var mapUser = new MapUserModel
            {
				Id = selectedUser.Id,
				UserNameStr = selectedUser.UserNameStr,
                MyChatBuddies = buddies
            };
			return View(mapUser);
        }


        [Authorize(Roles ="Admin")]
		public async Task<IActionResult> UpDown(string Id)
		{
			var user = await _manager.FindByIdAsync(Id);

			if (await _manager.IsInRoleAsync(user, "Admin")){
				await _manager.RemoveFromRoleAsync(user, "Admin");
			}
			else{
				await _manager.AddToRoleAsync(user, "Admin");
			}

			var me = await _manager.GetUserAsync(User);


            if (me.Id == Id)
			{
				return RedirectToAction("Index","Manage");
			}
			return RedirectToAction(nameof(AdminView));
		}

		[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _manager.FindByIdAsync(id);
			var me = await _manager.GetUserAsync(User);
            
			await _messagesStore.DeleteMessagesFromOrBy(id);
			await _ratingstore.DeleteRatingsReceivedBy(id);

			await _userRepo.DeleteUser(user);         

			if (me.Id == id)
            {
				foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

				return RedirectToAction("Login", "Account");
            }
			return RedirectToAction(nameof(AdminView));
		}
    }
}
