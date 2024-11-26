using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy.Strategies
{
    public class OtherStrategy : IEmailStrategy
    {
        public string MakeBodyContent(CreateEmailDto dto)
        {
            return EmailTemplateHelper.GenerateHtmlTemplateForAdmin("Inne", dto); 
        }
        //GenerateHtmplTemlate receive first parameter as subject but this is not a email subject, this is a message subject

        public string ReturnEmailSubject(CreateEmailDto dto)
        {
            return EmailTemplateHelper.ReturnSubject(dto);
        }
    }
}
