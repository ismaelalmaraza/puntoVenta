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
using System.Configuration;
using Win32ComplementoPago;

namespace Vendo
{
    public partial class MenuPrincipal : Form
    {
        USUARIO usuario;
        public MenuPrincipal(USUARIO usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            lbluser.Text ="USUARIO " + usuario.usuario;
            //validaversion();usua
            if(usuario.nivel== "ADMINISTRADOR")
            {
                lCliente.Enabled = true;
                linkLabel3.Enabled = true;
                linkLabel2.Enabled = true;
                linkLabel4.Enabled = true;
                linkLabel5.Enabled = true;

                panel9.Enabled = true;
                label10.Enabled = true;
                label9.Enabled = true;
                pictureBox5.Enabled = true;

                panel8.Enabled = true;
                label8.Enabled = true;
                label7.Enabled = true;
                pictureBox4.Enabled = true;

                panel7.Enabled = true;
                label6.Enabled = true;
                label5.Enabled = true;
                pictureBox3.Enabled = true;
                lblCompleento.Enabled = true;

                panel6.Enabled = true;
                label3.Enabled = true;
                label4.Enabled = true;
                pictureBox2.Enabled = true;
            }
        }

        private void validaversion()
        {
            string version_actual = ConfigurationManager.AppSettings.Get("VERSION");
            string versio_nueva = metodos_VERSION.seleccionarVERSION();

            if(version_actual!= versio_nueva)
            {
                MessageBox.Show("Hay una nueva versión disponoble");
            }
        }
        private void MenuPrincipal_Load(object sender, EventArgs e)
        {

            //Centrar el Panel
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize; //Captura el Tamaño del Monitor
            Int32 ancho = (this.Width - panel3.Width) / 2;
            panel3.Location = new Point(ancho, panel3.Location.Y);
            //lblfecha.Text = DateTime.Now.ToLongDateString();
            lblfecha.Text = DateTime.Now.ToShortDateString();
           // lbluser.Text = "Usuario: "+ Program.usuario;
            lblhora.Text = DateTime.Now.ToLongTimeString();


        }

        private void btnmin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btncerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estas seguro que desea Salir?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }


        private void panel6_Click(object sender, EventArgs e)
        {
            showSales();
        }

        private void showSales()
        {
            frmVenta venta = new frmVenta();
            venta.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            showSales();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            showSales();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            showSales();
        }

        private void showBuy()
        {
            frmCompras compra = new frmCompras();
            compra.Show();
        }

        private void lCliente_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmCliente cliente = new frmCliente();
            cliente.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmProveedor proveedores = new frmProveedor();
            proveedores.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmNvoProducto nvoProd = new frmNvoProducto();
            nvoProd.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            showBuy();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            showBuy();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            showBuy();
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            showBuy();
        }

        private void showSInvoices()
        {
            frmFacturas facturas = new frmFacturas();
            facturas.Show();
        }
        private void showStock()
        {
            frmAlmacen almacen = new frmAlmacen();
            almacen.Show();
        }
        private void label8_Click(object sender, EventArgs e)
        {
            showStock();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            showStock();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            showStock();
        }

        private void panel8_Click(object sender, EventArgs e)
        {
            showStock();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmUsuario usuario = new frmUsuario();
            usuario.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblhora.Text = DateTime.Now.ToString("hh:MM:ss");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {
            showSInvoices();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            showSInvoices();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel9_Click(object sender, EventArgs e)
        {
            showSInvoices();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmCargaMasivaProd cargaMasiva = new frmCargaMasivaProd();
            cargaMasiva.Show();
        }

        private void lblCompleento_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 complemento = new Form1();
            complemento.Show();
        }
    }
}
