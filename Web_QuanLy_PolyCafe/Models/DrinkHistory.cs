using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class DrinkHistory
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Đồ uống không được để trống")]
        [StringLength(50)]
        [Display(Name = "Đồ uống")]
        public string DrinkId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(150, ErrorMessage = "Tên tối đa 150 ký tự")]
        [Display(Name = "Tên (tại thời điểm lưu)")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự")]
        [Display(Name = "Ảnh")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, 99999999.99, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [StringLength(50)]
        [Display(Name = "Danh mục")]
        public string? CategoryId { get; set; }

        [Display(Name = "Còn phục vụ")]
        public bool? IsAvailable { get; set; }

        [Display(Name = "Đã xóa")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Thời điểm thay đổi")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        // Navigation
        public Drink? Drink { get; set; }
    }
}