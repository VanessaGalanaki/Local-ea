using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2_Common.Data.Interfaces
{
    public interface IStoreRatings
    {
        Task SetRating(Rating rating);
        Task<List<Rating>> ReadmyRatings(string awardedUserId);
        Task<bool> HasRating(string fromId,string toId);
        Task<List<Rating>> VisitorWindowsHasRatings(string awarderUserId);
		Task DeleteRatingsReceivedBy(string id);      
    }
}
