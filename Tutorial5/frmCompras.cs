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

namespace Tutorial5
{
    public partial class frmCompras : Form
    {
        string id = string.Empty;
        public frmCompras()
        {
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            //dgProductos.DataSource = metodos_PRODUCTOS.seleccionarPRODUCTOS();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                PRODUCTOS producto = metodos_PRODUCTOS.seleccionarPRODUCTOS(txtClaveProducto.Text.Trim());
                if (producto.clave != null)
                {
                    id = producto.id.ToString();
                    lblDescripcion.Text = producto.descripcion;
                    lblPrecio.Text = producto.costo.ToString();
                    lblExistencia.Text = producto.existencias.ToString();
                    txtPrecioActual.Text = producto.costo.ToString();
                    txtPrecioActual.Enabled = true;
                    txtCantidad.Enabled = true;
                    txtPrecioActual.Focus();
                }else
                {
                   if( MessageBox.Show("El producto no existe, ¿Desea darlo de alta?", "Compras", MessageBoxButtons.YesNo, MessageBoxIcon.Information)==DialogResult.Yes)
                   {
                       frmNvoProducto nuevoProducto = new frmNvoProducto();
                       nuevoProducto.Show();
                       reset();
                   }
                }
            }
        }

        private void txtPrecioActual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (txtPrecioActual.Text != string.Empty)
                {
                    txtCantidad.Focus();
                }
                else
                    txtPrecioActual.Focus();
            }
        }

        private void lklCancelar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            reset();
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void reset()
        {
            txtPrecioActual.Enabled = false;
            txtCantidad.Enabled = false;

            txtPrecioActual.Text = string.Empty;
            txtClaveProducto.Text = string.Empty;
            txtCantidad.Text = string.Empty;
            txtClaveProducto.Focus();
        }
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (txtCantidad.Text != string.Empty)
                {
                    dgCompras.Rows.Add(id,txtClaveProducto.Text, txtPrecioActual.Text, txtCantidad.Text);
                    reset();
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgCompras.Rows)
            {
                string a = row.Cells[0].Value.ToString();
                string b = row.Cells[2].Value.ToString();
                string c = row.Cells[3].Value.ToString();
                metodos_PRODUCTOS.actualizarPRODUCTOS(Convert.ToInt32(a), Convert.ToDecimal(b), Convert.ToInt32(c));
                metodos_PRODUCTOS.insertarPRODUCTOS(Convert.ToInt32(a), Convert.ToDecimal(b), Convert.ToInt32(c));
            }
            dgCompras.Rows.Clear();
            MessageBox.Show("Las compras se registraron con éxito", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
