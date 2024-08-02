using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "AgentOnly")]
    public class AgenteController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public AgenteController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            DateTime date_endLater = DateTime.UtcNow.AddDays(1).Date;
            DateTime date_startSooner = DateTime.UtcNow.AddDays(-1).Date;
            DateTime DtNow = DateOffset.DateNow(DateTime.UtcNow, CurrentUser.TimeOffset);

            var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach(var Item in materializedSummary)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float total_summary = (from summary in materializedSummary
                                   where summary.Created_at.Date == DtNow
                                   && summary.Id_agent == CurrentUser.Id
                                   join credit in _context.Credit on summary.Id_credit equals credit.Id
                                   join users in _context.Users on credit.Id_user equals users.Id
                                   select summary.Amount).Sum();


            var materializedClose = _context.CloseDay.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach (var Item in materializedClose)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            CloseDay? closeDay = (from close_day in materializedClose
                                  where close_day.Created_at.Date == DtNow
                                  && close_day.Id_agent == CurrentUser.Id
                                  select close_day).FirstOrDefault();

            float Base = (from supervisorAgent in _context.AgentSupervisor
                          where supervisorAgent.IdAgent == CurrentUser.Id
                          select supervisorAgent).FirstOrDefault().Base;


            var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach (var Item in materializedCredit)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float base_credit = (from credit in materializedCredit
                                 where credit.Created_at.Date == DtNow
                                 && credit.Id_agent == CurrentUser.Id
                                 select credit).Sum(x => x.Amount_neto);
            Base -= base_credit;

            var materializedBill = _context.Bills.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach (var Item in materializedBill)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }
            List<BillsResumeDTO> bill = materializedBill.Where(x => x.Id_agent == CurrentUser.Id && x.Created_at.Date == DtNow)
                         .Join(_context.Wallets, bills => bills.Id_wallet, wallets => wallets.Id,
                         (bills, wallets) => new BillsResumeDTO { bill = bills, wallet_name = wallets.Name })
                         .ToList();


            AgentResumeDTO data = new AgentResumeDTO()
            {
                base_agent = Base,
                total_bill = bill.Sum(x => x.bill.Amount),
                total_summary = total_summary,
                close_day = closeDay
            };

            return View(data);
        }

        public IActionResult NewClient(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            if (Id != 0)
            {
                UsersDTO? client = (from clients in _context.Users
                                    where clients.Id == Id
                                    select new UsersDTO()
                                    {
                                        Id = clients.Id,
                                        Nit = clients.Nit,
                                        Name = clients.Name,
                                        LastName = clients.LastName,
                                        Address = clients.Address,
                                        Province = clients.Province,
                                        Phone = clients.Phone,
                                        Status = clients.Status
                                    }).FirstOrDefault();
                if (client != null) { return View(client); }
            }

            return View((new UsersDTO()));
        }

        public IActionResult ClientData(int Id) {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            if (Id != 0)
            {
                UsersDTO? client = (from clients in _context.Users
                                    where clients.Id == Id
                                    select new UsersDTO()
                                    {
                                        Id = clients.Id,
                                        Nit = clients.Nit,
                                        Name = clients.Name,
                                        LastName = clients.LastName,
                                        Address = clients.Address,
                                        Province = clients.Province,
                                        Phone = clients.Phone,
                                        Status = clients.Status
                                    }).FirstOrDefault();
                if (client != null) { return View(client); }
            }
            return View(new UsersDTO());
        }

        [HttpPost]
        public IActionResult NewClient(UsersDTO user, float amount, float utility, int payment_number)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            float Base = (from agentSupervisor in _context.AgentSupervisor
                          where agentSupervisor.IdAgent == CurrentUser.Id
                          select agentSupervisor.Base).FirstOrDefault();

            float BaseCredit = (from credit in _context.Credit
                                where credit.Created_at.DayOfYear == DateTime.UtcNow.DayOfYear
                                && credit.Created_at.Year == DateTime.UtcNow.Year
                                select credit.Amount_neto).Sum();

            Base -= BaseCredit;

            if (amount > Base)
            {
                ErrorViewModel error = new ErrorViewModel() { description = "No tienes dinero suficiente" };
                return View(new UsersDTO() { Error = error });
            }

            if (!(from users in _context.Users
                  where users.Nit == user.Nit
                  select user).Any())
            {
                _context.Users.Add(new Users()
                {
                    Name = user.Name,
                    LastName = user.LastName,
                    Address = user.Address,
                    Nit = user.Nit,
                    Province = user.Province,
                    Phone = user.Phone,
                    Level = user.Level,
                    lat = user.lat,
                    lng = user.lng,

                });
                _context.SaveChanges();
            }
            else
            {
                int ClientId = (from users in _context.Users
                                where users.Nit == user.Nit
                                select users.Id).First();

                if ((from agentClient in _context.AgentHasClient
                     where agentClient.Id_client == ClientId
                     select agentClient).Any())
                {
                    AgentHasClient AgentData = (from users in _context.AgentHasClient
                                                where users.Id_client == ClientId
                                                select users).First();
                    if (AgentData.Id_agent != CurrentUser.Id)
                    {
                        ErrorViewModel error = new ErrorViewModel() { description = "Este usuario ya esta asignado a otro agente" };
                        return View(new UsersDTO() { Error = error });
                    }
                }
            }
            int IdClient = (from users in _context.Users
                            where users.Nit == user.Nit
                            select users.Id).First();
            if (!(from agentClient in _context.AgentHasClient
                  where agentClient.Id_agent == CurrentUser.Id && agentClient.Id_client == IdClient
                  select agentClient).Any())
            {
                _context.AgentHasClient.Add(new AgentHasClient {
                    Id_agent = CurrentUser.Id,
                    Id_client = IdClient,
                    Id_wallet = (from supervisorAgent in _context.AgentSupervisor
                                 where supervisorAgent.IdAgent == CurrentUser.Id
                                 select supervisorAgent.IdWallet).First()
                });

            }
            int LastOrder;
            if (!(from credit in _context.Credit
                  orderby credit.Order_list descending
                  select credit).Any())
            {
                LastOrder = 0;
            }
            else
            {
                LastOrder = (from credit in _context.Credit
                             orderby credit.Order_list descending
                             select credit.Order_list).First();
            }

            _context.Credit.Add(new Credit()
            {
                Created_at = DateTime.UtcNow,
                Payment_number = payment_number,
                Utility = utility,
                Amount_neto = amount,
                Id_user = IdClient,
                Id_agent = CurrentUser.Id,
                Order_list = LastOrder + 1
            });
            _context.SaveChanges();

            return RedirectToAction("ShowClients");
        }

        public IActionResult ShowClients()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            var agent_has_client = (from agentClient in _context.AgentHasClient
                                    where agentClient.Id_agent == CurrentUser.Id
                                    join user in _context.Users on agentClient.Id_client equals user.Id
                                    select user).ToList();

            List<ClientDTO> DataClients = new List<ClientDTO>();

            foreach (Users user in agent_has_client)
            {
                if ((from credit in _context.Credit
                     where credit.Id_user == user.Id
                     select credit).Any())
                {
                    ClientDTO? client = new ClientDTO()
                    {
                        name = user.Name,
                        last_name = user.LastName,
                        province = user.Province,
                        status = user.Status,
                        closed = (from credit in _context.Credit
                                  where credit.Status == "close" && credit.Id_user == user.Id
                                  select credit).Count(),
                        inprogress = (from credit in _context.Credit
                                      where credit.Status == "inprogress" && credit.Id_user == user.Id
                                      select credit).Count(),
                        credit_count = (from credit in _context.Credit
                                        where credit.Id_user == user.Id
                                        select credit).Count(),
                        amount_net = (from credit in _context.Credit
                                      where credit.Id_user == user.Id && credit.Status == "inprogress"
                                      select credit).ToList(),
                        Id = user.Id,
                        lat = user.lat,
                        lng = user.lng,

                    };

                    if (client.amount_net != null)
                    {
                        client.summary_net = (from summary in _context.Summary
                                              where summary.Id_agent == CurrentUser.Id
                                              join credit in _context.Credit on summary.Id_credit equals credit.Id
                                              where credit.Id == user.Id
                                              select summary.Amount).Sum();
                        float tmp_credit = client.amount_net.Sum(x => x.Amount_neto);
                        float tmp_rest = tmp_credit - client.summary_net;
                        client.summary_net = tmp_rest;

                        foreach (var utility in client.amount_net)
                        {
                            client.gap_credit += utility.Amount_neto * utility.Utility;
                        }
                    }
                    else { client.summary_net = 0; }


                    DataClients.Add(client);
                }
            }
            return View(DataClients);
        }

        public IActionResult Simulator()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;
            return View();
        }

        public IActionResult HistoryIndex()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            return View();
        }

        [HttpPost]
        public IActionResult HistoryCreate(DateTime date_start)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;


            float base_amount = _context.AgentSupervisor.Where(x => x.IdAgent == CurrentUser.Id).FirstOrDefault().Base;

            DateTime date_endLater = date_start.AddDays(1);
            DateTime date_startSooner = date_start.AddDays(-1);

            DateTime DtNow = DateOffset.DateNow(DateTime.UtcNow, CurrentUser.TimeOffset);


            var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();
            foreach (var Item in materializedSummary)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }
            
            float today_amount = materializedSummary.Where(x => x.Created_at.Date == date_start.Date && x.Id_agent == CurrentUser.Id).Sum(x => x.Amount);

            var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach (var Item in materializedCredit)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float today_sale = materializedCredit.Where(x => x.Created_at.Date == date_start.Date && x.Id_agent == CurrentUser.Id).Sum(x => x.Amount_neto);

            var materializedBill= _context.Bills.Where(x => x.Created_at.Date <= date_endLater && x.Created_at >= date_startSooner.Date).ToList();

            foreach (var Item in materializedBill)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float bills = materializedBill.Where(x => x.Created_at.Date == date_start).Sum(x => x.Amount);


            float total = (float)base_amount+today_amount - (float)today_sale+bills;
            float average = 1000;


            return View(new HistoryAgentDTO()
            {
                Base = base_amount,
                today_amount = today_amount,
                today_sell = today_sale,
                bills = bills,
                total = total,
                average = average
            });
        }
        
    }
}
