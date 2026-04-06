// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Demo.DataAccess.IRepository;
using Demo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Demo.Utility;

namespace Demo.Main.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserStore<IdentityUser> userStore;
        private readonly IUserEmailStore<IdentityUser> emailStore;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IUnitOfWork unitOfWork;

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            [Required] public string FirstName { get; set; }
            [Required] public string LastName { get; set; }
            public string? StreetAddress { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? PostalCode { get; set; }
            public string? PhoneNumber { get; set; }
            public int? CompanyId { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> CompanyList { get; set; }

        }

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userStore = userStore;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.unitOfWork = unitOfWork;
            emailStore = GetEmailStore();
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            if(!roleManager.RoleExistsAsync(SD.ROLE_USER_CUSTOMER).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_CUSTOMER)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_COMPANY)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_ADMIN)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_EMPLOYEE)).GetAwaiter().GetResult();
            }

            Input = new()
            {
                RoleList = roleManager.Roles
                    .Select(x => x.Name)
                    .Select(i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    }),
                CompanyList = unitOfWork.CompanyRepository.GetAll()
                    .Select(e => e.Name)
                    .Select(name => new SelectListItem
                    {
                        Text = name,
                        Value = name
                    })
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.StreetAddress = Input.StreetAddress;
                user.City = Input.City;
                user.PostalCode = Input.PostalCode;
                user.State = Input.State;
                user.PhoneNumber = Input.PhoneNumber;


                var userRole = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if(userRole != null)
                {
                    userManager.RemoveFromRoleAsync(user, userRole).GetAwaiter().GetResult();
                }
                if(Input.Role != null)
                {
                    userManager.AddToRoleAsync(user, Input.Role).GetAwaiter().GetResult();
                    if(Input.Role.EqualsAny(SD.ROLE_USER_COMPANY, SD.ROLE_USER_EMPLOYEE))
                    {
                        user.CompanyId = Input.CompanyId;
                    }
                }

                var result = await userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation("User created a new account with password.");

                    var role = !string.IsNullOrEmpty(Input.Role) ? Input.Role : SD.ROLE_USER_CUSTOMER;
                    await userManager.AddToRoleAsync(user, role);

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if(User.IsInRole(SD.ROLE_USER_ADMIN))
                        {
                            ControllerUtility.AddSuccessFeedback(TempData, "created", objName: "New User");
                        }
                        else
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                        }
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)userStore;
        }
    }
}

