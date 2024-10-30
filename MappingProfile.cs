using AutoMapper;
using Google.Apis.Calendar.v3.Data;
using TutorHelper.Entities;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Models.GoogleCalendarModels;

namespace TutorHelper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateStudentDto, Student>();

            CreateMap<CreateLessonDtoWithStudent, LessonWithStudent>();

            CreateMap<CreateLessonDtoWoStudent, Lesson>();

            CreateMap<Student, ViewStudentDto>()
                .ForMember(s => s.SubjectName, opt => opt.MapFrom(src => src.Subject.Topic))
                .ForMember(e => e.EduStageName, opt => opt.MapFrom(e => e.EduStage.Name))
                .ForMember(e => e.LessonPlaceName, opt => opt.MapFrom(e => e.LessonPlace.Type))
                .ForMember(e => e.StudentConditionName, opt => opt.MapFrom(e => e.StudentCondition.Condition));

            CreateMap<Student, ViewStudentDtoToList>()
                 .ForMember(s => s.SubjectName, opt => opt.MapFrom(src => src.Subject.Topic))
                .ForMember(e => e.EduStageName, opt => opt.MapFrom(e => e.EduStage.Name))
                .ForMember(e => e.LessonPlaceName, opt => opt.MapFrom(e => e.LessonPlace.Type))
                .ForMember(e => e.StudentConditionName, opt => opt.MapFrom(e => e.StudentCondition.Condition));

            CreateMap<LessonWithStudent, LessonListByStudentIdDto>();
            CreateMap<CreateNoteDto, Note>();

            CreateMap<Lesson, LessonObjectDto>()
           .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Topic))
           .ForMember(dest => dest.EduStage, opt => opt.MapFrom(src => src.EduStage.Name))
           .ForMember(dest => dest.LessonPlaceName, opt => opt.MapFrom(src => src.LessonPlace.Type))
           .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.StudentCondition.Condition))
           .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ContactTips));

            CreateMap<Lesson, GoogleCalendarEvent>()
             .ForMember(dest => dest.Summary, opt =>
                 opt.MapFrom(src => $"{src.StudentFirstName} {src.StudentLastName} - {src.ContactTips}"))
             .ForMember(dest => dest.StartTime, opt =>
                 opt.MapFrom(src => src.Date))
             .ForMember(dest => dest.EndTime, opt =>
                 opt.MapFrom(src => src.EndDate));


            CreateMap<LessonWithStudent, GoogleCalendarEvent>()
            .ForMember(dest => dest.Summary, opt =>
                opt.MapFrom(src => $"{src.StudentFirstName} {src.StudentLastName} - {src.ContactTips}"))
            .ForMember(dest => dest.StartTime, opt =>
                opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.EndTime, opt =>
                opt.MapFrom(src => src.EndDate));


            CreateMap<Event, PlaceholderEvent>()
           .ForMember(dest => dest.Title, opt => opt.MapFrom(src => "Zajęte w kalendarzu google"))
           .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.Start.DateTime.HasValue ? src.Start.DateTime.Value : DateTime.MinValue))
           .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.End.DateTime.HasValue ? src.End.DateTime.Value : DateTime.MinValue));
        }
    }
}