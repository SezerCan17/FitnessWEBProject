using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using loginDemo.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace MyApp.Namespace
{
    [Authorize(Roles = "Admin")]
    public class ToDoModel : PageModel
    {
        [BindProperty]
        public TblTodo NewToDo { get; set; } = default!;

        public UserFitnessWebDatabaseContext ToDoDb = new();

        public List<TblTodo> ToDoList { get; set; } = default!;

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
           
            ToDoList = (from item in ToDoDb.TblTodos
                        where item.IsDeleted == false
                        where item.UserId == userId
                        select item).ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || NewToDo == null)
            {
                return Page();
            }
            NewToDo.IsDeleted = false;
            NewToDo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ToDoDb.Add(NewToDo);
            ToDoDb.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (ToDoDb.TblTodos != null)
            {
                var todo = ToDoDb.TblTodos.Find(id);
                if (todo != null)
                {
                    todo.IsDeleted = true;
                    ToDoDb.SaveChanges();
                }
            }

            return RedirectToPage();
        }
    }
}