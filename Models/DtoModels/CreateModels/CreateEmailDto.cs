using Bogus.DataSets;
using TutorHelper.EmailStrategy;

namespace TutorHelper.Models.DtoModels.CreateModels
{
    public class CreateEmailDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Content { get; set; }
        public EmailCase EmailCase { get; set; }
    }
}
