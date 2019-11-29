using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmCompras : Form
    {
        string id = string.Empty;
        List<PRODUCTOS> productos;
        string productoCosto;
        public frmCompras()
        {
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            cmbProveedor.DataSource = metodos_PROVEEDORES.seleccionarPROVEEDORES();
            cmbProveedor.DisplayMember = "NOMBRE";
            cmbProveedor.ValueMember = "ID_PROVEEDOR";
            txtTipoCambio.Text =  metodos_PRODUCTOS.seleccionarTipoCambio().ToString();
            //productos = metodos_PRODUCTOS.seleccionarPRODUCTOS();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            productos = metodos_PRODUCTOS.seleccionarPRODUCTOSLst(txtClaveProducto.Text.Trim());
            //List<PRODUCTOS> _productos = (List<PRODUCTOS>)from u in productos
            //                                              where SqlMethods.Like(u.clave, "%" + txtClaveProducto.Text.Trim() + "%")
            //                                 select u;
            if (txtClaveProducto.Text.Trim().Length > 0)
            {
                if (productos.Count() > 0)
                    dgProductosLst.Visible = true;
            }
            else
            {
                dgProductosLst.Visible = false;

            }
            dgProductosLst.Rows.Clear();

            foreach (PRODUCTOS p in productos)
            {
                dgProductosLst.Rows.Add(p.clave, p.descripcion);
            }
            //seleccionarCLIENTES
        }

        private void getProducto(string cve)
        {
            PRODUCTOS producto = metodos_PRODUCTOS.seleccionarPRODUCTOS(cve);
            if (producto.clave != null)
            {
                id = producto.id.ToString();
                txtPrecioActual.Text = producto.costo.ToString();
                lblDescripcion.Text = producto.descripcion + " de $" + producto.costo.ToString() + " " + producto.moneda + " con " + producto.existencias.ToString() + " piezas en inventario";
                productoCosto = producto.costo.ToString();
                txtPrecioActual.Enabled = true;
                txtCantidad.Enabled = true;
                txtCredito.Enabled = true;
                txtFactura.Enabled = true;
                cmbProveedor.Enabled = true;
                txtPrecioActual.Focus();
            }
            else
            {
                if (MessageBox.Show("El producto no existe, ¿Desea darlo de alta?", "Compras", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    frmNvoProducto nuevoProducto = new frmNvoProducto();
                    nuevoProducto.Show();
                    reset(false);
                }
            }
            if (txtPrecioActual.Text != string.Empty)
            {
                txtCantidad.Focus();
            }
            else
                txtPrecioActual.Focus();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                getProducto(txtClaveProducto.Text);
            }
        }

        private void lklCancelar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            reset(false);
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void reset(bool v)
        {
            dateTimePicker1.Enabled = v;
            txtPrecioActual.Enabled = v;
            txtCantidad.Enabled = v;
            txtPrecioActual.Enabled = v;
            txtCantidad.Enabled = v;
            txtCredito.Enabled = v;
            txtFactura.Enabled = v;
            cmbProveedor.Enabled = v;
            txtPrecioActual.Text = string.Empty;
            txtClaveProducto.Text = string.Empty;
            lblDescripcion.Text = string.Empty;
            txtCantidad.Text = string.Empty;
            txtClaveProducto.Focus();
            txtDescuento.Enabled = v;
            txtTipoCambio.Enabled = v;
            rdMNX.Enabled = v;
            rdUSD.Enabled = v;
            dateTimePicker1.Enabled = v;
            txtClaveProducto.Enabled = v;
        }

        private bool valida()
        {
            bool valida = true;
            if (txtDescuento.Text.Trim() == string.Empty) txtDescuento.Text = "0.00";
            if (lblDescripcion.Text == string.Empty)
            { MessageBox.Show("Seleccione un producto", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information); valida = false; }
            if (txtCredito.Text == string.Empty)
            { MessageBox.Show("Ingrese los días de crédito para esta compra", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information); valida = false; }
            if (txtFactura.Text == string.Empty)
               { MessageBox.Show("Ingrese el número de factura de esta compra", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information); valida = false; }
            if (cmbProveedor.SelectedIndex<0)
                {MessageBox.Show("Seleccione un proveedor", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information); valida = false; }
            if (txtPrecioActual.Text == string.Empty)
                {MessageBox.Show("Seleccione un proveedor", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information); valida = false; }
            return valida;
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (txtCantidad.Text != string.Empty)
                {
                    if (valida())
                    {
                        decimal precioPesos = Convert.ToDecimal( txtPrecioActual.Text);
                        if (rdUSD.Checked)
                        {
                            precioPesos = Convert.ToDecimal(txtPrecioActual.Text) * Convert.ToDecimal(txtTipoCambio.Text);
                        }
                        dgCompras.Rows.Add(id, txtClaveProducto.Text, precioPesos, productoCosto, txtCantidad.Text, cmbProveedor.SelectedValue, txtCredito.Text, txtFactura.Text,
                            (precioPesos * Convert.ToDecimal(txtCantidad.Text)).ToString(),((precioPesos * Convert.ToDecimal(txtCantidad.Text))* (Convert.ToDecimal(txtDescuento.Text)))/100 );
                        txtClaveProducto.Text = "";
                        txtPrecioActual.Text = "";
                        txtCantidad.Text = "";
                        lblDescripcion.Text = string.Empty;
                        txtClaveProducto.Focus();
                    }
                }
            }

            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtCantidad.Text.Length; i++)
            {
                if (txtCantidad.Text[i] == '.')
                    IsDec = true;

                if (IsDec && nroDec++ >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            reset(true);
            btnNuevo.Enabled = false;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!metodos_PRODUCTOS.existeFactura(txtFactura.Text.Trim()))
            {
                if (txtDescuento.Text.Trim() == string.Empty) txtDescuento.Text = "0";
                reset(false);
                btnNuevo.Enabled = true;
                btnCancelar.Enabled = false;
                btnGuardar.Enabled = false;

                foreach (DataGridViewRow row in dgCompras.Rows)
                {
                    string a = row.Cells[0].Value.ToString();
                    string b = row.Cells[2].Value.ToString();
                    string c = row.Cells[4].Value.ToString();

                    metodos_PRODUCTOS.actualizarPRODUCTOS(Convert.ToInt32(a), Convert.ToDecimal(b), Convert.ToInt32(c));
                    metodos_PRODUCTOS.insertarPRODUCTOS(Convert.ToInt32(a), Convert.ToDecimal(b), Convert.ToInt32(c), cmbProveedor.SelectedValue.ToString(),Convert.ToInt32(txtCredito.Text), txtFactura.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"),Convert.ToDecimal(txtDescuento.Text),(Convert.ToDecimal(b)* Convert.ToInt32(c)) - Convert.ToDecimal(txtDescuento.Text));
                }
                dgCompras.Rows.Clear();
                MessageBox.Show("Las compras se registraron con éxito", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("La factura "+ txtFactura.Text.Trim() + " ya fue registrada anteriormente", "Compras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset(false);
            btnNuevo.Enabled = true;
            btnCancelar.Enabled = false;
            btnGuardar.Enabled = false;
            dgCompras.Rows.Clear();
        }

        private void sumar()
        {
            try
            {
                decimal total = 0;
                decimal descuento = 0;
                foreach (DataGridViewRow row in dgCompras.Rows)
                {
                    string t = row.Cells["Total"].Value.ToString();
                    string d = row.Cells["descuento"].Value.ToString();
                    total += Convert.ToDecimal(t);
                    descuento += Convert.ToDecimal(d);
                }

                lblSubTotal.Text = String.Format(CultureInfo.InvariantCulture,"{0:0.00}", total);
                lblIVA.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", ((total - descuento) * Convert.ToDecimal(0.16)));
                lblDescuento.Text= String.Format(CultureInfo.InvariantCulture, "{0:0.00}", descuento);
                lblBase.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Convert.ToDecimal(lblSubTotal.Text) - Convert.ToDecimal(lblDescuento.Text)));
                lblTotal.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", (Convert.ToDecimal(lblSubTotal.Text) + Convert.ToDecimal(lblIVA.Text)));
            }
            catch (Exception err)
            {

            }
        }

        private void dgCompras_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            sumar();
        }

        private void dgCompras_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            sumar();
        }

        private void txtCredito_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }


            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtCredito.Text.Length; i++)
            {
                if (txtCredito.Text[i] == '.')
                    IsDec = true;

                if (IsDec && nroDec++ >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;
        }

        private void txtPrecioActual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }


            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtPrecioActual.Text.Length; i++)
            {
                if (txtPrecioActual.Text[i] == '.')
                    IsDec = true;

                if (IsDec && nroDec++ >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;
        }

        private void dgProductosLst_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var cte = (from item in productos
                           where item.clave == this.dgProductosLst.Rows[e.RowIndex].Cells[0].Value.ToString()
                           select item).First();
                txtClaveProducto.Text = cte.clave;
                dgProductosLst.Visible = false;
                getProducto(cte.clave);

            }
            catch (Exception err)
            {

            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Process calc = new Process { StartInfo = { FileName = @"calc.exe" } };
            calc.Start();
        }

        private void dgCompras_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtCredito_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void rdUSD_CheckedChanged(object sender, EventArgs e)
        {
            if (rdUSD.Checked)
                txtTipoCambio.Visible = true;
            if (rdMNX.Checked)
                txtTipoCambio.Visible = false;
        }

        private void rdMNX_CheckedChanged(object sender, EventArgs e)
        {
            if (rdUSD.Checked)
                txtTipoCambio.Visible = true;
            if (rdMNX.Checked)
                txtTipoCambio.Visible = false;
        }
    }
}