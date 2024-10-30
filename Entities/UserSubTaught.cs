using TutorHelper.Entities;
using TutorHelper.Entities.ToSeed;

public class UserSubTaught
{
    public string UserId { get; set; }
    public User User { get; set; }

    public string SubjectId { get; set; }
    public Subject Subject { get; set; }
}