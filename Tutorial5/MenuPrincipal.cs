using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tutorial5
{
    public partial class MenuPrincipal : Form
    {
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {

            //Centrar el Panel
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize; //Captura el Tamaño del Monitor
            Int32 ancho = (this.Width - panel3.Width) / 2;
            panel3.Location = new Point(ancho, panel3.Location.Y);
            //lblfecha.Text = DateTime.Now.ToLongDateString();
            lblfecha.Text = DateTime.Now.ToShortDateString();
            lbluser.Text = "Usuario: "+ Program.usuario;
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
            venta.ShowDialog();
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
            frmProveedores proveedores = new frmProveedores();
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
    }
}
