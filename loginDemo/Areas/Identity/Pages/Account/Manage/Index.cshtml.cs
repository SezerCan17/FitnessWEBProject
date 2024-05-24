using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using loginDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace loginDemo.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserFitnessWebDatabaseContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            UserFitnessWebDatabaseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public BufferedSingleFileUploadDb FileUpload { get; set; } = new BufferedSingleFileUploadDb();

        public byte[]? Picture { get; set; }
        public UserDetail? ProfileDetail { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Bio")]
            public string? Bio { get; set; }  // Yeni bio alanı
        }

        public class BufferedSingleFileUploadDb
        {
            [Display(Name = "Profile Picture")]
            public IFormFile? FormFile { get; set; }
        }

        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public string SelectedCity { get; set; } = string.Empty;
private async Task LoadAsync(IdentityUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


            // Fetch user profile details
            ProfileDetail = await _context.UserDetails
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            // Fetch cities from the database
            var cities = await _context.TblCities.ToListAsync();
            Cities = cities.Select(c => new SelectListItem
            {
                Value = c.City,
                Text = c.City
            }).ToList();

            // Set properties based on the user profile
            if (ProfileDetail != null)
            {
                Picture = ProfileDetail.Photo ?? Array.Empty<byte>();
                SelectedCity = ProfileDetail.City;
                Input.Bio = ProfileDetail.bio;
            }
            else
            {
                // Provide a default picture if no profile picture is available
                string path = "./wwwroot/images/empty_profile.jpg";
                using var stream = System.IO.File.OpenRead(path);
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                Picture = memoryStream.ToArray();
// Create a new profile detail entry
                ProfileDetail = new UserDetail
                {
                    UserId = user.Id,
                    Photo = Picture,
                    City = string.Empty
                };
                _context.UserDetails.Add(ProfileDetail);
                await _context.SaveChangesAsync();
            }

            // Set input model properties
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Bio = ProfileDetail.bio // Ekleme
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }
public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            ProfileDetail = await _context.UserDetails.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (ProfileDetail != null)
            {
                // Update the city
                if (SelectedCity != ProfileDetail.City)
                {
                    ProfileDetail.City = SelectedCity;
                }
// Update the profile photo if a new file is uploaded
                if (FileUpload.FormFile != null)
                {
                    var memoryStream = new MemoryStream();
                    await FileUpload.FormFile.CopyToAsync(memoryStream);
                    ProfileDetail.Photo = memoryStream.ToArray();
                }

                // Update the bio if it has changed
                if (Input.Bio != null && Input.Bio != ProfileDetail.bio)
                {
                    ProfileDetail.bio = Input.Bio;
                }

                _context.UserDetails.Update(ProfileDetail);
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}