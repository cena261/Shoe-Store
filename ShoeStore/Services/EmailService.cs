using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace ShoeStore.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            _fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
            _fromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "ShoeStore";
        }

        public bool SendPasswordResetCode(string toEmail, string code, string userName)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = "Password Reset Verification Code - ShoeStore";
                    message.IsBodyHtml = true;

                    message.Body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .header {{ background-color: #000; color: white; padding: 20px; text-align: center; }}
                                .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                                .code {{ font-size: 32px; font-weight: bold; color: #000; text-align: center;
                                         letter-spacing: 5px; padding: 20px; background-color: #fff;
                                         border: 2px dashed #000; margin: 20px 0; }}
                                .footer {{ margin-top: 20px; text-align: center; color: #666; font-size: 12px; }}
                                .warning {{ color: #d9534f; margin-top: 15px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>ShoeStore</h1>
                                </div>
                                <div class='content'>
                                    <h2>Password Reset Request</h2>
                                    <p>Hi {userName},</p>
                                    <p>We received a request to reset your password. Use the verification code below to complete the process:</p>

                                    <div class='code'>{code}</div>

                                    <p>This code will expire in <strong>15 minutes</strong>.</p>

                                    <div class='warning'>
                                        <strong>Did not request this?</strong><br>
                                        If you did not request a password reset, please ignore this email. Your password will remain unchanged.
                                    </div>
                                </div>
                                <div class='footer'>
                                    <p>&copy; {DateTime.Now.Year} ShoeStore. All rights reserved.</p>
                                    <p>This is an automated email. Please do not reply.</p>
                                </div>
                            </div>
                        </body>
                        </html>";

                    using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        smtpClient.Timeout = 10000; 

                        smtpClient.Send(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email send error: {ex.Message}");
                return false;
            }
        }

        public static string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); 
        }
    }
}
