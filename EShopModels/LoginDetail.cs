using EShopModels.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace EShopModels
{
    public class LoginDetail : BaseEntity
    {
        public LoginDetail(int EShopUserID,DateTime loginTime,DateTime logOutTime)
        {
            this.EShopUserID = EShopUserID; 
            this.LoginTime = loginTime;
            this.LogOutTime = logOutTime;
        }

        [Required]
        public int EShopUserID { get; set; } 
        [Required]
        public DateTime LoginTime { get; set; }
        [Required]
        public DateTime LogOutTime { get; set; }

        public EShopUser EShopUser { get; set; } 
    }
}
