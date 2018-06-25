using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2_Common.Models.MapViewModels
{
    public class PizzaRatingViewModel
    {
        public string fromId { get; set; }
        public string toId { get; set; }
        public decimal ratingValue { get; set; }
        public string ratingText { get; set; }
    }
}
