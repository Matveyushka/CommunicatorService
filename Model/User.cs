public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<PrivateMessage> IncomingPrivateMessages { get; set; }
    public virtual ICollection<PrivateMessage> OutcomingPrivateMessages { get; set; }
    public virtual ICollection<UsersRelation> IncomingUsersRelation { get; set; }
    public virtual ICollection<UsersRelation> OutcomingUsersRelation { get; set; }


    public User(string name)
    {
        this.Name = name;
        this.IncomingPrivateMessages = new List<PrivateMessage>();
        this.OutcomingPrivateMessages = new List<PrivateMessage>();
        this.IncomingUsersRelation = new List<UsersRelation>();
        this.OutcomingUsersRelation = new List<UsersRelation>();
    }
}