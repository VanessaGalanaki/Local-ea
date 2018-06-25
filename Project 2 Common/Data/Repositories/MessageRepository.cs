using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models;
using Project_2_Common.Models.MapViewModels;

namespace Project_2_Common.Data
{   
	public class MessageRepository : IStoreMessages
    {
		
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public MessageRepository(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;

        }

        public async Task AddNewMessage(Message newmessage)
        {
            await _context.Messages.AddAsync(newmessage);
            await _context.SaveChangesAsync();
        }

        public List<string> myChatBuddies(string userId)
        {
            var user = _context.Users.Include("ReceivedMessages").Include("SentMessages").FirstOrDefault(x => x.Id == userId);
            var received = user.ReceivedMessages.Select(r => r.SenderId).ToList();
			var sent = user.SentMessages.Select(r => r.ReceiverId).ToList();
            
			var ConcatBuddies = received.Concat(sent);
            var ChatBuddies = ConcatBuddies.Distinct().ToList();
            return ChatBuddies;
        }

        public async Task<List<Message>> GetMessagesBetween(string userId, string receiverId)
        {
            var messages = _context.Messages.Include(m=>m.Receiver).Include(m=>m.Sender).Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == userId)).OrderBy(m=>m.DateSent);
            return await messages.ToListAsync();
        }
              
		public async Task<List<Message>> GetUnreadMessages(string myId)
		{
			var messages =  _context.Messages.Where(m=>m.ReceiverId==myId && !m.Read).Include(m => m.Sender).Include(m => m.Receiver).OrderBy(m => m.DateSent);

			return await messages.ToListAsync();
		}
        
		public async Task SetMessagesAsRead(string fromId, string toId)
		{
			var messages = _context.Messages.Where(m => m.SenderId == fromId && m.ReceiverId==toId);
            foreach (var messaz in messages)
			{
				messaz.Read = true;
			}
			await _context.SaveChangesAsync();
		}
        
        public async Task DeleteMessagesFromOrBy(string id)
		{
			var messages = _context.Messages.Where(m => m.SenderId == id || m.ReceiverId == id);
			_context.Messages.RemoveRange(messages);
			await _context.SaveChangesAsync();
		}
	}
}
