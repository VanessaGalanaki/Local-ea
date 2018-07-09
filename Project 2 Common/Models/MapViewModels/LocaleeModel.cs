using System;
using System.Collections.Generic;

namespace Localea.Models.MapViewModels
{
    public class LocaleeModel
    {
		public string Id { get; set; }
		public string UserNameStr { get; set; }

        public string lat { get; set; }
        public string lon { get; set; }
		public string Avatar { get; set; }
        
		public decimal OverallRating { get; set; }
		public int ReceivedRatingsCount { get; set; }
    }
}
