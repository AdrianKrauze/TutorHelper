using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy.Strategies
{
    public class SubscribeStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return EmailTemplateHelper.GenerateHtmlTemplateForAdmin("Problemy z subskrypcją", dto);
        }

        public string ReturnEmailSubject(CreateEmailDto dto)
        {
            return EmailTemplateHelper.ReturnSubject(dto);
        }
    }
}
