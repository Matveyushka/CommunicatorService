public class UsersRelation
{
    public Guid Id { get; set; }
    public bool Displayed { get; set; } = false;
    public bool Muted { get; set; } = false;
    public bool Blocked { get; set; } = false;

    public Guid SubjectId { get; set; }
    public User SubjectUser { get; set; } = null!;
    
    public Guid TargetId { get; set; }
    public User TargetUser { get; set; } = null!;
}