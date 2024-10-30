using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Entities;
using AutoMapper;
using TutorHelper.Entities.OwnershipChecker;
using Microsoft.EntityFrameworkCore;

namespace TutorHelper.Services
{
    public interface INoteService
    {
        Task CreateNote(string studentId, CreateNoteDto dto);

        Task DeleteNote(string noteId);

        Task<Note> GetNoteById(string studentId, string noteId);

        Task UpdateNote(string noteId, string Content);

        Task<List<Note>> GetListOfNotes(string studentId);
    }

    public class NoteService : INoteService
    {
        private readonly TutorHelperDb _db;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public NoteService(TutorHelperDb tutorHelperDb, IMapper mapper, IUserContextService _ucs)
        {
            _db = tutorHelperDb;
            _mapper = mapper;
            _userContextService = _ucs;
        }

        public async Task CreateNote(string studentId, CreateNoteDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("Bad Note Data");
            }
            var student = await _db.Students.FindAsync(studentId);
            var userId = _userContextService.GetAuthenticatedUserId;
            DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var note = _mapper.Map<Note>(dto);
            note.CreatedById = userId;
            note.CreateTime = DateTime.Now;
            note.StudentId = studentId;

            await _db.Notes.AddAsync(note);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateNote(string noteId, string Content)
        {
            var note = await _db.Notes.FindAsync(noteId);
            var userId = _userContextService.GetAuthenticatedUserId;
            DataValidationMethod.OwnershipAndNullChecker(note, userId);

            if (string.IsNullOrWhiteSpace(Content))
            {
                throw new ArgumentNullException($"Aktualizacja nie może być pusta");
            }
            if (Content.Length > 300)
            {
                throw new Exception("Aktualizacja notatki może mieć maks. 300 znaków");
            }
            note.Content = $"{note.Content}\n{Content} Updated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";
            if (note.Content.Length > 5000)
            {
                throw new Exception("Notatka może mieć maks. 5000 znaków");
            }
            await _db.SaveChangesAsync();
        }

        public async Task DeleteNote(string noteId)
        {
            var note = await _db.Notes.FindAsync(noteId);
            var userId = _userContextService.GetAuthenticatedUserId;
            DataValidationMethod.OwnershipAndNullChecker(note, userId);
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Note>> GetListOfNotes(string studentId)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var student = await _db.Students.FindAsync(studentId);
            DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var notes = await _db.Notes
                .Where(n => n.StudentId == studentId)
                .ToListAsync();

            return notes;
        }

        public async Task<Note> GetNoteById(string studentId, string noteId)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var student = await _db.Students.FindAsync(studentId);

            var note = await _db.Notes.FindAsync(noteId);
            DataValidationMethod.OwnershipAndNullChecker<IOwner>(student, note, userId);

            return note; // Zwróć znalezioną notatkę
        }
    }
}