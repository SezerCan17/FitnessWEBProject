using System.Collections.Generic;
using System.Linq;
using loginDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace loginDemo.Pages
{
    public class FavoriteModel : PageModel
    {
        private readonly UserFitnessWebDatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoriteModel(UserFitnessWebDatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<TblTodo> ToDoList { get; set; }

        public List<TblTodo> GetFavoriteChallenges(string userId)
        {
            var user = _context.UserDetails.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                var favoriteChallengeIds = user.favorite?.Split(',').Select(int.Parse).ToList();
                if (favoriteChallengeIds != null && favoriteChallengeIds.Any())
                {
                    return _context.TblTodos
                        .Where(t => favoriteChallengeIds.Contains(t.Id) && !t.IsDeleted) // !IsDeleted ise favorilere eklenir
                        .ToList();
                }
            }
            return new List<TblTodo>();
        }

        public async void OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Admin kullanıcıysa tüm meydan okumaları al
                ToDoList = _context.TblTodos.ToList();
            }
            else
            {
                // Normal kullanıcıysa sadece silinmemiş meydan okumaları al
                ToDoList = _context.TblTodos.Where(t => !t.IsDeleted).ToList();
            }
        }

        public IActionResult OnPost(int id)
        {
            var challenge = _context.TblTodos.FirstOrDefault(t => t.Id == id);

            if (challenge != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.UserDetails.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.favorite))
                        user.favorite = challenge.Id.ToString();
                    else
                        user.favorite += "," + challenge.Id.ToString();

                    _context.SaveChanges();
                }
            }

            return RedirectToPage("Index");
        }
    }
}
