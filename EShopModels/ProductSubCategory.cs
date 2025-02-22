namespace EShopModels
{
    using EShopModels.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ProductSubCategory : BaseEntity
    {
        public ProductSubCategory(int productCategoryID,string name)
        {
            this.ProductCategoryID = productCategoryID;
            this.Name = name;
        }

        [Required]
        public int ProductCategoryID { get; set; }
        [Required] 
        [StringLength(100)]
        public string Name { get; set; }
       
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
