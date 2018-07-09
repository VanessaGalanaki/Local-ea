using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Localea.Models;
using Localea.Models.MapViewModels;

namespace Localea.Data.Interfaces
{
    public interface IUserRepo
    {
		Task DeleteUser(ApplicationUser user);
		List<ApplicationUser> GetLocalsInFrame(MapFrame mapFrame);
    }
}
