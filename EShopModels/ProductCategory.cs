namespace EShopModels
{
    using EShopModels.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class ProductCategory : BaseEntity
    {
        public ProductCategory(string name)
        {
            this.Name = name;
        }

        [Required] 
        [StringLength(100)] 
        public string Name { get; set; }
        public virtual List<Product> Products{ get; set; }
        public virtual List<ProductSubCategory> ProductSubCategories { get; set; }
    }
}
