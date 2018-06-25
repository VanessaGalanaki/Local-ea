using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Project_2_Common.Data;
using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Project_2_Common.Models.ChatviewModels;
using Project_2_Common.Data.Interfaces;
using System.Collections.Generic;

namespace Project_2_Common.Infrastructure
{
    public class ChatHub : Hub
    {      
        private readonly IStoreMessages _store;
        private readonly UserManager<ApplicationUser> _manager;
        public ChatHub(IStoreMessages store, UserManager<ApplicationUser> manager)
        {
            _manager = manager;
            _store = store;
        }
              

        public async Task SendMessage(MessageViewModel model)
        {
            Message message = new Message
            {
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                DateSent = model.Date.ToLocalTime(),
                Body = model.Body,
                Read = false
            };
            await _store.AddNewMessage(message);
			model.Date = message.DateSent;
            model.MessageId = message.MessageId;
            model.hasBeenRead = message.Read;

			await NotifyUser(model);
            
			await Clients.User(model.ReceiverId).SendAsync("ReceiveMessage", model);
        }

        public override async Task OnConnectedAsync()
        {
            var me =  await _manager.GetUserAsync(Context.User);
            List<Message> unreadMessages = await _store.GetUnreadMessages(me.Id);
            foreach (var messaze in unreadMessages)
            {   
                await Clients.Caller.SendAsync("ReceiveMessage", new MessageViewModel
                {
                    MessageId = messaze.MessageId,
                    SenderId = messaze.SenderId,
                    SenderName = messaze.Sender.UserNameStr,
                    ReceiverId = messaze.ReceiverId,
                    ReceiverName = messaze.Receiver.UserNameStr,
                    Body = messaze.Body
                });
            }
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }



		public async Task NotifyUser(MessageViewModel model)
		{
			var user = await _manager.FindByIdAsync(model.SenderId);

			await Clients.User(model.ReceiverId).SendAsync("Notify", user.UserNameStr,user.Avatar);         
		}
    }
 
}
