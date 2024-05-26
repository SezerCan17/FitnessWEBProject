using System.Security.Claims;
using System.Linq;
using loginDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    [Authorize]
    public class RateModel : PageModel
    {
        private readonly UserFitnessWebDatabaseContext _context;

        public RateModel(UserFitnessWebDatabaseContext context)
        {
            _context = context;
        }
 public int Id { get; set; }

        public void OnGet(int id) 
        {
            Id = id; 
        }

        public IActionResult OnPostRate(int id, int _rate,string commentText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && id != 0 && _rate != 0)
        {
            
            var existingVote = _context.UserRates
                .FirstOrDefault(r => r.UserId == userId && r.TodoId == id);
if (existingVote == null)
            {
                
                var rating = new UserRate
                {
                    UserId = userId,
                    Rate = (short?)_rate,
                    TodoId = id
                };
                rating.Comment = commentText; 
                _context.UserRates.Add(rating);
                _context.SaveChanges();
                return RedirectToPage("/UserRates");
            }
            else
{
   
    return Content("<script>alert('You Already Rated!'); window.history.go(-2);</script>", "text/html");
}



        }
        return Page();
        }
    }
}