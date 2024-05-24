using System.Security.Claims;
using loginDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    [Authorize]
    public class RateModel : PageModel
    {
        public UserFitnessWebDatabaseContext ToDoDb = new();

        bool flag = false;

        public void OnGet()
        {

        }

        public void OnPostRate(int id, int _rate)
        {
            var rating = new UserRate();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the logged-in user's userId
            if (userId!=null && id!=0 && _rate!=0)
            {
                rating.UserId = userId;
                rating.Rate = (short?)_rate;
                rating.TodoId = id;
                ToDoDb.UserRates.Add(rating);
                ToDoDb.SaveChanges();
                flag=true;
            }
            if (flag)
            {
                Response.Redirect("/UserRates"); 
            }
            else{
                Page();
            }
        }
    }
}