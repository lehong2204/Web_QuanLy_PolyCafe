using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class Order
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Khách hàng không được để trống")]
        [StringLength(50)]
        [Display(Name = "Khách hàng")]
        public string UserId { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tổng tiền không được để trống")]
        [Range(0, 99999999.99, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalPrice { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "Số tiền giảm phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [StringLength(50, ErrorMessage = "Trạng thái tối đa 50 ký tự")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [StringLength(50)]
        [Display(Name = "Voucher")]
        public string? VoucherId { get; set; }

        // Navigation
        public User? User { get; set; }
        public Voucher? Voucher { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}