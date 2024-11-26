using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy.Strategies
{
    public class PageErrorStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return EmailTemplateHelper.GenerateHtmlTemplateForAdmin("Błędy na stronie", dto);
        }

        public string ReturnEmailSubject(CreateEmailDto dto)
        {
            return EmailTemplateHelper.ReturnSubject(dto);
        }
    }
}
