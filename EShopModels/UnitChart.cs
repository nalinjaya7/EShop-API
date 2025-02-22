namespace EShopModels
{
    using EShopModels.Common;
    using System.ComponentModel.DataAnnotations;

    public partial class UnitChart : BaseEntity
    {
        public UnitChart(int unitTypeID, int productID, decimal quantity, string unitChartName)
        {
            this.UnitTypeID = unitTypeID;
            this.ProductID = productID;
            this.Quantity = quantity;
            this.UnitChartName = unitChartName;
        }

        [Required] 
        public virtual int UnitTypeID { get; set; }
        [Required] 
        public virtual int ProductID { get; set; }
        [Required]
        public decimal Quantity { get; set; }
         
        public string UnitTypeName
        {
            get
            {
                return (UnitType == null) ? "" : UnitType.Name;
            }
        }

        [Required]
        [StringLength(maximumLength:100)] 
        public string UnitChartName { get; set; }
       
        public virtual UnitType UnitType { get; set; }
        public virtual Product Product { get; set; }
    }
}
