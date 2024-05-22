// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Fitness.Pages;
using System.Collections.Generic;


namespace Fitness.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly FitnessChallengeContext _context;
        private readonly LoginTermProjectDbContext _context2;
 
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            FitnessChallengeContext context,
            LoginTermProjectDbContext context2)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _context2 = context2;
        }

        [BindProperty]
        public BufferedSingleFileUploadDb FileUpload { get; set; } = new BufferedSingleFileUploadDb();

        public byte[]? Picture { get; set; }
        public User? UserDetail { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }
        
        public class BufferedSingleFileUploadDb
        {
            [Display(Name = "Profile Picture")]
            public IFormFile? FormFile { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            int userId;
            if (int.TryParse(user.Id, out userId))
            {

                UserDetail = _context2.Users.Where(p => p.UserId == userId).FirstOrDefault();
                if (UserDetail != null && UserDetail.Photo != null)
                {
                    Picture = UserDetail.Photo;
                }
                else
                {
                    // Save a default image if no profile photo is available
                    string path = "./wwwroot/images/empty_profile.jpg";
                    using var stream = System.IO.File.OpenRead(path);
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    Picture = memoryStream.ToArray();
                    UserDetail = new User
                    {
                        Photo = Picture,
                        UserId = userId,
                    };
                    _context2.Users.Add(UserDetail);
                    await _context2.SaveChangesAsync();
                }
            }    
            else
            {
                throw new ArgumentException("Invalid user ID");
            }
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

            await _signInManager.RefreshSignInAsync(user);

            int userId;
            if (int.TryParse(user.Id, out userId))
            {
                UserDetail = _context2.Users.Where(p => p.UserId == userId).FirstOrDefault();

                if (FileUpload.FormFile != null)
                {
                    var memoryStream = new MemoryStream();
                    await FileUpload.FormFile.CopyToAsync(memoryStream);
                    if (UserDetail != null)
                    {
                        UserDetail.Photo = memoryStream.ToArray();
                        _context2.Users.Update(UserDetail);
                    }
                }
                    await _context2.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid user ID");
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
