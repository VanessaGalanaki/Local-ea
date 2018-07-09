using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Localea.Infrastructure.POCOs;
using Localea.Models;
using Localea.Models.MapViewModels;

namespace Localea.Data.Interfaces
{
	public interface IStoreMessages
    {      
        Task AddNewMessage(Message message);
        Task<List<Message>> GetMessagesBetween(string id1, string id2);
        List<string> myChatBuddies(string userId);

		Task<List<Message>> GetUnreadMessages(string myId);      
		Task SetMessagesAsRead(string FromId, string ToId);
		Task DeleteMessagesFromOrBy(string id);
    }
}
