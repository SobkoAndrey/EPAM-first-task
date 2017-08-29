using System;
using System.Web.Mvc;
using SimpleCRUD2.Interfaces;
using SimpleCRUD2.Models;
using SimpleCRUD2.Models.ViewModels;
using SimpleCRUD2.Models.ViewModels.HomeViewModels;

namespace SimpleCRUD2.Controllers
{
    [Authorize(Roles = "user")]
    public class HomeController : Controller
    {
        private IUserRepository repository;
        const int DefaultPageSize=5;

        public HomeController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult Index(int pageNumber = 1)
        {
            //var pageSize = DefaultPageSize; 

            var users = this.repository.GetUsersListForPage(pageNumber, DefaultPageSize);

            var pageInfo = new PageInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = this.repository.UsersCount,
                ControllerName = "Home",
                ActionName = "Index"
            };

            var viewModel = new IndexViewModel { PageInfo = pageInfo, Users = users };

            return this.View("Index", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddUser()
        {
            return this.View("AddUser");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(UserModel userModel)
        {
            if (ModelState.IsValid && this.repository.AddUser(userModel))
            {
                return this.RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Email's been registred");

            return this.View("AddUser", userModel);
        }

        [HttpGet]
        [Authorize(Roles = "admin, moder")]
        [HandleError(ExceptionType = typeof(ArgumentException), View = "UserNotExistError")]
        public ActionResult EditUserInfo(int id)
        {
            var userModel = this.repository.GetUserById(id);

            if (userModel == null)
            {
                return this.View("UserNotExistError");
            }
            else if (!User.IsInRole("admin") && this.repository.IsUserInRole(userModel.Email, "admin"))
            {
                return this.View("NotEnoughRightsError");
            }
            else
            {
                var editUserViewModel = new EditUserViewModel(userModel);
                return this.View("EditUserInfo", editUserViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moder")]
        public ActionResult EditUserInfo(EditUserViewModel editUserViewModel)
        {
            if (ModelState.IsValid)
            {
                this.repository.EditUserInfo(editUserViewModel);

                return this.RedirectToAction("Index");
            }

            return this.View("EditUserInfo", editUserViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "admin, moder")]
        [HandleError(ExceptionType = typeof(ArgumentException), View = "UserNotExistError")]
        public ActionResult DeleteUser(int id)
        {
            var userModel = this.repository.GetUserById(id);

            if (userModel == null)
            {
                return this.View("UserNotExistError");
            }
            else if (!User.IsInRole("admin") && this.repository.IsUserInRole(userModel.Email, "admin"))
            {
                return this.View("NotEnoughRightsError");
            }
            else
            {
                return this.View("DeleteUser", userModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moder")]
        public ActionResult DeleteUserById(int id)
        {
            var userModel = this.repository.GetUserById(id);

            if (userModel == null)
            {
                return this.View("UserNotExistError");
            }
            else if (!User.IsInRole("admin") && this.repository.IsUserInRole(userModel.Email, "admin"))
            {
                return this.View("NotEnoughRightsError");
            }
            else
            {
                this.repository.DeleteUserById(id);
                return this.RedirectToAction("Index");
            }
        }
    }
}
