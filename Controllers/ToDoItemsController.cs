using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAuthApi.Data;
using TodoAuthApi.Models;

namespace TodoAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece Token sahipleri girebilir
    public class ToDoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToDoItemsController(AppDbContext context)
        {
            _context = context;
        }

        // Kendi görevlerimi listele
        [HttpGet]
        public IActionResult GetMyTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var tasks = _context.ToDoItems.Where(t => t.UserId == userId).ToList();
            return Ok(tasks);
        }

        // Yeni görev ekle
        [HttpPost]
        public IActionResult CreateTask(ToDoItem task)
        {
            // Token'dan gelen User ID'yi göreve atıyoruz
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            task.UserId = userId;
            
            _context.ToDoItems.Add(task);
            _context.SaveChanges();
            
            return Ok(task);
        }

        // Görevi Sil
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var task = _context.ToDoItems.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            
            if (task == null) return NotFound("Görev bulunamadı.");
            
            _context.ToDoItems.Remove(task);
            _context.SaveChanges();
            return Ok("Görev silindi.");
        }

        // Görevi Güncelle (Tamamlandı/Tamamlanmadı)
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, ToDoItem updatedTask)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var task = _context.ToDoItems.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            
            if (task == null) return NotFound("Görev bulunamadı.");
            
            task.IsCompleted = updatedTask.IsCompleted;
            _context.SaveChanges();
            return Ok(task);
        }
    }
}