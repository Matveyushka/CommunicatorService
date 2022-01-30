public class PrivateMessageDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SendingDateTime { get; set; }
    public DateTime? ReceiptDateTime { get; set; }
    public string Sender { get; set; }
    public string Recepient { get; set; }

    public PrivateMessageDTO(PrivateMessage message)
    {
        this.Id = message.Id;
        this.Content = message.Content;
        this.Sender = message.Sender.Name;
        this.Recepient = message.Recipient.Name;
        this.SendingDateTime = message.SendingDateTime;
        this.ReceiptDateTime = message.ReceiptDateTime;
    }
}