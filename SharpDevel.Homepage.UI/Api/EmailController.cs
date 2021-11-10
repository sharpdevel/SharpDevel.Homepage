using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace SharpDevel.Homepage.UI.Api
{
	//Classes
	#region MessageViewModel
	public class MessageViewModel
	{
		public String SenderName { get; set; } = String.Empty;
		public String SenderEmail { get; set; } = String.Empty;
		public String SenderMessage { get; set; } = String.Empty;
	}
	#endregion

	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		//Fields
		#region smtpSettings
		private SmtpSettings smtpSettings;
		#endregion

		//Constructors
		#region EmailController
		public EmailController(IConfiguration configuration)
		{
			this.smtpSettings = configuration
				.GetSection("Smtp")
				.Get<SmtpSettings>();
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
			var message = new MailMessage();
			message.From = new MailAddress("ich@tobiasmundt.de");
			message.To.Add(new MailAddress("ich@tobiasmundt.de"));
			message.Subject = "Message from tobiasmundt.de";
			message.Body = $"<p>Company: {viewModel.SenderName}</p><p>Email: {viewModel.SenderEmail}</p><p>Message: {viewModel.SenderMessage}</p>";
			message.IsBodyHtml = true;

			var client = new SmtpClient(this.smtpSettings.Url, this.smtpSettings.Port);
			client.Credentials = new NetworkCredential(this.smtpSettings.User, this.smtpSettings.Pass);
			client.EnableSsl = true;
			client.Send(message);
		}
		#endregion
	}
}
