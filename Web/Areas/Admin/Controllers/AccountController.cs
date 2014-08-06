using MarkdownBlog.Net.Web.Models.UserViewModel;
using MarkdownBlog.Net.Web.Services;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Areas.Admin.Controllers {
    public class AccountController : Controller {
        private readonly IFormsAuthenticationService authenticationService;
        private readonly IUserService userService;
        private readonly IFormsIdentityService identityService;

        public AccountController(IUserService userService, IFormsAuthenticationService authenticationService,
                                 IFormsIdentityService identityService) {
            this.userService = userService;
            this.authenticationService = authenticationService;
            this.identityService = identityService;
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model) {
            if (ModelState.IsValid) {
                if (userService.ValidateUser(model.Username, model.Password)) {
                    authenticationService.SignIn(model.Username, false);

                    if (ReturnUrlIsValid(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    ModelState.AddModelError("Username", Models.UserViewModel.User.ValidationMessageUsernameOrPasswordIncorrect);
                }
            }

            return View("Login", model);
        }

        [HttpGet]
        public RedirectToRouteResult Logout() {
            authenticationService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ChangePassword() {
            if (!identityService.IsAuthenticated())
                return RedirectToAction("Login");

            return View("ChangePassword");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordInput input) {
            if (!identityService.IsAuthenticated())
                return RedirectToAction("Login");

            if (ModelState.IsValid) {
                if (userService.ChangePassword(identityService.Username, input.OldPassword, input.NewPassword)) {
                    return View("ChangePasswordSuccess");
                }
                else {
                    ModelState.AddModelError("", ChangePasswordInput.ValidationErrorChangePasswordFailed);
                }
            }

            // If we got this far, something failed, redisplay form
            return View("ChangePassword", input);
        }

        private bool ReturnUrlIsValid(string returnUrl) {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl);
        }
    }


}
