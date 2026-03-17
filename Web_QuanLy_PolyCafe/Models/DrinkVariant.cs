using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class DrinkVariant
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Đồ uống không được để trống")]
        [StringLength(50)]
        [Display(Name = "Đồ uống")]
        public string DrinkId { get; set; }

        [Required(ErrorMessage = "Tên biến thể không được để trống")]
        [StringLength(100, ErrorMessage = "Tên biến thể tối đa 100 ký tự")]
        [Display(Name = "Tên biến thể (Size, Topping...)")]
        public string VariantName { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "Giá thêm phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giá thêm")]
        public decimal ExtraPrice { get; set; } = 0;

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        // Navigation
        public Drink? Drink { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}