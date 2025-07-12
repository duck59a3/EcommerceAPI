using MyWebApi.Enums;

namespace MyWebApi.Responses
{
    public class PaymentResponse 
    {
        
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus status     { get; set; }
        public string PaymentUrl { get; set; }
        public object Data { get; set; }
    }
}
