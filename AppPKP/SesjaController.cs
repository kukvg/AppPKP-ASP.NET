using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppPKP
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesjaController : ControllerBase
    {
        [HttpPost]
        public IEnumerable<string> SesjaStatus()
        {
            List<string> statusSesja = new List<string>();

            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(ZmienneSesja.kluczSesji)))
            {
                HttpContext.Session.SetString(ZmienneSesja.kluczSesji, "User");
                HttpContext.Session.SetString(ZmienneSesja.idSesji, Guid.NewGuid().ToString());
            }


            var nazwaUser = HttpContext.Session.GetString(ZmienneSesja.kluczSesji);
            var sesjaId = HttpContext.Session.GetString(ZmienneSesja.idSesji);

            statusSesja.Add(nazwaUser);
            statusSesja.Add(sesjaId);

            return statusSesja;
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }

    }
}
