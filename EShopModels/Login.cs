namespace EShopModels
{
    using EShopModels.Common;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Login : BaseEntity
    {
        public Login(int EShopUserID,string userName,string password,bool isRemember)
        {
            this.EShopUserID = EShopUserID;
            this.UserName = userName;
            this.Password = password;
            this.IsRemember = isRemember;
        }

        [Key]
        [ForeignKey("EShopUser")]
        public int EShopUserID { get; set; }
        [Required]
        [StringLength(200)] 
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(200)]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool IsRemember { get; set; }
       
        public virtual EShopUser EShopUser { get; set; }
    }
}
