public class PrivateMessage
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SendingDateTime { get; set; }
    public DateTime ReceiptDateTime { get; set; }

    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public Guid RecipientId { get; set; }
    public User Recipient { get; set; } = null!;
}