namespace EShopModels
{
    using EShopModels.Common;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations; 

    public partial class Product : BaseEntity
    {
        public Product(int? productSubCategoryID, int productCategoryID,string name,string barCode,string itemCode,int reOrderLevel,int? taxGroupID,bool taxInclude,decimal? taxRate)
        {
            this.ProductSubCategoryID = productSubCategoryID;
            this.ProductCategoryID = productCategoryID;
            this.Name = name;
            this.BarCode = barCode;
            this.ItemCode = itemCode;
            this.ReOrderLevel = reOrderLevel;
            this.TaxGroupID = taxGroupID;
            this.TaxInclude = taxInclude;
            this.TaxRate = taxRate;
            this.UnitCharts = new List<UnitChart>();
            this.Inventories = new List<Inventory>();
        }
  
        public int? ProductSubCategoryID { get; set; }
        [Required] 
        public int ProductCategoryID { get; set; }
        [Required] 
        [StringLength(200)] 
        public string Name { get; set; }       
        [Required]
        [StringLength(20)] 
        public string BarCode { get; set; }
        [Required]
        [StringLength(20)] 
        public string ItemCode { get; set; }
        [Required]
        public int ReOrderLevel { get; set; }
        public int? TaxGroupID { get; set; }
        [Required]
        public bool TaxInclude { get; set; }        
        public decimal? TaxRate { get; set; }
        [MaxLength(4000)]
        public byte[]? ProductImage { get; set; } 
        public virtual ProductSubCategory? ProductSubCategory { get; set; }
        [ValidateNever]         
        public virtual ProductCategory ProductCategory { get; set; }
        [ValidateNever]
        public virtual List<Inventory> Inventories { get; set; }
        [ValidateNever]
        public virtual List<UnitChart> UnitCharts { get; set; } 
    }
}
