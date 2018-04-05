using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SecondAttempt.Controllers
{
    public class MembersController : Controller
    {
        [Authorize(Policy = "ValidCodeRequirement")]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }

       

    }
}