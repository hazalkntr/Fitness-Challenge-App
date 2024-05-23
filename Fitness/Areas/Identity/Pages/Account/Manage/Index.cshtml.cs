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

namespace Fitness.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly FitnessChallengeContext _context;
        
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            FitnessChallengeContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        
        [BindProperty]
        public BufferedSingleFileUploadDb FileUpload { get; set; } = new BufferedSingleFileUploadDb();
        public byte[]? Picture { get; set; }
        public UserDetail? ProfileDetail { get; set; }

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

            ProfileDetail = _context.UserDetails.Where(p => p.UserId == user.Id).FirstOrDefault();
            if (ProfileDetail != null && ProfileDetail.Photo != null)
            {
                Picture = ProfileDetail.Photo;
            }
            else
            {
                // Save a default image if no profile photo is available
                string path = "./wwwroot/images/empty_profile.jpg";
                using var stream = System.IO.File.OpenRead(path);
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                Picture = memoryStream.ToArray();
                ProfileDetail = new UserDetail
                {
                    Photo = Picture,
                    UserId = user.Id
                };
                _context.UserDetails.Add(ProfileDetail);
                await _context.SaveChangesAsync();
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

            ProfileDetail = _context.UserDetails.Where(p => p.UserId == user.Id).FirstOrDefault();

            if (FileUpload.FormFile != null)
            {
                var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                if (ProfileDetail != null)
                {
                    ProfileDetail.Photo = memoryStream.ToArray();
                    _context.UserDetails.Update(ProfileDetail);
                }
            }
                await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
