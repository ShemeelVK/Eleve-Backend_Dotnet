namespace Eleve_Backend.Application.DTOs.Payment
{
    public class PaymentVerificationDto
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId {get; set; }
        public string RazorpaySignature { get; set; }
    }
}
