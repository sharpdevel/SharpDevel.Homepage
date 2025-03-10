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
			var message = new MailMessage();
			message.From = new MailAddress("ich@tobiasmundt.de");
			message.To.Add(new MailAddress("ich@tobiasmundt.de"));
			message.Subject = "Message from tobiasmundt.de";
			message.Body = $"<p>Company: {viewModel.SenderName}</p><p>Email: {viewModel.SenderEmail}</p><p>Message: {viewModel.SenderMessage}</p>";
			message.IsBodyHtml = true;

			var smtpSettings = this.configuration
				.GetSection("Smtp")
				.Get<SmtpSettings>();
			var client = new SmtpClient(smtpSettings.Url, smtpSettings.Port);
			client.Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Pass);
			client.EnableSsl = true;
			client.Send(message);
		}
		#endregion
	}
}
