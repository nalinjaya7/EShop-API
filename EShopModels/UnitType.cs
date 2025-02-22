namespace EShopModels
{
    using EShopModels.Common;
    using System.ComponentModel.DataAnnotations;

    public partial class UnitType : BaseEntity
    {
        public UnitType(string code, string name, bool isBaseUnit)
        {
            this.Code = code;
            this.Name = name;
            this.IsBaseUnit = isBaseUnit;
        }

        [Required]
        [StringLength(15)]
        public string Code { get; set; }
        [Required]
        [StringLength(20)] 
        public string Name { get; set; }
        [Required]
        public bool IsBaseUnit { get; set; } 
    }
}
