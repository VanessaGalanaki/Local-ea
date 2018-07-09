using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Localea.Models.MapViewModels
{
    public class MapUserModel
    {
		public string Id { get; set; }
		public string UserNameStr { get; set; }
        public string Avatar { get; set; }
        public IFormFile File { get; set; }
        public List<MapUserModel> MyChatBuddies { get; set; }
    }
}
