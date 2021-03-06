﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SimpleCRUD2.Controllers;
using SimpleCRUD2.Models;
using SimpleCRUD2.Models.ViewModels.CourseViewModels;
using SimpleCRUD2.Test.Mocks;
using SimpleCRUD2.XmlWork;

namespace SimpleCRUD2.Test.Controllers
{
    [TestFixture]
    public class CourseControllerTest
    {
        private CourseController controller;

        [SetUp]
        public void Initialize()
        {
            this.controller = new CourseController(new CourseRepositoryMock(), new XmlLinqProcessorMock());
        }

        [Test]
        public void CreateCourse_ReturnCorrectViewIfModelIsNotValid()
        {
            var courseModel = new CourseModel();

            this.controller.ModelState.AddModelError(string.Empty, string.Empty);

            ViewResult result = this.controller.CreateCourse(courseModel) as ViewResult;

            Assert.AreEqual("CreateCourse", result.ViewName);
        }

        [Test]
        public void CreateCourse_ReturnCorrectViewIfCourseNameWasUsed()
        {
            var courseModel = new CourseModel();

            ViewResult result = this.controller.CreateCourse(courseModel) as ViewResult;

            Assert.AreEqual("CreateCourse", result.ViewName);
        }

        [Test]
        public void CreateCourse_ReturnCorrectViewIfCourseWasCreated()
        {
            var courseModel = new CourseModel() { Name = "test" };

            ViewResult result = this.controller.CreateCourse(courseModel) as ViewResult;

            Assert.AreEqual("AddLessonsToCourse", result.ViewName);
        }

        [Test]
        public void AddLessonsToCourse_ReturnCorrectViewIfModelIsNotValid()
        {
            var mockModel = new AddLessonsViewModel();

            this.controller.ModelState.AddModelError(string.Empty, string.Empty);

            ViewResult result = this.controller.AddLessonsToCourse(mockModel) as ViewResult;

            Assert.AreEqual("AddLessonsToCourse", result.ViewName);
        }

        [Test]
        public void AddLessonsToCourse_ReturnCorrectViewIfDatesCoincides()
        {
            var courseMock = new CourseModel();
            var lessonMock = new LessonModel() { DateTime = new DateTime(2001, 01, 01) };
            var mockModel = new AddLessonsViewModel() { LessonModel = lessonMock, CourseModel = courseMock };

            ViewResult result = this.controller.AddLessonsToCourse(mockModel) as ViewResult;

            Assert.AreEqual("AddLessonsToCourse", result.ViewName);
        }

        [Test]
        public void AddLessonsToCourse_ReturnCorrectValueIfCourseIsDone()
        {
            var courseMock = new CourseModel() { IsDone = true };
            var lessonMock = new LessonModel() { DateTime = new DateTime() };
            var mockModel = new AddLessonsViewModel() { LessonModel = lessonMock, CourseModel = courseMock };

            RedirectToRouteResult result = this.controller.AddLessonsToCourse(mockModel) as RedirectToRouteResult;

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void AddLessonsToCourse_ReturnCorrectViewIfCourseIsNotDone()
        {
            var courseMock = new CourseModel();
            var lessonMock = new LessonModel() { DateTime = new DateTime() };
            var mockModel = new AddLessonsViewModel() { LessonModel = lessonMock, CourseModel = courseMock };

            ViewResult result = this.controller.AddLessonsToCourse(mockModel) as ViewResult;

            Assert.AreEqual("AddLessonsToCourse", result.ViewName);
        }

        [Test]
        public void GetCourseFromXmlFile_ReturnCorrectViewIfFileIsWrong()
        {
            var mock = new Mock<HttpPostedFileBase>();
            mock.Setup(_ => _.FileName).Returns("test");

            ViewResult result = this.controller.GetCourseFromXmlFile(mock.Object) as ViewResult;

            Assert.AreEqual("Error", result.ViewName);
        }

        [Test]
        public void GetCourseFromXmlFile_ReturnCorrectViewIfCourseModelIsNotDone()
        {
            var mock = new Mock<HttpPostedFileBase>();
            mock.Setup(_ => _.FileName).Returns("test.xml");

            ViewResult result = this.controller.GetCourseFromXmlFile(mock.Object) as ViewResult;

            Assert.AreEqual("AddLessonsToCourse", result.ViewName);
        }

        [Test]
        public void GetCourseFromXmlFile_ReturnCorrectValueIfCourseModelIsDone()
        {
            var mock = new Mock<HttpPostedFileBase>();
            mock.Setup(_ => _.FileName).Returns("true.xml");

            RedirectToRouteResult result = this.controller.GetCourseFromXmlFile(mock.Object) as RedirectToRouteResult;

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void SaveCourseToXml_ReturnCorrectValue()
        {
            RedirectToRouteResult result = this.controller.SaveCourseToXml("test") as RedirectToRouteResult;

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
