using System.ComponentModel.DataAnnotations;

namespace Web_QuanLy_PolyCafe.Models
{
    public class Category
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<Drink>? Drinks { get; set; }
    }
}