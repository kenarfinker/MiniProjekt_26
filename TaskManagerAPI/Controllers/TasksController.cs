using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        // Zwraca zadania przypisane do aktualnego użytkownika
        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Where(t => t.AssignedUserId == userId)
                .ToListAsync();

            return Ok(tasks);
        }

        // POST: api/tasks
        // Tworzy zadanie w projekcie (tylko właściciel projektu)
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskItem task)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == task.ProjectId);

            if (project == null)
                return NotFound("Project not found");

            if (project.OwnerId != userId)
                return Forbid();

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            // Edytować może:
            // - właściciel projektu
            // - użytkownik przypisany do zadania
            if (task.Project.OwnerId != userId &&
                task.AssignedUserId != userId)
                return Forbid();

            task.Title = updatedTask.Title;
            task.IsCompleted = updatedTask.IsCompleted;
            task.AssignedUserId = updatedTask.AssignedUserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            if (task.Project.OwnerId != userId)
                return Forbid();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}