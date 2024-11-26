using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy
{
    public class PageErrorStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
