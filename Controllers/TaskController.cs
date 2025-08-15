//using Microsoft.AspNetCore.Mvc;
//using WebApplication1.Models;
//using WebApplication1.Services;

//namespace WebApplication1.Controllers
//{
//    public class TaskController : Controller
//    {
//        private readonly JsonTaskRepository _repo = new JsonTaskRepository();

//        public IActionResult Index()
//        {
//            var tasks = _repo.LoadTasks();
//            ViewBag.Tasks = tasks;
//            return View(new RozkStr()); // порожня модель для форми

//        }

//        [HttpPost]
//        public IActionResult Add(RozkStr task)
//        {
//            Console.WriteLine("Add() викликано!");
//            Console.WriteLine($"Title: {task.Title}, Description: {task.Description}, IsCompleted: {task.IsCompleted}");
//            Console.WriteLine("ModelState: " + (ModelState.IsValid ? "Valid" : "Invalid"));

//            if (!ModelState.IsValid)
//            {
//                Console.WriteLine("Модель невалідна. Повертаємо форму з помилками.");
//                var tasks = _repo.LoadTasks(); // щоб не втратити список
//                ViewBag.Tasks = tasks;
//                return View("Index", task); // повертаємо знову форму з помилками
//            }

//            var existingTasks = _repo.LoadTasks();
//            existingTasks.Add(task);
//            _repo.SaveTasks(existingTasks);
//            Console.WriteLine("✅ Задача додана: " + task.Title);
//            return RedirectToAction("Index");
//        }


//        [HttpPost]
//        public IActionResult ToggleComplete(Guid id)
//        {
//            var tasks = _repo.LoadTasks();
//            var task = tasks.FirstOrDefault(t => t.Id == id);
//            if (task != null)
//            {
//                task.IsCompleted = !task.IsCompleted;
//                _repo.SaveTasks(tasks);
//            }
//            return RedirectToAction("Index");
//        }

//        [HttpPost]
//        public IActionResult Delete(Guid id)
//        {
//            var tasks = _repo.LoadTasks();
//            tasks.RemoveAll(t => t.Id == id);
//            _repo.SaveTasks(tasks);
//            return RedirectToAction("Index");
//        }

//        [HttpPost]
//        public IActionResult Edit(Guid Id, string Title, string Description)
//        {
//            var tasks = _repo.LoadTasks();
//            var task = tasks.FirstOrDefault(t => t.Id == Id);
//            if (task == null) return NotFound();

//            task.Title = Title;
//            task.Description = Description;
//            _repo.SaveTasks(tasks);

//            return RedirectToAction("Index");
//        }
//    }
//}
