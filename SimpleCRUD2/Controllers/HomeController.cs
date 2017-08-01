﻿using System;
using System.Linq;
using System.Web.Mvc;
using SimpleCRUD2.Interfaces;
using SimpleCRUD2.Models;
using SimpleCRUD2.Models.ViewModels;

namespace SimpleCRUD2.Controllers
{
    [HandleError(ExceptionType = typeof(Exception), View = "Error")]
    public class HomeController : Controller
    {
        private IUserRepository repository;

        public HomeController(IUserRepository repository)
        {
            this.repository = repository;
        }
        
        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            var users = this.repository.GetUsersList();

            var pageSize = 5;

            var usersPerPage = users.Skip((page - 1) * pageSize).Take(pageSize);

            var pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = users.Count() };

            var viewModel = new IndexViewModel { PageInfo = pageInfo, Users = usersPerPage };

            return this.View(viewModel);
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                this.repository.AddUser(userModel);

                return this.RedirectToAction("Index");
            }

            return this.View(userModel);
        }

        [HttpGet]
        [HandleError(ExceptionType = typeof(NullReferenceException), View = "UserNotExistError")]
        public ActionResult EditUserInfo(int id)
        {
            var user = this.repository.GetUserById(id);
            return this.View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserInfo(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                this.repository.EditUserInfo(userModel);

                return this.RedirectToAction("Index");
            }

            return this.View(userModel);
        }

        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            return this.View(this.repository.GetUserById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserById(int id)
        {
            this.repository.DeleteUserById(id);

            return this.RedirectToAction("Index");
        }
    }
}