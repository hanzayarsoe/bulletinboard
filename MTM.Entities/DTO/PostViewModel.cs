using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace MTM.Entities.DTO
{
    public class PostListViewModel
    {
        #region Properties
        public List<PostViewModel> PostList { get; set; }
        #endregion 

        public PostListViewModel() {
            this.PostList = new List<PostViewModel>();
        }
    }

    public class PostViewModel
    {
        public PostViewModel()
        {
            this.Id = string.Empty;
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.IsPublished = false;
            this.IsDeleted = false;
            this.CreatedDate = DateTime.Now;
            this.CreatedUserId = string.Empty;
            this.UpdatedDate = DateTime.Now;
            this.UpdatedUserId = string.Empty;
            this.DeletedDate = DateTime.Now;
            this.DeletedUserId = string.Empty;
            // virtual
            this.CreatedFullName = string.Empty;
        }

        #region Properties
        [DisplayName("No")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Post Title is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters")]
        public string Title { get; set; }
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Decription must be between 5 and 200 characters.")]
        public string Description { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Created User")]
        public string CreatedUserId { get; set; }

        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [DisplayName("Updated User")]
        public string UpdatedUserId { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string DeletedUserId { get; set; }

        //vitual properties
        public string CreatedFullName { get; set; }
        #endregion
    }
}






