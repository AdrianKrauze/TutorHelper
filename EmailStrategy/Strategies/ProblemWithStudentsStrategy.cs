using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy.Strategies
{
    public class ProblemWithStudentsStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return EmailTemplateHelper.GenerateHtmlTemplateForAdmin("Problem z obsługą uczniów", dto);
        }

        public string ReturnEmailSubject(CreateEmailDto dto)
        {
            return EmailTemplateHelper.ReturnSubject(dto);
        }
    }
}
