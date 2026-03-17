using System.ComponentModel.DataAnnotations;

namespace Web_QuanLy_PolyCafe.Models
{
    public class VoucherScope
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Voucher không được để trống")]
        [StringLength(50)]
        [Display(Name = "Voucher")]
        public string VoucherId { get; set; }

        /// <summary>0 = All | 1 = Specific drink</summary>
        [Display(Name = "Loại áp dụng (0=Tất cả, 1=Cụ thể)")]
        public int ApplyType { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "Đồ uống áp dụng")]
        public string? DrinkId { get; set; }

        // Navigation
        public Voucher? Voucher { get; set; }
        public Drink? Drink { get; set; }
    }
}