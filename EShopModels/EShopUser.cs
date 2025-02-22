using EShopModels.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShopModels
{
    public class EShopUser : BaseEntity
    {   
        public EShopUser()
        { 
        }
         
        public EShopUser(int iD,string userName,string firstName,string lastName,string address,string email,bool isActive,Guid activationCode,string password,Roles roleName)
        {
            this.ID = iD;
            this.UserName = userName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Address = address;
            this.Email = email;
            this.IsActive = isActive;
            this.ActivationCode = activationCode; 
            this.Password = password;
            this.ConfirmPassword = Password;
            this.RoleName = roleName;
        }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string LastName { get; set; }
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Required]
        [StringLength(250)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)] 
        //[System.Text.Json.Serialization.JsonIgnore] //prevent the password property from being serializrd and returned in API response
        public string Password { get; set; }       
        [NotMapped]
        [DataType(DataType.Password)] 
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public Guid ActivationCode { get; set; }  
 
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public virtual Roles RoleName { get; set; }

        
    }
}
