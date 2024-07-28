﻿using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Numerics;

namespace PrestamosCreciendo.Controllers
{
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            return View();
        }

        [HttpPost]
        public IActionResult Index(DateTime date_start)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;
            List<TransactionSummaryDTO> data_summary = (from summary in _context.Summary
                                                 where summary.Created_at.Date == date_start.ToUniversalTime().Date
                                                 join credit in _context.Credit on summary.Id_credit equals credit.Id
                                                 where credit.Id_agent == CurrentUser.Id
                                                 join users in _context.Users on credit.Id_user equals users.Id
                                                 select new TransactionSummaryDTO()
                                                 {
                                                     name = users.Name,
                                                     last_name = users.LastName,
                                                     payment_number = credit.Payment_number,
                                                     utility = credit.Utility,
                                                     amount_neto = credit.Amount_neto,
                                                     id_credit = credit.Id,
                                                     number_index = summary.Number_index,
                                                     amount = summary.Amount,
                                                     created_at = summary.Created_at,
                                                 }).ToList();
            List<TransactionCreditDTO> data_credit = (from credit in _context.Credit
                                                      where credit.Created_at == date_start.ToUniversalTime().Date
                                                      && credit.Id_agent == CurrentUser.Id
                                                      join users in _context.Users on credit.Id_user equals users.Id
                                                      select new TransactionCreditDTO()
                                                      {
                                                          credit_id = credit.Id,
                                                          id = users.Id,
                                                          name = users.Name,
                                                          last_name = users.LastName,
                                                          province = users.Province,
                                                          created_at = credit.Created_at,
                                                          utility = credit.Utility,
                                                          payment_number = credit.Payment_number,
                                                          amount_neto = credit.Amount_neto,
                                                      }).ToList();

            List<Bills> data_bill = (from bill in _context.Bills
                                    where bill.Created_at.Date == date_start.ToUniversalTime().Date
                                    && bill.Id_agent == CurrentUser.Id
                                    select bill).ToList();
            float total_credit;
            float total;
            foreach (TransactionSummaryDTO data in data_summary)
            {
                total = _context.Summary.Where(x => x.Id_credit == data.id_credit).Sum(x => x.Amount);
                total_credit = _context.Credit.Where(x => x.Id == data.id_credit).Sum(x => x.Amount_neto);
                total_credit = total_credit+(total_credit*data.utility);
                total = total_credit - total;

                data.total_payment = total;
            }

            float total_summary = data_summary.Sum(x => x.amount);
            total_credit = data_credit.Sum(x => x.amount_neto);
            float total_bills = data_bill.Sum(x => x.Amount);

            TransactionDTO dataTransaction = new TransactionDTO()
            {
                summary = data_summary,
                credit = data_credit,
                bills = data_bill,
                total_summary = total_summary,
                total_bills = total_bills,
                total_credit = total_credit,
            };

            return View(dataTransaction);
        }
    }
}