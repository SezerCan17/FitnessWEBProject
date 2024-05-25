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
        public string StatusMessage { get; set; }
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserFitnessWebDatabaseContext _context;

        public List<TblTodo> FavoriteChallenges { get; set; }
        
       [BindProperty]
        public BufferedSingleFileUploadDb FileUpload { get; set; } = new BufferedSingleFileUploadDb();
            
        public byte[]? Picture { get; set; }
        public UserDetail? ProfileDetail { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "Bio")]
            public string? Bio { get; set; }
        }

        public class BufferedSingleFileUploadDb
        {
            [Display(Name = "Profile Picture")]
            public IFormFile? FormFile { get; set; }
        }

        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public string SelectedCity { get; set; } = string.Empty;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            UserFitnessWebDatabaseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

        private async Task LoadAsync(IdentityUser user)
{
    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

    ProfileDetail = await _context.UserDetails
        .FirstOrDefaultAsync(p => p.UserId == user.Id);

    FavoriteChallenges = GetFavoriteChallenges(user.Id);

    var cities = await _context.TblCities.ToListAsync();
    Cities = cities.Select(c => new SelectListItem
    {
        Value = c.City,
        Text = c.City
    }).ToList();

    if (ProfileDetail != null)
    {
        Picture = ProfileDetail.Photo ?? Array.Empty<byte>();
        SelectedCity = ProfileDetail.City;
        Input.Bio = ProfileDetail.bio;
    }
    else
    {
        string path = "./wwwroot/images/empty_profile.jpg";
        using var stream = System.IO.File.OpenRead(path);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        Picture = memoryStream.ToArray();
        ProfileDetail = new UserDetail
        {
            UserId = user.Id,
            Photo = Picture,
            City = string.Empty
        };
        _context.UserDetails.Add(ProfileDetail);
        await _context.SaveChangesAsync();
    }

    Input = new InputModel
    {
        PhoneNumber = phoneNumber,
        Bio = ProfileDetail.bio
    };
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
            return RedirectToPage();
        }
    }

    ProfileDetail = await _context.UserDetails.FirstOrDefaultAsync(p => p.UserId == user.Id);

    if (ProfileDetail != null)
    {
        if (SelectedCity != ProfileDetail.City)
        {
            ProfileDetail.City = SelectedCity;
        }

        if (FileUpload.FormFile != null)
        {
            var fileExtension = Path.GetExtension(FileUpload.FormFile.FileName).ToLowerInvariant();
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
            {
                using var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                ProfileDetail.Photo = memoryStream.ToArray();
            }
            else
            {
                ModelState.AddModelError("FileUpload.FormFile", "Yalnızca jpg, jpeg ve png formatlarında dosya yükleyebilirsiniz.");
                await LoadAsync(user);
                return Page();
            }
        }

        if (Input.Bio != null && Input.Bio != ProfileDetail.bio)
        {
            ProfileDetail.bio = Input.Bio;
        }

        _context.UserDetails.Update(ProfileDetail);
        await _context.SaveChangesAsync();
    }

    await _signInManager.RefreshSignInAsync(user);
    return RedirectToPage();
}


        public List<TblTodo> GetFavoriteChallenges(string userId)
        {
            var user = _context.UserDetails.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                var favoriteChallengeIds = user.favorite?.Split(',').Select(int.Parse).ToList();
                if (favoriteChallengeIds != null && favoriteChallengeIds.Any())
                {
                    return _context.TblTodos
                        .Where(t => favoriteChallengeIds.Contains(t.Id) && !t.IsDeleted)
                        .ToList();
                }
            }
            return new List<TblTodo>();
        }
    }
}
