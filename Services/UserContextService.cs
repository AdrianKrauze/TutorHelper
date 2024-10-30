using System.Security.Claims;
using TutorHelper.Middlewares.Exceptions;

namespace TutorHelper.Services
{
    public interface IUserContextService
    {
        string GetFirstName { get; }
        string GetAuthenticatedUserId { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ClaimsPrincipal User => _contextAccessor.HttpContext?.User;

        public UserContextService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private string? GetUserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string GetFirstName => User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        public string GetAuthenticatedUserId =>
            GetUserId ?? throw new ForbidException("User is not authenticated");
    }
}