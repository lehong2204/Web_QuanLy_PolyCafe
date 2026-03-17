using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class OrderDetail
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Đơn hàng không được để trống")]
        [StringLength(50)]
        [Display(Name = "Đơn hàng")]
        public string OrderId { get; set; }

        [Required(ErrorMessage = "Đồ uống không được để trống")]
        [StringLength(50)]
        [Display(Name = "Đồ uống")]
        public string DrinkId { get; set; }

        [StringLength(50)]
        [Display(Name = "Biến thể")]
        public string? VariantId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0, 99999999.99, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Đơn giá")]
        public decimal UnitPrice { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public Drink? Drink { get; set; }
        public DrinkVariant? Variant { get; set; }
    }
}