using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2_Common.Data.Repositories
{
    public class RatingRepository : IStoreRatings
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly ApplicationDbContext _context;
        public RatingRepository(UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            _manager = manager;
            _context = context;
        }
       

        public async Task<List<Rating>> ReadmyRatings(string awarderUserId)
        {
            var myRatings = _context.Ratings.Include(m=>m.Reviewer).Where(m => m.ToId == awarderUserId);
            return await myRatings.ToListAsync();
        }
        public async Task SetRating(Rating newrating)
        {
            await _context.Ratings.AddAsync(newrating);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> HasRating(string fromId, string toId)
        {
            var ratingExist = await _context.Ratings.AnyAsync(x => x.FromId == fromId && x.ToId == toId);
            return ratingExist;

            
        }
        public async Task<List<Rating>>  VisitorWindowsHasRatings(string awarderUserId)
        {
            var hasRatings =  _context.Ratings.Include(u => u.Reviewer).Where(a => a.AwardedUser.Id == awarderUserId);
            return await hasRatings.ToListAsync();
        }
        
		public async Task DeleteRatingsReceivedBy(string id)
        {
            var ratings = _context.Ratings.Where(m =>m.ToId == id);
			_context.Ratings.RemoveRange(ratings);
			await _context.SaveChangesAsync();
        }
    }

}
