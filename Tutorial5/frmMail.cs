using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmMail : Form
    {
        string file;
        string mail;
        string pdf;
        bool esMiPago=false;
        public frmMail(string file, string pdf, string mail, bool esPago)
        {
            InitializeComponent();
            this.file = file;
            this.pdf = pdf;
            this.esMiPago = true;
            txtEmail.Text = mail;
        }

        public frmMail(string file,string pdf,string mail)
        {
            InitializeComponent();
            this.file = file;
            this.pdf = pdf;
            txtEmail.Text = mail;
        }

        private void frmMail_Load(object sender, EventArgs e)
        {
            //this.file = "AUN690327J48_A_43";
            //this.pdf = "AUN690327J48_A_43";
            //txtEmail.Text = "ismael.almaraz@gpintegra.com";
        }

        private void btnSi_Click(object sender, EventArgs e)
        {
            if(rbGmail.Checked)
            {
                gmail();
            }
            else if (rbYahoo.Checked)
            {
                yahoo();
            }

            this.Hide();
        }

        private void yahoo()
        {
            MAIL yahooMail = metodos_MAIL.seleccionarYahoo();
            txtEmail.Text = txtEmail.Text.Replace(',', ';');
            List<string> destinatarios = new List<string>();
            destinatarios = txtEmail.Text.Split(';').ToList();

            string[] datos = pdf.Split('_');
            string remoteUri = "";
            if (esMiPago==true)
                remoteUri = "http://eostechnology.com.mx/EOSFACT/pagofiscal.php?idventa=" + datos[2];
            else
                remoteUri = "http://eostechnology.com.mx/factura/facturafiscal.php?idventa=" + datos[2];

            string smtpAddress = "smtp.mail.yahoo.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "ismaelalmaraz_a98@yahoo.com";
            string password = "zaramla26";
            //string emailTo = "ismaelalmaraz@live.com.mx";
            string subject = "Envio de Factura";
            string body = "Este email fue enviado de manera automática, por favor no responda a este. <br>Para consultar su factura haga clic <a href='" + remoteUri + "'>aquí</a> ";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(yahooMail.email.Trim());
                foreach (string item in destinatarios)
                {
                    mail.To.Add(new MailAddress(item));
                }
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.
                Attachment data2 = new Attachment(file, MediaTypeNames.Application.Octet);
                mail.Attachments.Add(data2);
                //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(yahooMail.email.Trim(), yahooMail.pasw.Trim());
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        bool esGmail = false;
        private void gmail()
        {
            MAIL gmailMail = metodos_MAIL.seleccionarGMAIL();
            txtEmail.Text = txtEmail.Text.Replace(',', ';');
            List<string> destinatarios = new List<string>();
            destinatarios = txtEmail.Text.Split(';').ToList();

            string[] datos = pdf.Split('_');
            string remoteUri = "";
            if (esMiPago==true)
                remoteUri = "http://eostechnology.com.mx/EOSFACT/pagofiscal.php?idventa=" + datos[2];
            else
                remoteUri = "http://eostechnology.com.mx/factura/facturafiscal.php?idventa=" + datos[2];


            Attachment data2 = new Attachment(file, MediaTypeNames.Application.Octet);
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(gmailMail.email),
                Body = "Este email fue enviado de manera automática, por favor no responda a este. <br>Para consultar su factura haga clic <a href='" + remoteUri + "'>aquí</a> ",
                Subject = "Envio de Factura",
                IsBodyHtml = true
            };
            mail.Attachments.Add(data2);
            foreach (string item in destinatarios)
            {
                mail.To.Add(new MailAddress(item));
            }

            SmtpClient smtp = new SmtpClient()
            {

                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(gmailMail.email.Trim(), gmailMail.pasw.Trim()),
                EnableSsl = true
            };
            smtp.Send(mail);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}