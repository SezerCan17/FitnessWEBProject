using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using loginDemo.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Namespace
{
    [Authorize] // Yetkilendirme belirtilmediğinde tüm yetkilendirilmiş kullanıcılara izin verir
    public class UserRatesModel : PageModel
    {
        private readonly UserFitnessWebDatabaseContext _context;

        public UserRatesModel(UserFitnessWebDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblTodo NewToDo { get; set; }

        public List<TblTodo> ToDoList { get; set; }

        public List<AverageRating> AverageRatings { get; set; }

        public List<float> Ratings { get; set; }

        public void OnGet()
        {
             // Retrieve all ToDos
        ToDoList = _context.TblTodos.Where(item => item.IsDeleted == false).ToList();

        // Calculate average ratings for each ToDo
        AverageRatings = (from todo in _context.TblTodos
                          join rate in _context.UserRates on todo.Id equals rate.TodoId into gj
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
