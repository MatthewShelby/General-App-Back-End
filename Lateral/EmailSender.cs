using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Doctors
{
    public class EmailSender
    {
        private IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string link)
        {
            var a = await SendActivationLinkAsync(email, link);
        }





        private async Task<IActionResult> SendActivationLinkAsync(string email, string link)
        {
            try
            {
                string smtpServerAddress = _configuration.GetSection("SMTP:SMTPServer").Value;
                string smtpServerPort = _configuration.GetSection("SMTP:SMTPPort").Value;
                string senderEmail = _configuration.GetSection("SMTP:Username").Value;
                string senderPassword = _configuration.GetSection("SMTP:Password").Value;
                string senderDisplayName = _configuration.GetSection("SMTP:senderDisplayName").Value;
                bool useSSL = Convert.ToBoolean(_configuration.GetSection("SMTP:SMTPUseSSL").Value);
                bool useDC = Convert.ToBoolean(_configuration.GetSection("SMTP:useDefaultCredentials").Value);


                //User user = await _user.GetUserByEmail(email);

                var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, senderDisplayName + " --  registration  complete!");
                mail.To.Add(email);
                mail.Subject = "Registration Status";

                string activateAddress = _configuration.GetSection("AppSettings:ActivateAddress").Value;
                string helpAddress = _configuration.GetSection("AppSettings:HelpAddress").Value;
                //string[] model = new string[] { activateAddress+"/"+user.ActivationCode ,
                string[] model = new string[] { activateAddress+"/"+"user.ActivationCode" ,
                    _configuration.GetSection("Names:FaName").Value,
                    _configuration.GetSection("Names:EnName").Value,
                    helpAddress
                };
                mail.IsBodyHtml = true;
                //mail.Body = await _viewRenderService.RenderToStringAsync("Emails/ActivateAccount", model);
                mail.Body =
                    "<div style = \"margin: 30px; padding: 30px; border: 1px lightsteelblue solid;\" >" +
                     "<h3> Thank you for register to our app.</h3>" +
                        $"<p>in order to activate your account, please <a href=\" {link} \" > click here</a>. </p>" +
                        "<p> or copy this link into your browser address bar and hit enter.</p>" +
                        $"<p style = \"color: limegreen;\" > {link} </p> " +
                    "</div>";

                var SmtpServer = new SmtpClient(smtpServerAddress);
                SmtpServer.Port = Convert.ToInt32(smtpServerPort);//465;// 25;// 587;
                SmtpServer.EnableSsl = useSSL;
                SmtpServer.UseDefaultCredentials = useDC;
                SmtpServer.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);

                SmtpServer.Send(mail);


                return new ObjectResult("Done");

            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message);

            }

        }
        public string Send(string smtpServerAddress, string smtpServerPort, string senderEmail, string senderPassword, string senderDisplayName, string recipientEmail,
            bool isHTML, bool useSSL,
            string subject, string body)
        {
            int step = 0;
            try
            {
                var mail = new MailMessage();
                step++;

                //var SmtpServer = new SmtpClient("barnamenevis.ir");
                var SmtpServer = new SmtpClient(smtpServerAddress);
                step++;

                mail.From = new MailAddress(senderEmail, senderDisplayName);
                step++;

                mail.To.Add(recipientEmail);
                step++;

                mail.Subject = subject;
                step++;

                mail.Body = body;
                step++;

                mail.IsBodyHtml = isHTML; //true
                step++;

                // System.Net.Mail.Attachment attachment;
                // attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
                // mail.Attachments.Add(attachment);

                SmtpServer.Port = Convert.ToInt32(smtpServerPort);
                step++;

                SmtpServer.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                step++;

                SmtpServer.EnableSsl = useSSL; // false
                step++;

                SmtpServer.UseDefaultCredentials = true;
                step++;

                SmtpServer.Send(mail);
                step++;

                return "Done.";

            }
            catch (Exception ex)
            {
                return "step: " + step + " \n" + ex.Message;
            }
        }
    }
}
