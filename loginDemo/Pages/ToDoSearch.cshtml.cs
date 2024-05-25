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

    // Kullanıcı zorluk seviyesini belirtti mi?
    bool difficultySpecified = !string.IsNullOrEmpty(Difficulty);

    // Kullanıcı zorluk seviyesini belirttiyse, diğer filtreleme kriterlerine bakmaksızın tüm meydan okumalarını getirin
    if (difficultySpecified)
    {
        query = query.Where(t => t.Difficulty == Difficulty);
    }
    else // Kullanıcı zorluk seviyesini belirtmemişse, mevcut filtreleme mantığına devam edin
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
    }
    // Sorting
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