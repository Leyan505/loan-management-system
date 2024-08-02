using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "AgentOnly")]
    public class paymentController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public paymentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? format)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<PaymentDTO> data_user = (from data in _context.Credit
                              where data.Id_agent == CurrentUser.Id
                              join users in _context.Users on data.Id_user equals users.Id
                              select new PaymentDTO
                              {
                                  credit_id = data.Id,
                                  id_user = data.Id_user,
                                  name = users.Name,
                                  last_name = users.LastName,
                                  payment_number = data.Payment_number,
                                  amount_neto = data.Amount_neto,
                                  utility = data.Utility
                              }).ToList();

            foreach(PaymentDTO data in data_user)
            {
                if((from credit in _context.Credit
                    where credit.Id_user == data.id_user && credit.Id_agent == CurrentUser.Id
                    select credit).Any())
                {
                    data.amount_neto = data.amount_neto + (data.amount_neto*data.utility);
                    data.positive = data.amount_neto - (from summary in _context.Summary where summary.Id_credit == data.credit_id 
                                                        && summary.Id_agent == CurrentUser.Id
                                                        select summary.Amount).Sum();
                    data.payment_current = (from summary in _context.Summary where summary.Id_credit == data.credit_id 
                                            select summary).Count();
                }
            }    
            
            return View(data_user);
        }

        public IActionResult payment(int id, bool rev, string format)
        {
            CurrentUser = new LoggedUser(HttpContext);

            PaymentDTO? data = (from credit in _context.Credit
                       where credit.Id == id
                       select new PaymentDTO()
                       {
                           credit_id = credit.Id,
                           id_user = credit.Id_user,
                           amount_neto = credit.Amount_neto,
                           payment_number = credit.Payment_number,
                           utility = credit.Utility,
                           
                       }).FirstOrDefault();
            if(data == null) { return Json("No existe credito"); }

            data.name = (from user in _context.Users
                        where user.Id == data.id_user
                        select user.Name).First();
             
            data.last_name = (from user in _context.Users
                           where user.Id == data.id_user
                           select user.LastName).First();

            float tmp_amount = (from summary in _context.Summary
                                where summary.Id_credit == id
                                && summary.Id_agent == CurrentUser.Id
                                select summary.Amount).Sum();
            float amount_neto = data.amount_neto;
            amount_neto += amount_neto * data.utility;
            data.amount_neto = amount_neto;

            float tmp_quote = amount_neto / data.payment_number;           
            float tmp_rest = amount_neto-tmp_amount;

            data.positive = tmp_amount;
            data.rest = amount_neto - tmp_amount;
            data.payment_done = (from summary in _context.Summary
                                 where summary.Id_credit == id
                                 select summary).Count();
            data.payment_quote = (tmp_rest < tmp_quote) ? tmp_rest : tmp_quote;

            if (format != null)
            {
                if(format.Equals("json"))
                {
                    return Json(data);
                }
            }
            data.rev = rev;
            return CreatePayment(data);
        }

        public IActionResult CreatePayment(PaymentDTO data_payment)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;
            return View("CreatePayment", data_payment);
        }

        [HttpPost]
        public IActionResult NewPayment(NewPaymentRequest request, string? format)
        {            
            CurrentUser = new LoggedUser(HttpContext);

            if (!request.rev)
            {
                if((from summary in  _context.Summary
                   where summary.Created_at.DayOfYear == DateTime.UtcNow.DayOfYear 
                   && summary.Created_at.Year == DateTime.UtcNow.Year && summary.Id_credit == request.credit_id
                   select summary).Any())
                {
                    var response = new Response()
                    {
                        status = "fail",
                        msj = "Ya existe un pago hoy",
                        code = 0
                    };
                    return Json(response);                    
                }
            }

            int index = (from summary in _context.Summary
                         where summary.Id_credit == request.credit_id
                         && summary.Id_agent == CurrentUser.Id
                         select summary).Count();
            Summary values = new Summary()
            {
                Amount = request.amount,
                Id_credit = request.credit_id,
                Id_agent = CurrentUser.Id,
                Created_at = DateTime.UtcNow,
                Number_index = index + 1
            };

            _context.Summary.Add(values);
            _context.SaveChanges();

            var sum = (from summary in _context.Summary
                      where summary.Id_credit == request.credit_id
                      select summary.Amount).Sum();

            float sum_neto = _context.Credit.Find(request.credit_id).Amount_neto + _context.Credit.Find(request.credit_id).Amount_neto * _context.Credit.Find(request.credit_id).Utility;

            if (sum >= sum_neto-0.99)
            {
                _context.Credit.Where(x => x.Id == request.credit_id)
                    .ExecuteUpdate(setters => setters
                    .SetProperty(x => x.Status, "close"));
            }
            float amount_last = 0;
            Expression<Func<PrestamosCreciendo.Models.Summary, bool>> sql = x => x.Id_agent == CurrentUser.Id;
            if (request.credit_id != 0)
            {
                sql = x => x.Id_credit == request.credit_id;

            }

            if (_context.Summary.Where(sql).Any())
            {
                amount_last = _context.Summary.Where(sql)
                    .OrderByDescending(x => x.Id).FirstOrDefault().Amount;
            }

            CreditDTO? data_credit = (from credit in _context.Credit
                                 where credit.Id == request.credit_id
                                 select new CreditDTO
                                 {
                                     Id = credit.Id,
                                     Id_agent = credit.Id_agent,
                                     Id_user = credit.Id_user,
                                     Amount_neto = credit.Amount_neto,
                                     Created_at = credit.Created_at,
                                     Order_list = credit.Order_list,
                                     Payment_number = credit.Payment_number,
                                     Status = credit.Status,
                                     Utility = credit.Utility,
                                 }).FirstOrDefault();
            
            if ((data_credit != null))
            {
                data_credit.total = data_credit.Utility * data_credit.Amount_neto + (data_credit.Amount_neto);

            }
            Response data = new Response()
            {
                status = "success",
                code = 0,
                recent = amount_last,
                rest = (data_credit.total) - (_context.Summary.Where(sql).Select(x => x.Amount).FirstOrDefault())
            };

            if(format != null)
            {
                return Json(data);
            }
            return RedirectToAction("Summary", "payment", new { id_credit = request.credit_id, show = "last"});
        }

        public IActionResult Summary(int id_credit, string? show)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            if (!_context.Credit.Where(x => x.Id == id_credit).Any())
            {
                return View(new SummaryDTO() { error = new ErrorViewModel { description = "ID no existe"} });
            }

            Expression<Func<PrestamosCreciendo.Models.Summary, bool>> sql = x => x.Id_agent == CurrentUser.Id;
            if (id_credit != 0)
            {
                sql = x => x.Id_credit == id_credit;

            }

            SummaryCredit? data_credit = _context.Credit.Where(x=> x.Id == id_credit)
                .Select(x=> new SummaryCredit () { 
                    Id = x.Id,
                    Id_agent = x.Id_agent,
                    Amount_neto = x.Amount_neto,
                    Created_at = x.Created_at,
                    Id_user = x.Id_user,
                    Order_list = x.Order_list,
                    Status = x.Status,
                    Payment_number = x.Payment_number,
                    Utility = x.Utility,
                }).FirstOrDefault();
            List<SummaryClients>? tmp = _context.Summary.Where(sql).Select(x => new SummaryClients()
            {
                Amount = x.Amount,
                Created_at = x.Created_at,
                Id_credit = x.Id_credit,
                Id = x.Id,
                Id_agent = x.Id_agent,
                Number_index = x.Number_index,
            }).ToList();

            float amount = _context.Credit.Find(id_credit).Amount_neto + _context.Credit.Find(id_credit).Amount_neto * _context.Credit.Find(id_credit).Utility;

            foreach(SummaryClients t in tmp) 
            {
                amount -= t.Amount;
                /*if(amount < 1) { amount = 0; }*/
                t.rest = amount;
            }

            data_credit.utility_amount = (float)(data_credit.Utility * data_credit.Amount_neto);
            data_credit.Utility = (float)(data_credit.Utility * 100);
            data_credit.payment_amount = (float)(data_credit.Amount_neto + data_credit.utility_amount) / (float)(data_credit.Payment_number);

            data_credit.total = (float)(data_credit.utility_amount+data_credit.Amount_neto);
            float amount_last = 0;

            if(_context.Summary.Where(sql).Any())
            {
                amount_last = _context.Summary.Where(sql).OrderByDescending(x => x.Id).FirstOrDefault().Amount;
            }

            SummaryDTO data = new SummaryDTO()
            {
                clients = tmp,
                user = _context.Users.Find(_context.Credit.Find(id_credit).Id_user),
                credit_data = data_credit,
                recent = amount_last,
                rest = data_credit.total - (_context.Summary.Where(sql).Sum(x => x.Amount)),
                show = show,
                other_credit = _context.Credit.Where(x => x.Id_user == data_credit.Id_user).ToList()
            };

            return View(data); 
        }
    }
}
