using System.Globalization;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using TutorHelper.Entities;
using TutorHelper.Models.GoogleCalendarModels;

namespace TutorHelper.Services
{
    public interface IGoogleCalendarApi
    {
        Task<string> AddEventToGoogleCalendar(GoogleCalendarEvent googleCalendarEvent);

        Task<List<PlaceholderEvent>> GetCalendarData(bool forceRefresh = false);

        Task UpdateGoogleEvent(Lesson lesson);
        Task DeleteGoogleEvent(Lesson lesson);
    }

    public class GoogleCalendarApi : IGoogleCalendarApi
    {
        private const string CalendarId = "primary";
        private readonly UserManager<User> _userManager;
        private readonly IUserContextService _userContextService;
        private static readonly string solutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        private static readonly string credentialsPath = Path.Combine(solutionPath, "TutorHelper", "Files", "credentials.json");
        private static readonly string tokensPath = Path.Combine(solutionPath, "TutorHelper", "Files", "Tokens");
        private static readonly string ApplicationName = "TutorHelperTest";
        private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GoogleCalendarApi(UserManager<User> userManager, IUserContextService userContextService, IMemoryCache memoryCache, IMapper mapper)
        {
            _userManager = userManager;
            _userContextService = userContextService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        public async Task<List<PlaceholderEvent>> GetCalendarData(bool forceRefresh = false)
        {
            if (!forceRefresh && _memoryCache.TryGetValue("CalendarData", out List<PlaceholderEvent> cachedData))
            {
                return cachedData;
            }

            var now = DateTime.UtcNow;
            var startDate = now.AddDays(-7);
            var endDate = now.AddDays(14);

            UserCredential credential = await GetUserCredential();
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            EventsResource.ListRequest request = service.Events.List(CalendarId);
            request.TimeMin = startDate;
            request.TimeMax = endDate;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.TimeZone = "Europe/Warsaw";
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = await request.ExecuteAsync();

            var placeholderCalendar = events.Items.Select(eventItem =>
            {
                DateTime eventStartTime = eventItem.Start.DateTime.Value;
                DateTime eventEndTime = eventItem.End.DateTime.Value;

                DateTime localEventStartTime = eventStartTime.ToLocalTime();
                DateTime localEventEndTime = eventEndTime.ToLocalTime();

                var placeholderEvent = _mapper.Map<PlaceholderEvent>(eventItem);
                placeholderEvent.StartDateTime = DateTime.SpecifyKind(localEventStartTime, DateTimeKind.Unspecified);
                placeholderEvent.EndDateTime = DateTime.SpecifyKind(localEventEndTime, DateTimeKind.Unspecified);

                return placeholderEvent;
            }).ToList();

            _memoryCache.Set("CalendarData", placeholderCalendar, TimeSpan.FromDays(7));

            return placeholderCalendar;
        }

        public async Task<string> AddEventToGoogleCalendar(GoogleCalendarEvent googleCalendarEvent)
        {
            UserCredential credential = await GetUserCredential();

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var newEvent = MapGoogleCalendarEventToEvent(googleCalendarEvent);

            try
            {
                EventsResource.InsertRequest request = service.Events.Insert(newEvent, CalendarId);
                Event createdEvent = await request.ExecuteAsync();
                return createdEvent.Id;
            }
            catch (Google.GoogleApiException ex)
            {
                throw new Exception($"Error adding event to Google Calendar: {ex.Message} (Status Code: {ex.HttpStatusCode})");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }

        private static Event MapGoogleCalendarEventToEvent(GoogleCalendarEvent googleCalendarEvent)
        {
            Event.RemindersData reminders = new Event.RemindersData()
            {
                UseDefault = false,
                Overrides = googleCalendarEvent.PushBoolean && googleCalendarEvent.PushTimeBeforeLesson.HasValue
                   ? new EventReminder[]
                   {
                new EventReminder()
                {
                    Method = "popup",
                    Minutes = (int)(googleCalendarEvent.PushTimeBeforeLesson.Value)
                }
                   }
                   : null
            };

            Event newEvent = new Event()
            {
                Summary = googleCalendarEvent.Summary,
                Description = "From TutorHelperApp",
                Start = new EventDateTime()
                {
                    DateTime = googleCalendarEvent.StartTime,
                    TimeZone = googleCalendarEvent.TimeZone,
                },
                End = new EventDateTime()
                {
                    DateTime = googleCalendarEvent.EndTime,
                    TimeZone = googleCalendarEvent.TimeZone,
                },
                Reminders = reminders,
            };

            return newEvent;
        }

        public async Task UpdateGoogleEvent(LessonWithStudent lesson)
        {
       
            if (string.IsNullOrEmpty(lesson.GoogleEventId))
            {
                throw new ArgumentException("GoogleEventId is required to update the event.");
            }

            UserCredential credential = await GetUserCredential();

           
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

           
            var googleCalendarEvent = _mapper.Map<GoogleCalendarEvent>(lesson);
            var updatedEvent = MapGoogleCalendarEventToEvent(googleCalendarEvent); 

            try
            {
                EventsResource.UpdateRequest request = service.Events.Update(updatedEvent, CalendarId, lesson.GoogleEventId);
                await request.ExecuteAsync(); 
            }
            catch (Google.GoogleApiException ex)
            {
                throw new Exception($"Error updating event in Google Calendar: {ex.Message} (Status Code: {ex.HttpStatusCode})");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while updating event: {ex.Message}");
            }
        }

        public async Task UpdateGoogleEvent(Lesson lesson)
        {
     
            if (string.IsNullOrEmpty(lesson.GoogleEventId))
            {
                throw new ArgumentException("GoogleEventId is required to update the event.");
            }

            UserCredential credential = await GetUserCredential();

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var googleCalendarEvent = _mapper.Map<GoogleCalendarEvent>(lesson);
            var updatedEvent = MapGoogleCalendarEventToEvent(googleCalendarEvent); 

            try
            {
                EventsResource.UpdateRequest request = service.Events.Update(updatedEvent, CalendarId, lesson.GoogleEventId);
                await request.ExecuteAsync(); 
            }
            catch (Google.GoogleApiException ex)
            {
                throw new Exception($"Error updating event in Google Calendar: {ex.Message} (Status Code: {ex.HttpStatusCode})");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while updating event: {ex.Message}");
            }
        }

        public async Task DeleteGoogleEvent(Lesson lesson)
        {
           
            if (string.IsNullOrEmpty(lesson.GoogleEventId))
            {
                throw new ArgumentException("GoogleEventId is required to update the event.");
            }
            UserCredential credential = await GetUserCredential();

           
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            string objectIdToDelete = lesson.GoogleEventId;

            try
            {
                EventsResource.DeleteRequest request = service.Events.Delete(CalendarId, objectIdToDelete);
                await request.ExecuteAsync(); 
            }
            catch (Google.GoogleApiException ex)
            {
                throw new Exception($"Error updating event in Google Calendar: {ex.Message} (Status Code: {ex.HttpStatusCode})");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while updating event: {ex.Message}");
            }
        }

        private async Task<UserCredential> GetUserCredential()
        {
            string userId = _userContextService.GetAuthenticatedUserId;
            UserCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    userId,
                    CancellationToken.None,
                    new FileDataStore(tokensPath, true));
            }
            return credential;
        }
    }
}