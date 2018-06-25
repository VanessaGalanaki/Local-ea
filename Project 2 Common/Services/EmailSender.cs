using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Project_2_Common.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
			var apiKey = "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";
            var client = new SendGridClient(apiKey);
            
			var mssg = new SendGridMessage()
			{
				From = new EmailAddress("locale@support.gr", "Support"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message               
			};

			mssg.AddTo(new EmailAddress(email));
			return  client.SendEmailAsync(mssg);


   //         var from = new EmailAddress("locale@support.net", "Support");
   //         var to = new EmailAddress(email);
            
			//var msg = MailHelper.CreateSingleEmail(from, to, subject, message);
   //         var response = await client.SendEmailAsync(msg);


			//return Task.CompletedTask;

        }
    }
}
