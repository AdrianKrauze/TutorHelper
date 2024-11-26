using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy.Strategies
{
    public class DefaultMailStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return EmailTemplateHelper.GenerateHtmlTemplateForAdmin("Inne", dto);
        }

        public string ReturnEmailSubject(CreateEmailDto dto)
        {
            return EmailTemplateHelper.ReturnSubject(dto);
        }
    }
}
