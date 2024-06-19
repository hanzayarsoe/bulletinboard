using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTM.Entities.DTO
{
    public class UserListViewModel
    {
        #region Properties
        public List<UserViewModel> UserList { get; set; }
        #endregion 

        public UserListViewModel() {
            this.UserList = new List<UserViewModel>();
        }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
            this.Id = string.Empty;
            this.UserName = string.Empty;
            this.NormalizedUserName = string.Empty;
            this.Email = string.Empty;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.NormalizedEmail = string.Empty;
            this.EmailConfirmed = false;
            this.PasswordHash = string.Empty;
            this.SecurityStamp = string.Empty;
            this.ConcurrencyStamp = string.Empty;
            this.PhoneNumber = string.Empty;
            this.PhoneNumberConfirmed = false;
            this.TwoFactorEnabled = false;
            this.LockoutEnd = DateTime.Now;
            this.LockoutEnabled = false;
            this.AccessFailedCount = 0;
            this.Address = string.Empty;
            this.DOB = DateTime.Now;
            this.Role = 0;
            this.IsActive = true;
            this.IsDeleted = false;
            this.CreatedDate = DateTime.Now;
            this.CreatedUserId = string.Empty;
            this.UpdatedDate = DateTime.Now;
            this.UpdatedUserId = string.Empty;
            this.DeletedDate = DateTime.Now;
            this.DeletedUserId = string.Empty;
        }

        #region Properties
        [DisplayName("No")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category name must be less than 3")]
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        [DisplayName("Created Date")]
        public string LastName { get; set; }

        [DisplayName("Created User")]
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }

        [DisplayName("Updated Date")]
        public string PasswordHash { get; set; }

        [DisplayName("Updated User")]
        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime DeletedDate { get; set; }
        public string DeletedUserId { get; set; }


        #endregion
    }
}






