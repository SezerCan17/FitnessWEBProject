using System.Security.Claims;
using loginDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    [Authorize]
    public class UserRatesModel : PageModel
    {
        [BindProperty]
        public TblTodo NewToDo { get; set; } = default!;

        public UserFitnessWebDatabaseContext ToDoDb = new();

        public List<TblTodo> ToDoList { get;set; } = default!;

        public List<AverageRating> AverageRatings { get;set; } = default!;

        public List<float> Ratings { get;set;} = default!;

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the logged-in user's userId

            // LINQ query to retrieve items where IsDeleted is false and other users' ToDos
            ToDoList = (from item in ToDoDb.TblTodos
                          where item.IsDeleted == false
                          where item.UserId != userId
                          select item).ToList();

            // Calculate average rating for each product
            AverageRatings = (from todo in ToDoDb.TblTodos
                                 join rate in ToDoDb.UserRates on todo.Id equals rate.TodoId into gj
                                 from subRate in gj.DefaultIfEmpty()
                                 group subRate by todo into g
                                 select new AverageRating
                                 {
                                    TodoId = g.Key.Id,
                                    Average = g.Any() ? (float)g.Average(r => r.Rate ?? 0) : 0
                                }).ToList();

             Ratings = AverageRatings.Select(x => x.Average).ToList();
        }
    }
 }