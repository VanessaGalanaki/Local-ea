using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Localea.Data.Interfaces;
using Localea.Models;
using Localea.Models.MapViewModels;

namespace Localea.Data.Repositories
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
