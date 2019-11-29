using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            USUARIO usuario = metodos_USUARIO.login(txtUsuario.Text.Trim(), txtContrasena.Text.Trim());
            if (usuario == null)
            {
                lblMsg.Text = "Accedo denegado";
            }
            else
            {
                this.Hide();
                MenuPrincipal menu = new MenuPrincipal(usuario);
                menu.Show();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "Versión " + Application.ProductVersion;
        }
    }
}
