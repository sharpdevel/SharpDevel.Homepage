using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace SharpDevel.Homepage.UI.Api
{
	//Classes
	#region MessageViewModel
	public class MessageViewModel
	{
		[Required]
		[MaxLength(200)]
		public String SenderName { get; set; } = String.Empty;

		[Required]
		[MaxLength(320)]
		[EmailAddress]
		public String SenderEmail { get; set; } = String.Empty;

		[Required]
		[MaxLength(5000)]
		public String SenderMessage { get; set; } = String.Empty;
	}
	#endregion

	[Route("api/[controller]")]
	[ApiController]
	[EnableRateLimiting("email")]
	public class EmailController : ControllerBase
	{
		//Fields
		#region smtpSettings
		private IConfiguration configuration;
		#endregion

		//Constructors
		#region EmailController
		public EmailController(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		#endregion

		//Methods
		#region Post
		/// <summary>
		/// Send an email.
		/// URL = POST api/<EmailController>
		/// </summary>
		/// <param name="value"></param>
		[HttpPost]
		public void Post([FromBody] MessageViewModel viewModel)
		{
			// User input is HTML-encoded — it ends up in an HTML mail body.
			var message = new MailMessage();
			message.From = new MailAddress("ich@tobiasmundt.de");
			message.To.Add(new MailAddress("ich@tobiasmundt.de"));
			message.Subject = "Message from tobiasmundt.de";
			message.Body =
				$"<p>Company: {WebUtility.HtmlEncode(viewModel.SenderName)}</p>" +
				$"<p>Email: {WebUtility.HtmlEncode(viewModel.SenderEmail)}</p>" +
				$"<p>Message: {WebUtility.HtmlEncode(viewModel.SenderMessage).ReplaceLineEndings("<br />")}</p>";
			message.IsBodyHtml = true;

			var smtpSettings = this.configuration
				.GetSection("Smtp")
				.Get<SmtpSettings>();
			using var client = new SmtpClient(smtpSettings.Url, smtpSettings.Port);
			client.Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Pass);
			client.EnableSsl = true;
			client.Send(message);
		}
		#endregion
	}
}
