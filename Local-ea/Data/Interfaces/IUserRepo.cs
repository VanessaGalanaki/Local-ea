using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_2_Common.Models;
using Project_2_Common.Models.MapViewModels;

namespace Project_2_Common.Data.Interfaces
{
    public interface IUserRepo
    {
		Task DeleteUser(ApplicationUser user);
		List<ApplicationUser> GetLocalsInFrame(MapFrame mapFrame);
    }
}
