using MyWebApi.DTOs.VoucherDTOs;

namespace MyWebApi.Responses
{
    public class VoucherDiscountResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int DiscountAmount { get; set; }
        public int FinalAmount { get; set; }
        public VoucherDTO Voucher { get; set; }
    }
}
