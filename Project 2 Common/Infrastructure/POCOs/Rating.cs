using Project_2_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2_Common.Infrastructure.POCOs
{
    public class Rating
    {
        public int Id { get; set; }
        public decimal RatingValue { get; set; }
        public string RatingText { get; set; }
        public string FromId { get; set; }
        public virtual ApplicationUser Reviewer { get; set; }
        public string ToId{ get; set; }
        public virtual ApplicationUser AwardedUser { get; set; }
    }
}
