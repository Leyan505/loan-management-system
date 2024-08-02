using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "AgentOnly")]
    public class RouteController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public RouteController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? request)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<Credit> data = (from credit in _context.Credit
                                      where credit.Id_agent == CurrentUser.Id
                                      && credit.Status == "inprogress"
                                      orderby credit.Order_list
                                      select credit).ToList();

            List<DataRouteDTO> clients = new List<DataRouteDTO>();
            DateTime dt = DateTime.UtcNow;

            foreach (Credit d in data)
            {   
                if(!_context.Summary
                    .Where(x => x.Id_credit == d.Id && x.Created_at.DayOfYear == DateTime.UtcNow.DayOfYear && x.Created_at.Year == DateTime.UtcNow.Year)
                    .Any())
                    {
                        clients.Add(new DataRouteDTO()
                        {
                            user = _context.Users.Find(d.Id_user),
                            amount_total = d.Amount_neto+(d.Amount_neto*d.Utility),
                            days_rest = (dt - d.Created_at).Days,
                            saldo = (d.Amount_neto + (d.Amount_neto * d.Utility))-
                            _context.Summary.Where(x => x.Id_credit == d.Id).Sum(x => x.Amount),
                            quote = ((float)(d.Amount_neto*d.Utility)+(float)d.Amount_neto)/(float)d.Payment_number,
                            last_pay = _context.Summary.Where(x => x.Id_credit == d.Id)
                            .OrderByDescending(x => x.Id).FirstOrDefault(),
                            request = request,
                            Id = d.Id,
                            order_list = d.Order_list,


                        });
                    }
                
            }


            return View(clients);
        }
    }
}
