public class CommunicatorDeleteInfo
{
    public string Recipient { get; set; } = null!;
    public string Sender { get; set; } = null!;
    public Guid Id { get; set; }
    public bool Watched { get; set; }
}