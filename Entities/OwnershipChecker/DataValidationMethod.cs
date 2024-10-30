
using Microsoft.AspNetCore.Http.HttpResults;
using TutorHelper.Middlewares.Exceptions;

namespace TutorHelper.Entities.OwnershipChecker;

public static class DataValidationMethod
{

    public static void OwnershipAndNullChecker<T>(T? obj, string userId) where T : class, IOwner
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Obiekt jest pusty");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BadRequestException("Musisz być zalogowany");
        }

        if (obj.CreatedById != userId)
        {
            throw new ForbidException("Nie jesteś autoryzowany do edycji tego obiektu");
        }

    }
    public static void OwnershipAndNullChecker<T>(T? obj1, T? obj2, string userId) where T : class, IOwner
    {
        if (obj1 == null)
        {
            throw new ArgumentNullException("Something went wrong");
        }

        if (obj2 == null)
        {
            throw new ArgumentNullException("Something went wrong");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BadRequestException("Musisz być zalogowany");
        }

        if (obj1.CreatedById != userId)
        {
            throw new ForbidException("Nie jesteś autoryzowany do edycji");
        }

        if (obj2.CreatedById != userId)
        {
            throw new ForbidException("Nie jesteś autoryzowany do edycji");
        }
    }



}

