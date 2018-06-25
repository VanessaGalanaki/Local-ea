using Microsoft.AspNetCore.Http;
using Project_2_Common.Infrastructure.POCOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2_Common.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Id { get; set; }
		public string UserNameStr { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string AboutMe { get; set; }
        
		[Required]
        [EmailAddress]
        public string Email { get; set; }

        //[FileExtensions(Extensions = "jpg,jpeg,png,gif")]
        public IFormFile File { get; set; }
        public string Avatar { get; set; }
        public bool showMeOnMap { get; set; }
		public string lat { get; set; }
        public string lon { get; set; }
        public ICollection<Rating> ReceivedRatings { get; set; }



        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }

        
    }
}
