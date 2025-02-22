using EShopModels.Common;
using System.ComponentModel.DataAnnotations;

namespace EShopModels
{
    public class SearchBox
    { 
        [Required]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public SearchBoxType Type { get; set; }
        [Required]
        public List<Price> Prices { get; set; }
    }

    public class Price
    {
        [Required]
        public decimal AvailableQty { get; set; }
        [Required]
        public string UnitName { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
    }
}
