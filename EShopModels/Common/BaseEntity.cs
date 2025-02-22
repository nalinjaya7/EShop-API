using System.ComponentModel.DataAnnotations;

namespace EShopModels.Common
{
    public abstract class BaseEntity
    {
        [Required]
        public int ID { get; set; }
 
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ModifiedDate { get; set; }
        [Required]
        public bool IsDeleted { get; set; } 
        [Timestamp]
        public byte[] RowVersion { get; set; } 
    }
}
