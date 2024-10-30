using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TutorHelper.Middlewares.Exceptions;

namespace TutorHelper.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException badRequestException)
            {
              
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"{badRequestException.Message} (Middleware)" );
            }
            catch (NotFoundException notFoundException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, $"{notFoundException.Message} (Middleware)");
            }
            catch (ForbidException forbidException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status403Forbidden, $"{forbidException.Message} (Middleware)");
            }
            catch (ArgumentNullException argumentNullException)
            {
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, $"{argumentNullException.Message} (Middleware)");
            }
            catch (Google.GoogleApiException googleApiException)
            {
                // Ustaw odpowiedni kod statusu HTTP na podstawie rodzaju błędu
                context.Response.StatusCode = googleApiException.Error.Code switch
                {
                    400 => StatusCodes.Status400BadRequest,
                    401 => StatusCodes.Status401Unauthorized,
                    403 => StatusCodes.Status403Forbidden,
                    404 => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                var errorResponse = new
                {
                    Message = $"Google API Error: {googleApiException.Message}(Middleware)",
                    Code = googleApiException.Error.Code,
                    Reason = googleApiException.Error.Errors?.FirstOrDefault()?.Reason
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            catch (DbException ex)
            {
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "bazadanych niebazodanuje "+ ex.Message);
            }
          /*  catch (Exception ex)
            {
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Something went wrong(Middleware): " + ex.Message);
            }*/
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                Message = message
            };

            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
