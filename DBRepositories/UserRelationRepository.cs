public class UserRelationRepository
{
    CommunicatorDbContext context { get; set; }

    public UserRelationRepository(CommunicatorDbContext context)
    {
        this.context = context;
    }

    public UsersRelation Add(Guid subjectUserId, Guid targetUserId, bool display = true, bool mute = false, bool block = false)
    {
        var newRelation = new UsersRelation()
        {
            SubjectId = subjectUserId,
            TargetId = targetUserId,
            Displayed = display,
            Muted = mute,
            Blocked = block
        };
        context.UsersRelation.Add(newRelation);
        context.SaveChanges();
        return newRelation;
    }
}