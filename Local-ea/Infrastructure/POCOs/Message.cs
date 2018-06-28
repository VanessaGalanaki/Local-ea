using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project_2_Common.Models;

namespace Project_2_Common.Infrastructure.POCOs
{
    public class Message
    {
		public int MessageId { get; set; }
        
        public string SenderId { get; set; }
		public virtual ApplicationUser Sender { get; set; }
        
		public string ReceiverId { get; set; }
		public virtual ApplicationUser Receiver { get; set; }

		public string Body { get; set; }
		public DateTime DateSent { get; set; }
		public bool Read { get; set; }

		public Message()
        {
            DateSent = DateTime.Now;
        }
    }
}
