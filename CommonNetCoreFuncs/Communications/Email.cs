﻿using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonNetCoreFuncs.Communications
{
    public class MailAddress
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public static class EmailConfig
    {
        public static SmtpClient SsmtpClient { get; private set; }
        public static async Task InitializeSmtp(string smtpServer, int smtpPort)
        {
            if (!SsmtpClient.IsConnected)
            {
                SmtpClient client = new();
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.None);
                SsmtpClient = client;
            }
        }
    }

    public static class Email
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<bool> SendEmail(string smtpServer, int smtpPort, MailAddress from, List<MailAddress> toAddresses, string subject, string body, bool bodyIsHtml = false, List<MailAddress> ccAddresses = null, string attachmentName = null, FileStream fileData = null)
        {
            bool success = true;
            try
            {
                //Confirm emails
                if (!ConfirmValidEmail(from?.Email ?? ""))
                {
                    success = false;
                }

                if (success && toAddresses.Any())
                {
                    foreach (MailAddress mailAddress in toAddresses)
                    {
                        if (!ConfirmValidEmail(mailAddress?.Email ?? ""))
                        {
                            success = false;
                            break;
                        }
                    }
                }
                else
                {
                    success = false;
                }


                if (success && ccAddresses != null)
                {
                    if (ccAddresses.Any())
                    {
                        foreach (MailAddress mailAddress in ccAddresses)
                        {
                            if (!ConfirmValidEmail(mailAddress?.Email ?? ""))
                            {
                                success = false;
                                break;
                            }
                        }
                    }
                }

                if (success)
                {
                    MimeMessage email = new();
                    email.From.Add(new MailboxAddress(from.Name, from.Email));
                    email.To.AddRange(toAddresses.Select(x => new MailboxAddress(x.Name, x.Email)).ToList());
                    if (ccAddresses != null)
                    {
                        if (ccAddresses.Any())
                        {
                            email.Cc.AddRange(ccAddresses.Select(x => new MailboxAddress(x.Name, x.Email)).ToList());
                        }
                    }
                    email.Subject = subject;

                    BodyBuilder bodyBuilder = new();
                    if (bodyIsHtml) { bodyBuilder.HtmlBody = body; }
                    else { bodyBuilder.TextBody = body; }

                    bodyBuilder.TextBody = body;
                    if (!string.IsNullOrEmpty(attachmentName) && fileData != null)
                    {
                        bodyBuilder.Attachments.Add(attachmentName, fileData);
                    }

                    email.Body = bodyBuilder.ToMessageBody();

                    //using SmtpClient smtpClient = new();
                    if (!EmailConfig.SsmtpClient.IsConnected)
                    {
                        await EmailConfig.InitializeSmtp();
                    }
                    //smtpClient.Authenticate("user", "password");
                    await EmailConfig.SsmtpClient.SendAsync(email);
                    //await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                success = false;
            }
            return success;
        }

        public static bool ConfirmValidEmail(string email)
        {
            bool isValid = false;
            try
            {
                isValid = Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
            }
            return isValid;
        }
    }
}
