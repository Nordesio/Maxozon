﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext
{
    public class SendMessage
    {
        public static MailMessage CreateMail(string emailTo, int code)
        {
            var from = new MailAddress("testovik402@gmail.com", "Maxozon");
            var to = new MailAddress(emailTo);
            var mail = new MailMessage(from, to);
            mail.Subject = "Продолжите регистрацию в Maxozon";
            mail.Body = "Для продолжения регистрации введите следующий верификационный код: \n" + code;
            mail.IsBodyHtml = true;
            return mail;
        }

        public static void SendMail(MailMessage mail)
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("testovik402@gmail.com", "aeiatsozzwodwbop");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }
    }
}
