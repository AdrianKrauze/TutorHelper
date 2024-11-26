using AutoMapper;
using TutorHelper.Entities.DbContext;
using TutorHelper.Models.ConfigureClasses;

namespace TutorHelper.Services
{
    public interface ITestService
    {
        Task GenerateDataForLoggedUser();
        string returnLoggedId();
        Task UseDataGenerator();
        string ReturnGoogleData();
        string ReturnDevData();
    }

    public class TestService : ITestService
    {
        private readonly IUserContextService _ucs;
        private readonly TutorHelperDb _tutorHelperDb;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IDataGenerator _dataGenerator;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly DeveloperInfo _developerInfo;

        public TestService(IUserContextService userContextSerivce, TutorHelperDb tutorHelperDb, IMapper mapper, IAccountService accountService, IDataGenerator dataGenerator, GoogleAuthSettings googleAuthSettings, DeveloperInfo developerInfo)
        {
            _ucs = userContextSerivce;
            _tutorHelperDb = tutorHelperDb;
            _mapper = mapper;
            _accountService = accountService;
            _dataGenerator = dataGenerator;
            _googleAuthSettings = googleAuthSettings;
            _developerInfo = developerInfo;
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
        
        public string ReturnGoogleData()
        {
            return $"{_googleAuthSettings.ClientId} /// {_googleAuthSettings.ClientSecret}";
        }

        public string ReturnDevData()
        {
            return $"{_developerInfo.Email}";
        }
    }
}