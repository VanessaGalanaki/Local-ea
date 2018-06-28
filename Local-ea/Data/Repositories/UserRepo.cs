using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Models;
using Project_2_Common.Models.MapViewModels;

namespace Project_2_Common.Data.Repositories
{
	public class UserRepo : IUserRepo
	{
		private ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _manager;

		public UserRepo(ApplicationDbContext context, UserManager<ApplicationUser> manager)
		{
			_context = context;
			_manager = manager;
		}

		public List<ApplicationUser> GetLocalsInFrame(MapFrame mapFrame)
        {
            var users = _context.Users.Include(u => u.ReceivedRatings).Where(x => (x.InRange(mapFrame) && x.showMeOnMap)).ToList();

            return users;
        }


		public async Task DeleteUser(ApplicationUser user)
		{

			var logins = await _manager.GetLoginsAsync(user);
			var roles = await _manager.GetRolesAsync(user);
            

			foreach (var login in logins)
			{
				await _manager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
			}

			if (roles.Any())
			{
				foreach (var item in roles.ToList())
				{
					// item should be the name of the role
					var result = await _manager.RemoveFromRoleAsync(user, item);
				}
			}

			//Delete User
            await _manager.DeleteAsync(user);
		}      
	}
}
