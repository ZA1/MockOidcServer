using Microsoft.AspNetCore.Mvc;

namespace MockOidcServer.Controllers;

public class Home : Controller
{
    [HttpGet("~/health")]
    public async Task<IActionResult> Health()
    {
        return Ok();
    }

    [HttpGet("~/")]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}