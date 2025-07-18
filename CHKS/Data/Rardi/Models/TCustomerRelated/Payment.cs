using chks.Enum;

public class PaymentModel
{
    public PaymentType PaymentType { get; set; } = PaymentType.Bank;
    public decimal Amount { get; set; } = 0; 
}
