﻿using MyMoney.Models;
using MyMoney.Models.Enums;
using MyMoney.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyMoney.Controllers
{
    public class AccountingController : Controller
    {
        private AccountingModels accountDbContext = new AccountingModels();

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddForAjax(AccountingViewModel pageData)
        {
            if (!ModelState.IsValid)
            {
                var errorFields = ModelState.Where(d => d.Value.Errors.Any())
                     .Select(x => new { x.Key, x.Value.Errors });

                var errors = new List<AjaxErrorResultViewModel>();
                foreach (var item in errorFields)
                {
                    errors.Add(item.Errors.Select(
                        d => new AjaxErrorResultViewModel()
                        {
                            ClientId = item.Key,
                            ErrorMessage = d.ErrorMessage
                        }).FirstOrDefault());
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errors);
            }

            var accountBook = new AccountBook
            {
                Amounttt = pageData.Amount,
                Categoryyy = (int)pageData.Type,
                Dateee = pageData.Date,
                Remarkkk = pageData.Remark,
                Id = Guid.NewGuid()
            };

            this.accountDbContext.AccountBook.Add(accountBook);
            this.accountDbContext.SaveChanges();

            return PartialView("ShowHistory", this.GetLastAccountings(10));
        }

        [HttpPost]
        public ActionResult Add(AccountingViewModel pageData)
        {
            if (!ModelState.IsValid)
            {
                return View(pageData);
            }

            var accountBook = new AccountBook
            {
                Amounttt = pageData.Amount,
                Categoryyy = (int)pageData.Type,
                Dateee = pageData.Date,
                Remarkkk = pageData.Remark,
                Id = Guid.NewGuid()
            };

            this.accountDbContext.AccountBook.Add(accountBook);
            this.accountDbContext.SaveChanges();

            return RedirectToAction("Add");
        }

        public ActionResult ShowHistory()
        {
            IQueryable<AccountingViewModel> accountingBooks = GetLastAccountings(10);

            return View(accountingBooks);
        }

        private IQueryable<AccountingViewModel> GetLastAccountings(int recordCount)
        {
            return this.accountDbContext.AccountBook
                            .OrderByDescending(x => x.Dateee)
                            .Take(recordCount)
                            .Select(x =>
                            new AccountingViewModel
                            {
                                Amount = x.Amounttt,
                                Date = x.Dateee,
                                Remark = x.Remarkkk,
                                Type = (AccountingType)x.Categoryyy
                            });
        }

        protected override void Dispose(bool disposing)
        {
            accountDbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}