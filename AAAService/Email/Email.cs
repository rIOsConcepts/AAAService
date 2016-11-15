using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using AAAService.Models;

namespace AAAService.Correspondence
{
    public class Mail
    {
        private aaahelpEntities db = new aaahelpEntities();
        //public MailMessage mailMessage;
        public Mail()
        {
            //mailMessage = new MailMessage("rIOsConcepts@gmail.com", "rIOsConcepts@gmail.com");
        }

        //public async void Send(string subject)
        public void Send(string subject, string body, string emailAddress = "", bool isHTML = false, string email = "")
        {
            //var client = new SmtpClient();
            //client.Port = 25;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = "smtp.assetsaaa.com";
            //mailMessage.Subject = "This is a test email.";
            //mailMessage.Body = "This is my test email body";
            //client.Send(mailMessage);

            //var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            //var message = new MailMessage();
            //message.To.Add(new MailAddress("riosconcepts@gmail.com"));  // replace with valid value 
            //message.From = new MailAddress("administrator@aaahelp.aaapropertyservices.com");  // replace with valid value
            //message.Subject = "AAAWebPortal Location Add";
            //var fromName = "Manuel Rios";
            //var fromEmail = "administrator@aaahelp.aaapropertyservices.com";
            //var bodyMessage = "This is the body of the email";
            //message.Body = string.Format(body, fromName, fromEmail, bodyMessage);
            //message.IsBodyHtml = true;

            //using (var smtp = new SmtpClient())
            //{
            //var credential = new NetworkCredential
            //{
            //    UserName = "administrator@aaahelp.aaapropertyservices.com",  // replace with valid value
            //    Password = ""  // replace with valid value
            //};

            //smtp.Credentials = credential;
            //smtp.Host = "72.172.94.31";
            //smtp.Port = 2525;
            //smtp.EnableSsl = false;
            //smtp.Timeout = 5000;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp.UseDefaultCredentials = false;
            //await smtp.SendMailAsync(message);
            //return RedirectToAction("Sent");
            //}

            //try
            //{
            //    SmtpClient smtpClient = new SmtpClient();
            //    smtpClient.Host = "assetsaaa.com";
            //    smtpClient.Port = 25;
            //    smtpClient.Credentials = new System.Net.NetworkCredential("administrator@assetsaaa.com", "4vplV01Yz00");

            //    MailMessage mail = new MailMessage();
            //    mail.To.Add(new MailAddress("riosconcepts@gmail.com"));
            //    mail.Body = "This is a test message.";
            //    mail.Subject = "Test - " + DateTime.Now;
            //    mail.From = new MailAddress("administrator@assetsaaa.com");
            //    smtpClient.Send(mail);
            //}
            //catch (Exception ex)
            //{
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
            //    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", ex.Message);
            //}

            //MailMessage msg = new MailMessage();
            //msg.From = new MailAddress("assetsaaa.com@gmail.com");
            //var emailTo = new MailAddress("riosconcepts@gmail.com");
            //msg.To.Add(emailTo);
            //emailTo = new MailAddress("giulianomx@me.com");
            //msg.To.Add(emailTo);
            //msg.Subject = "This is your lucky day" + DateTime.Now.ToString();
            //msg.Body = "Hi you, this is me :)";

            //SmtpClient client = new SmtpClient();
            //client.Host = "smtp.gmail.com";
            //client.Port = 587;
            //client.EnableSsl = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("assetsaaa.com@gmail.com", "87A69D62-071D-46A2-8325-00412E2E5DD5");
            //client.Timeout = 20000;
            //try
            //{
            //    client.Send(msg);
            //    return "Mail has been successfully sent!";
            //}
            //catch (Exception ex)
            //{
            //    return "Fail Has error" + ex.Message;
            //    throw ex; Error 500 on server
            //    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", ex.ToString());
            //}
            //finally
            //{
            //    msg.Dispose();
            //}

            //MailMessage msg = new MailMessage();
            //msg.From = new MailAddress("​administrator@assetsaaa.com");
            //var emailTo = new MailAddress("riosconcepts@gmail.com");
            //msg.To.Add(emailTo);
            //msg.Subject = "This is your lucky day" + DateTime.Now.ToString();
            //msg.Body = "Hi you, this is me :)";

            //SmtpClient client = new SmtpClient("localhost");
            //client.Host = "smtp.​arvixe.com";
            //client.Port = 587;
            //client.EnableSsl = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("​administrator@assetsaaa.com", "LetMeIn00@");
            //client.Timeout = 20000;
            //try
            //{
            //    client.Send(msg);
            //    return "Mail has been successfully sent!";
            //}
            //catch (Exception ex)
            //{
            //    return "Fail Has error" + ex.Message;
            //    throw ex; Error 500 on server
            //    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", ex.ToString());
            //}
            //finally
            //{
            //    msg.Dispose();
            //}

            try
            {
                //MailMessage msg = new MailMessage("administrator@assetsaaa.com", "riosconcepts@gmail.com");
                //msg.Subject = "Subject here";
                //msg.Body = "Body here";

                //SmtpClient client = new SmtpClient("localhost");
                //client.UseDefaultCredentials = false;
                //client.Credentials = new System.Net.NetworkCredential("​administrator@assetsaaa.com", "​LetMeIn00@");
                //client.Send(msg);

                System.Net.Mail.MailMessage eMail = new System.Net.Mail.MailMessage();
                eMail.From = new System.Net.Mail.MailAddress("support@assetsaaa.com");

                if (email != "")
                {
                    eMail.To.Add(email);
                }

                eMail.Bcc.Add("riosconcepts@gmail.com");
                eMail.Bcc.Add("TZhang@aaacompanies.com");

                //When ticket is first entered – email goes to order@aaacompanies.com, erios@aaacompanies.com, sdollen@aaacompanies.com, bhiggins@aaacompanies.com, the user that entered the ticket and every user attached to that site / location.

                //Anytime a ticket is updated, changed, etc.an email needs to go out to the following people:  erios@aaacompanies.com, sdollen@aaacompanies.com, bhiggins@aaacompanies.com, the user that entered the ticket and every user attached to that site / location.

                if (subject == "Web Portal Service Ticket Entered")
                {
                    eMail.To.Add("order@aaacompanies.com");
                }

                if (subject != "Web Portal User Invitation")
                {
                    eMail.CC.Add("erios@aaacompanies.com");
                    eMail.CC.Add("sdollen@aaacompanies.com");
                    //eMail.CC.Add("schristopulos@aaacompanies.com");
                    eMail.CC.Add("bhiggins@aaacompanies.com");

                    if (email != "")
                    {
                        var query = db.AspNetUsers.Where(o => o.Email == email);

                        if (query.Count() > 0)
                        {
                            var userGUID = query.ToList()[0].guid;
                            var userLocationGUID = db.user_to_location.Where(o => o.user_guid == userGUID);

                            if (userLocationGUID.Count() > 0)
                            {
                                var userLocationGUIDs = userLocationGUID.ToList()[0].location_guid;
                                var location = db.locationinfoes.Where(o => o.guid == userLocationGUIDs);

                                if (location.Count() > 0)
                                {
                                    var parentGUID = location.ToList()[0].parentguid;
                                    var parentCompany = db.Companies.Where(o => o.guid == parentGUID);

                                    if (parentCompany.Count() > 0)
                                    {
                                        var isAAA = parentCompany.ToList()[0].guid.ToString() == "04846763-5aa2-442f-9ae1-b4dc36f32388";

                                        if (!isAAA)
                                        {
                                            var usersGUIDsOnThatLocation = db.user_to_location.Where(o => o.location_guid == userLocationGUIDs);

                                            if (usersGUIDsOnThatLocation.Count() > 0)
                                            {
                                                var listOfUsersGUIDsOnThatLocation = usersGUIDsOnThatLocation.ToList();

                                                foreach (var user in listOfUsersGUIDsOnThatLocation)
                                                {
                                                    var emailOfUserOfThatLocation = db.AspNetUsers.Where(o => o.Id == user.user_guid.ToString() && o.account_status == 1);

                                                    if (emailOfUserOfThatLocation.Count() > 0)
                                                    {
                                                        var userOfThatLocation = emailOfUserOfThatLocation.ToList()[0];
                                                        var isCorpAdmin = db.AspNetUserRoles.Where(o => o.UserId == userOfThatLocation.Id && o.RoleId == "0b2e4862-7ed4-4c3d-9af7-069ab7ba9a25");

                                                        if (isCorpAdmin.Count() == 0)
                                                        {
                                                            eMail.CC.Add(emailOfUserOfThatLocation.ToList()[0].Email);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                eMail.IsBodyHtml = isHTML;
                eMail.Subject = subject;
                eMail.Body = body;
                eMail.Priority = MailPriority.High;

                System.Net.Mail.SmtpClient SMTP = new System.Net.Mail.SmtpClient();
                SMTP.Host = "64.79.170.149";
                //SMTP.Port = 587;
                //SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                SMTP.EnableSsl = false;
                SMTP.UseDefaultCredentials = true;
                SMTP.Credentials = new System.Net.NetworkCredential("support@assetsaaa.com", "Letmein00@");
                //SMTP.DeliveryFormat = SmtpDeliveryFormat.International;                
                SMTP.Send(eMail);
                //await SMTP.SendMailAsync(eMail);
            }
            catch (Exception e)
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "log.txt", DateTime.Now + " => " + e.ToString());
            }
        }
    }
}