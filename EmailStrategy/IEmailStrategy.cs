using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.EmailStrategy
{
    public interface IEmailStrategy
    {
        string MakeBodyContent(CreateEmailDto dto); // Generowanie treści e-maila
        
    }
}
