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

        public void OnGet()
        {

        }

        public IActionResult OnPostRate(int id, int _rate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && id != 0 && _rate != 0)
        {
            // Kullanıcının bu challenge için zaten oy verip vermediğini kontrol edin
            var existingVote = _context.UserRates
                .FirstOrDefault(r => r.UserId == userId && r.TodoId == id);

            if (existingVote == null)
            {
                // Kullanıcı henüz oy kullanmadıysa, oy kullanın
                var rating = new UserRate
                {
                    UserId = userId,
                    Rate = (short?)_rate,
                    TodoId = id
                };

                _context.UserRates.Add(rating);
                _context.SaveChanges();
                return RedirectToPage("/UserRates");
            }
            else
            {
                return Page();
            }
        }
        return Page();
        }
    }
}
