using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoContext context;

        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string id)
        {
            var filters = new Filters(id);
            ViewBag.Filters = filters;
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo.Models.ToDo> query = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Where(t => !t.IsDeleted);

            if (filters.HasCategory && int.TryParse(filters.CategoryId, out int categoryId))
            {
                query = query.Where(t => t.CategoryId == categoryId);
            }

            if (filters.HasStatus && int.TryParse(filters.StatusId, out int statusId))
            {
                query = query.Where(t => t.StatusId == statusId);
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;

                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var task = new ToDo.Models.ToDo { StatusId = 1 };
            return View(task);
        }

        [HttpPost]
        public IActionResult Add(ToDo.Models.ToDo task)
        {
            if (ModelState.IsValid)
            {
                task.CreatedAt = DateTime.Now; // to track when task was added
                context.ToDos.Add(task);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            return View(task);
        }

        [HttpPost]
        public IActionResult Filter(string categoryFilter, string statusFilter, string dueFilter)
        {
            // Correct order: category - due - status
            string[] filters = new string[]
            {
        string.IsNullOrEmpty(categoryFilter) ? "all" : categoryFilter,
        string.IsNullOrEmpty(dueFilter) ? "all" : dueFilter,
        string.IsNullOrEmpty(statusFilter) ? "all" : statusFilter
            };

            string id = string.Join('-', filters);

            return RedirectToAction("Index", new { id });
        }


        [HttpPost]
        public IActionResult MarkComplete(int id, string? filterId)
        {
            var task = context.ToDos.Find(id);
            if (task != null)
            {
                // Use StatusId directly instead of matching string name
                int completedStatusId = context.Statuses.FirstOrDefault(s => s.Name.ToLower() == "completed")?.StatusId ?? 2;

                task.StatusId = completedStatusId;
                task.CompletedAt = DateTime.Now; // Track completion time
                context.SaveChanges();
            }

            // Fix redirect parameter (must be lowercase 'id')
            return RedirectToAction("Index", new { id = filterId });
        }


        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = context.ToDos.Where(t => t.StatusId == 2 && !t.IsDeleted).ToList();

            foreach (var task in toDelete)
            {
                task.IsDeleted = true;
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult UpdateDueDate(int id, DateTime dueDate)
        {
            var task = context.ToDos.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
            if (task != null)
            {
                task.DueDate = dueDate;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult AllTasks()
        {
            var tasks = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status)
                .ToList();
            return Json(tasks);
        }

        public IActionResult DeletedTasks()
        {
            var deletedTasks = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Where(t => t.IsDeleted)
                .OrderBy(t => t.DueDate)
                .ToList();

            return View(deletedTasks);
        }

        [HttpPost]
        public IActionResult RestoreTask(int id)
        {
            var task = context.ToDos.Find(id);
            if (task != null && task.IsDeleted)
            {
                task.IsDeleted = false;
                context.SaveChanges();
            }
            return RedirectToAction("DeletedTasks");
        }

        [HttpPost]
        public IActionResult PermanentDelete(int id)
        {
            var task = context.ToDos.Find(id);
            if (task != null && task.IsDeleted)
            {
                context.ToDos.Remove(task);
                context.SaveChanges();
            }
            return RedirectToAction("DeletedTasks");
        }
    }
}
