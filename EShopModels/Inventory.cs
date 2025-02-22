namespace EShopModels
{
    using EShopModels.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Inventory : BaseEntity
    {
        public Inventory(string code, int productID, int unitChartID, decimal quantity, int batchID, decimal sellingPrice, decimal purchasePrice, decimal reservedQuantity)
        {
            this.Code = code;
            this.ProductID = productID;
            this.UnitChartID = unitChartID;
            this.Quantity = quantity;
            this.BatchID = batchID;
            this.SellingPrice = sellingPrice;
            this.PurchasePrice = purchasePrice;
            this.ReservedQuantity = reservedQuantity;
        }


        [Required]
        [StringLength(15)]
        public string Code { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UnitType Not Selected")]
        public int UnitChartID { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public int BatchID { get; set; }
        [Required]
        public decimal SellingPrice { get; set; }
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public decimal ReservedQuantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual UnitChart UnitChart { get; set; }
        public virtual string UnitChartName{
            get
            {
                if(UnitChart != null)
                {
                   return UnitChart.UnitChartName;
                }
                else
                {
                    return string.Empty;
                }
            }                
        } 
    }
}
