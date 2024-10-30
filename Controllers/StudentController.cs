using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Entities;
using TutorHelper.Models;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Models.DtoModels.UpdateModels;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [Route("api/students")]
    [ApiController]
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserContextService _userContextService;
        private readonly INoteService _noteService;

        public StudentController(IStudentService studentService, IUserContextService userContextService, INoteService noteService)
        {
            _studentService = studentService;
            _userContextService = userContextService;
            _noteService = noteService;
        }

        #region Student Endpoints

        // Tworzenie nowego studenta
        [HttpPost("create")]
        public async Task<ActionResult> CreateStudentAsync([FromBody] CreateStudentDto dto)
        {
            await _studentService.CreateStudentAsync(dto);
            return Created();
        }

        // Pobieranie listy zakończonych studentów (pełna lista)
        [HttpGet("list/ended/full")]
        public async Task<ActionResult<List<ViewStudentDtoToList>>> ViewFullStudentListEnded()
        {
            var studentList = await _studentService.ViewEndedStudentList();
            return Ok(studentList);
        }

        // Pobieranie listy zakończonych studentów (z frazą wyszukiwania)
        [HttpGet("list/ended")]
        public async Task<ActionResult<List<ViewStudentDtoToList>>> ViewStudentListEnded([FromQuery] string searchPhrase)
        {
            var studentList = await _studentService.ViewEndedStudentListBySearchPhrase(searchPhrase);
            return Ok(studentList);
        }

        // Pobieranie listy aktywnych studentów (pełna lista)
        [HttpGet("list/active/full")]
        public async Task<ActionResult<List<ViewStudentDtoToList>>> ViewFullStudentListActive()
        {
            var studentList = await _studentService.ViewActiveStudentList();
            return Ok(studentList);
        }

        // Pobieranie listy aktywnych studentów (z frazą wyszukiwania)
        [HttpGet("list/active")]
        public async Task<ActionResult<List<ViewStudentDtoToList>>> ViewStudentListActive([FromQuery] string searchPhrase)
        {
            var studentList = await _studentService.ViewActiveStudentListBySearchPhrase(searchPhrase);
            return Ok(studentList);
        }

        // Pobieranie studenta po jego ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewStudentDto>> ViewStudentById([FromRoute] string id)
        {
            var student = await _studentService.ViewStudentById(id);
            return Ok(student);
        }

        // Aktualizacja danych studenta
        [HttpPatch("edit/{id}")]
        public async Task<ActionResult> UpdateStudent([FromRoute] string id, [FromBody] UpdateStudentDto dto)
        {
            await _studentService.UpdateStudent(id, dto);
            return NoContent();
        }

        // Usuwanie studenta po jego ID
        [HttpDelete("{studentId}/delete")]
        public async Task<IActionResult> DeleteStudentAsync(string studentId)
        {
            await _studentService.DeleteStudentAsync(studentId);
            return NoContent();
        }

        #endregion

        #region Note Endpoints

        // Tworzenie nowej notatki dla studenta
        [HttpPost("{studentId}/notes")]
        public async Task<ActionResult> CreateNoteAsync([FromRoute] string studentId, [FromBody] CreateNoteDto dto)
        {
            await _noteService.CreateNote(studentId, dto);
            return Created();
        }

        // Pobieranie listy notatek przypisanych do studenta
        [HttpGet("{studentId}/notes")]
        public async Task<ActionResult<List<Note>>> GetNotesByStudentIdAsync([FromRoute] string studentId)
        {
            var notes = await _noteService.GetListOfNotes(studentId);
            return Ok(notes);
        }

        // Pobieranie konkretnej notatki po ID
        [HttpGet("{studentId}/notes/{noteId}")]
        public async Task<ActionResult<Note>> GetNoteByIdAsync([FromRoute] string studentId, [FromRoute] string noteId)
        {
            var note = await _noteService.GetNoteById(studentId, noteId);
            return Ok(note);
        }

        // Aktualizacja konkretnej notatki
        [HttpPatch("{studentId}/notes/{noteId}")]
        public async Task<ActionResult> UpdateNoteAsync([FromRoute] string studentId, [FromRoute] string noteId, [FromBody] string content)
        {
            await _noteService.UpdateNote(noteId, content);
            return NoContent();
        }

        // Usuwanie konkretnej notatki
        [HttpDelete("{studentId}/notes/{noteId}")]
        public async Task<ActionResult> DeleteNoteAsync([FromRoute] string studentId, [FromRoute] string noteId)
        {
            await _noteService.DeleteNote(noteId);
            return NoContent();
        }

        #endregion

        #region Lesson Endpoints

        // Pobieranie listy lekcji przypisanych do studenta
        [HttpGet("{studentId}/lessons")]
        public async Task<ActionResult<PageResult<LessonListByStudentIdDto>>> GetLessonsByStudentId([FromRoute] string studentId, [FromQuery] SearchQuery searchQuery)
        {
            var result = await _studentService.GetLessonsByStudentId(studentId, searchQuery);
            return Ok(result);
        }

        #endregion
    }
}
