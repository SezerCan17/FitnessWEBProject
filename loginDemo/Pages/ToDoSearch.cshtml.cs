using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using loginDemo.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Namespace
{
    public class ToDoSearchModel : PageModel
    {
        private readonly UserFitnessWebDatabaseContext _context;

        public ToDoSearchModel(UserFitnessWebDatabaseContext context)
        {
            _context = context;
        }

        public IList<TblTodo> ToDoList { get; set; } = new List<TblTodo>();

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Difficulty { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Period { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        public void OnGet()
{
    var query = _context.TblTodos.Where(t => !t.IsDeleted).AsQueryable();

    var selectedProperties = new List<string> { Difficulty, Category, Period };
    selectedProperties.RemoveAll(string.IsNullOrEmpty);

    if (selectedProperties.Count == 2)
    {
        query = query.Where(t =>
            (string.IsNullOrEmpty(Difficulty) || t.Difficulty == Difficulty) &&
            (string.IsNullOrEmpty(Category) || t.Category == Category) &&
            (string.IsNullOrEmpty(Period) || t.Period == Period)
        );
    }
    else
    {
        if (!string.IsNullOrEmpty(SearchKeyword))
        {
            query = query.Where(t => t.Instruction.Contains(SearchKeyword));
        }

        if (!string.IsNullOrEmpty(Category))
        {
            query = query.Where(t => t.Category == Category);
        }

        if (!string.IsNullOrEmpty(Period))
        {
            query = query.Where(t => t.Period == Period);
        }

        if (!string.IsNullOrEmpty(Difficulty))
        {
            query = query.Where(t => t.Difficulty == Difficulty);
        }
    }

    switch (SortOrder)
    {
        case "category_asc":
            query = query.OrderBy(t => t.Category);
            break;
        case "category_desc":
            query = query.OrderByDescending(t => t.Category);
            break;
        case "difficulty_asc":
            query = query.OrderBy(t => t.Difficulty);
            break;
        case "difficulty_desc":
            query = query.OrderByDescending(t => t.Difficulty);
            break;
        default:
            break;
    }

    ToDoList = query.ToList();
}
    }
}
