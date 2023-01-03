using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StatsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<SE> results = _context.SE.Include(s => s.Session).Where(s => s.Session.DateTimeStart >= DateTime.Now.AddDays(-28)).Where(s => s.UserId == _userManager.GetUserId(User)).ToList();
            List<Exercise> exercises = _context.Exercise.ToList();
            List<Session> sessions = _context.Session.Where(s => s.DateTimeStart >= DateTime.Now.AddDays(-28)).Where(s => s.UserId == _userManager.GetUserId(User)).ToList();
            List<Statistic> stats = new List<Statistic>();
            
            foreach(Exercise ec in exercises)
            {
                int numOfSessions = 0;
                int bestResult = 0;
                foreach(SE result in results)
                {
                    if(result.ExerciseId == ec.Id && result.SessionId != null)
                    {
                        numOfSessions++;
                    }
                    if(result.ExerciseId == ec.Id && result.Weight * result.NumOfSeries * result.NumOfReps >= bestResult)
                    {
                        bestResult = result.Weight * result.NumOfSeries * result.NumOfReps;
                    }
                }
                if (numOfSessions > 0)
                {
                    Statistic stat = new Statistic();
                    stat.ExerciseName = ec.Name;
                    stat.NumOfSessions = numOfSessions;
                    stat.BestResult = bestResult;
                    stats.Add(stat);
                }
            }
            ViewData["Stats"] = stats;
            return View();
        }
    }
}
