using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

[ApiController]
[Route("[controller]")]
public class ProtectEmailController : ControllerBase
{
    private static readonly string[] ProtectedDomains = { "example.com", "sample.net", "gmail.com" };

    [HttpPost]
    public IActionResult Protect([FromBody] EmailRequest request)
    {
        if (request?.Email == null)
        {
            return BadRequest(new { error = "メールアドレスが送信されていません" });
        }

        string email = request.Email.ToLower();

        if (!Regex.IsMatch(email, @"[^@]+@[^@]+\.[^@]+"))
        {
            return BadRequest(new { error = "無効なメールアドレス形式です" });
        }

        string domain = email.Split('@')[1];

        if (ProtectedDomains.Contains(domain))
        {
            string username = email.Split('@')[0];
            string maskedUsername = username.Substring(0, 2) + "***";
            string maskedEmail = $"{maskedUsername}@{domain}";
            return Ok(new { protected_email = maskedEmail });
        }
        else
        {
            return Ok(new { message = "このドメインのメールアドレスは保護対象外です" });
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}
