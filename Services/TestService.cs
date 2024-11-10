using AutoMapper;
using TutorHelper.Entities.DbContext;

namespace TutorHelper.Services
{
    public interface ITestService
    {
        Task GenerateDataForLoggedUser();
        string returnLoggedId();
        Task UseDataGenerator();
    }

    public class TestService : ITestService
    {
        private readonly IUserContextService _ucs;
        private readonly TutorHelperDb _tutorHelperDb;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IDataGenerator _dataGenerator;

        public TestService(IUserContextService userContextSerivce, TutorHelperDb tutorHelperDb, IMapper mapper, IAccountService accountService, IDataGenerator dataGenerator)
        {
            _ucs = userContextSerivce;
            _tutorHelperDb = tutorHelperDb;
            _mapper = mapper;
            _accountService = accountService;
            _dataGenerator = dataGenerator;
        }

        public string returnLoggedId()
        {
            string loggedId = _ucs.GetAuthenticatedUserId;
            return loggedId;
        }

        public async Task UseDataGenerator()
        {

            await _dataGenerator.GenerateDataAsync();
        }

        public async Task GenerateDataForLoggedUser()
        {
            await _dataGenerator.GenerateStudentAndLessonForLoggedUser();
        }
    }
}