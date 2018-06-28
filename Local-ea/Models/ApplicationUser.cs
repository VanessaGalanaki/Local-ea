using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Project_2_Common.Infrastructure.POCOs;
using Project_2_Common.Models.MapViewModels;

namespace Project_2_Common.Models
{
    
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {      
		public string UserNameStr { get; set; }
		public bool showMeOnMap { get; set; }
		public string lat { get; set; }
        public string lon { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        //[NotMapped]
        //public IFormFile File { get; set; }

        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }

        
        public virtual ICollection<Rating> GivenRatings { get; set; }
        
        public virtual ICollection<Rating> ReceivedRatings { get; set; }
        
  

        public bool InRange(MapFrame mapFrame)
        {
			var latT = Double.Parse(lat);
			var lonT = Double.Parse(lon);

			var latBL = Double.Parse(mapFrame.latBotLeft);
			var lonBL = Double.Parse(mapFrame.lonBotLeft);
			var latTR = Double.Parse(mapFrame.latTopRight);
			var lonTR = Double.Parse(mapFrame.lonTopRight);
			
			if (latT > latBL && latT < latTR && lonT > lonBL && lonT < lonTR)
                return true;
            return false;
        }


    }
}
