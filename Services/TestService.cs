namespace TutorHelper.Services
{
    public interface ITestService
    {
        string returnLoggedId();
    }

    public class TestService : ITestService
    {
        private readonly IUserContextService _ucs;

        public TestService(IUserContextService userContextSerivce)
        {
            _ucs = userContextSerivce;
        }

        public string returnLoggedId()
        {
            string loggedId = _ucs.GetAuthenticatedUserId;
            return loggedId;
        }
    }
}