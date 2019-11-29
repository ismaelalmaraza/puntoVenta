using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class splash : Form
    {
        public splash()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            frmLogin login = new frmLogin();
            login.Show();
            this.Hide();
        }

        private void splash_Load(object sender, EventArgs e)
        {
            string str = diagnostico();
            Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("rutaXML"));
            Directory.CreateDirectory(@"c:\CFDI");
            if (diagnostico()!=string.Empty)
            {
                MessageBox.Show("El sistema no puede continuar por las soguientes causas:\n\n"+ str,"Error en sistema",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Application.Exit();
            }
        }


        private string diagnostico()
        {
            string respuesta=string.Empty;
            if(ConfigurationManager.AppSettings.Get("rutaCer")!="")
            {
                if(!File.Exists(ConfigurationManager.AppSettings.Get("rutaCer")))
                {
                    respuesta += "*No existe la ruta del certificado\n";
                }
            }

            if (ConfigurationManager.AppSettings.Get("rutaKey") != "")
            {
                if (!File.Exists(ConfigurationManager.AppSettings.Get("rutaKey")))
                {
                    respuesta += "* No existe la ruta de la llave del certificado\n";
                }
            }

            if (ConfigurationManager.AppSettings.Get("rutaXML") != "")
            {
                if (!Directory.Exists(ConfigurationManager.AppSettings.Get("rutaXML")))
                {
                    respuesta += "* No existe la ruta de la llave privada del certificado\n";
                }
            }

            if (ConfigurationManager.AppSettings.Get("keypass") == "")
            {
                respuesta += "* No se ha asignado la contraseña de la llave privada\n";
            }

            if (ConfigurationManager.AppSettings.Get("rfcEmisor") == "")
            {
                respuesta += "* No se ha asignado RFC  del emisor\n";
            }

            if (ConfigurationManager.AppSettings.Get("LugarExpedicion") == "")
            {
                respuesta += "* No se ha asignado lugar de expedición\n";
            }

            if (ConfigurationManager.AppSettings.Get("noCertificado") == "")
            {
                respuesta += "* No se ha asignado número de certificado\n";
            }

            if (ConfigurationManager.AppSettings.Get("RegimenFiscal") == "")
            {
                respuesta += "* No se ha asignado un régimen fiscal\n";
            }

            if (ConfigurationManager.AppSettings.Get("Empresa") == "")
            {
                respuesta += "* No se ha asignado un nombre de la empresa\n";
            }
            return respuesta;
        }
    }
}
