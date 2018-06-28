using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Project_2_Common.Models;
using Project_2_Common.Models.ChatviewModels;
using Project_2_Common.Models.MapViewModels;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Data.Repositories;
using Project_2_Common.Data;

namespace Project_2_Common.Controllers
{
    public class MapPageController : Controller
    {
		private readonly UserManager<ApplicationUser> _manager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IStoreMessages _messageStore;
		private readonly IStoreRatings _ratingstore;
		private readonly IUserRepo _userRepo;
      

        public MapPageController( UserManager<ApplicationUser> manager,SignInManager<ApplicationUser> signInManager, IStoreMessages store, IStoreRatings ratingstore, IUserRepo userRepo)
		{
			_manager = manager;
			_signInManager = signInManager;
            
			_ratingstore= ratingstore;
			_messageStore = store;

			_userRepo = userRepo;
		}

        public async Task<IActionResult> Index()
        {
			         
			if (!_signInManager.IsSignedIn(User))
			{
				return View();
			}


			var user = await _manager.GetUserAsync(User);

            var buddiesIDs = _messageStore.myChatBuddies(user.Id);         
			var buddiesAU = _manager.Users.Where(x => buddiesIDs.Contains(x.Id)).ToList();
			var buddies = new List<MapUserModel>();
			foreach (var u in buddiesAU)
			{
				buddies.Add(
					new MapUserModel{
					    Id = u.Id,
                        UserNameStr = u.UserNameStr,
                        Avatar = (u.Avatar == null? "Avatar_Default1.jpg" : u.Avatar)
                        
				});
			}

			var mapUser = new MapUserModel
			{
				Id = user.Id,
				UserNameStr = user.UserNameStr,
				MyChatBuddies = buddies
			};

			return View(mapUser);
        }

		// GET : /MapPage/GetHistoryBetweenAsync
        [HttpPost]
		public async Task<JsonResult> GetHistoryBetweenAsync(string id1, string id2)
        {
			var messages = await _messageStore.GetMessagesBetween(id1, id2);
			List<MessageViewModel> messagesModel = new List<MessageViewModel>();
			foreach (var item in messages)
			{
				messagesModel.Add(
					new MessageViewModel{
					MessageId = item.MessageId,
					SenderId = item.SenderId,
					SenderName = item.Sender.UserNameStr,
					ReceiverId = item.ReceiverId,
					ReceiverName = item.Receiver.UserNameStr,
					Body = item.Body,
					Date = item.DateSent,
					hasBeenRead = item.Read
				});
			}         
			return Json(messagesModel);
		}

		// POST : /MapPage/SearchThisArea 
        [HttpPost]
		public JsonResult SearchThisArea(string latTopRight, string lonTopRight, string latBotLeft, string lonBotLeft) // (MapFrame mapFrame) //
        {   
            MapFrame mapFrame = new MapFrame
            {
				latTopRight = latTopRight,
				lonTopRight = lonTopRight,
				latBotLeft = latBotLeft,
				lonBotLeft = lonBotLeft
            };



			var users = _userRepo.GetLocalsInFrame(mapFrame);
			//var users = _manager.Users.Where(x => (x.InRange(mapFrame) && x.showMeOnMap)).ToList(); 
			var result = new List<LocaleeModel>();

            foreach (var u in users)
			{
				result.Add(new LocaleeModel
				{
					Id = u.Id,
                    UserNameStr = u.UserNameStr,
                    lat = u.lat,
                    lon = u.lon,
                    Avatar = (u.Avatar == null ? "Avatar_Default1.jpg" : u.Avatar),
					ReceivedRatingsCount = u.ReceivedRatings.Count,
					OverallRating = Math.Round((u.ReceivedRatings.Count!=0? u.ReceivedRatings.Average(r=>r.RatingValue):0),2)
                   


                });
                
			}
            return Json(result.OrderByDescending(x => x.OverallRating).ThenByDescending(i=>i.ReceivedRatingsCount));
        }

        public IActionResult Message()
        {
            //get all messages for user id with sender id
            //check which messages are not read yet
            //then append them to the view
		// POST : /MapPage/GetUserInfo 
			return View();
        }
        
		[HttpPost]
        public async Task<JsonResult> GetUserInfo(string id)
		{
			var user = await _manager.FindByIdAsync(id);
			MapUserModel userModel = new MapUserModel { 
				Id = user.Id,
                UserNameStr = user.UserNameStr,
                Avatar = (user.Avatar == null ? "Avatar_Default1.jpg" : user.Avatar)
            };

           
			return Json(userModel);
		}

        [HttpPost]
        public async Task MarkConversationRead(string fromId, string toId)
		{
			await _messageStore.SetMessagesAsRead(fromId, toId);
		}

        [HttpGet]
        public async Task<IActionResult> UserHasRated(string fromId, string toId)
        {
           bool result= await _ratingstore.HasRating(fromId, toId);
            if (result)
            {
                return PartialView();
            }
            else
            {
                return View("SetRating");
            }
            
        }

        [HttpPost]
        public async Task<JsonResult> SetRating(Rating newrating)
        {

            if (newrating != null && newrating.RatingText != "")
            {

                await _ratingstore.SetRating(new Rating()
                {
                    FromId = newrating.FromId,
                    ToId = newrating.ToId,
                    RatingText = newrating.RatingText,
                    RatingValue = newrating.RatingValue
                });

               // await _context.SaveChangesAsync();

            }
            return Json("Ok");
        }

    }
}
