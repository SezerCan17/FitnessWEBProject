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
 public int Id { get; set; } // Id özelliğini ekleyin

        public void OnGet(int id) // OnGet metodu Id parametresini alacak şekilde güncellendi
        {
            Id = id; // Id özelliğini ayarlayın
        }

        public IActionResult OnPostRate(int id, int _rate,string commentText)
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
                rating.Comment = commentText; // Yorumu ekleyin
                _context.UserRates.Add(rating);
                _context.SaveChanges();
                return RedirectToPage("/UserRates");
            }
            else
{
    // JavaScript kodu çalıştırarak bir uyarı göster ve iki önceki sayfaya geri dön
    return Content("<script>alert('Zaten oy verdiniz!'); window.history.go(-2);</script>", "text/html");
}



        }
        return Page();
        }
    }
}