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
            try
            {                
                System.Net.Mail.MailMessage eMail = new System.Net.Mail.MailMessage();
                eMail.From = new System.Net.Mail.MailAddress("support@assetsaaa.com");

                if (email != "")
                {
                    eMail.To.Add(email);
                }

                eMail.Bcc.Add("riosconcepts@gmail.com");

                //When ticket is first entered – email goes to order@aaacompanies.com, erios@aaacompanies.com, sdollen@aaacompanies.com, bhiggins@aaacompanies.com, the user that entered the ticket and every user attached to that site / location.

                //Anytime a ticket is updated, changed, etc.an email needs to go out to the following people:  erios@aaacompanies.com, sdollen@aaacompanies.com, bhiggins@aaacompanies.com, the user that entered the ticket and every user attached to that site / location.

                if (subject == "Web Portal Service Ticket Entered")
                {
                    eMail.To.Add("order@aaacompanies.com");
                }

                if (subject == "AAAWebPortalCommentsErrors")
                {
                    eMail.CC.Add("schristopulos@aaacompanies.com");
                    eMail.CC.Add("JKatinos@AAACompanies.com");
                }

                if (subject != "Web Portal User Invitation")
                {
                    eMail.CC.Add("erios@aaacompanies.com");
                    eMail.CC.Add("sdollen@aaacompanies.com");
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
                eMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

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