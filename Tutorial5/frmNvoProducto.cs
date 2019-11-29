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
    public partial class frmNvoProducto : Form
    {
        bool esNuevo=false;
        public frmNvoProducto()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limparTextBox();
            esNuevo = true;
            habilitarTextBox(true);
            lblCveCte.Enabled = true;
            btnNuevo.Enabled = false;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            habilitarTextBox(true);
            lblCveCte.Enabled = false;
            btnEditar.Enabled = false;
            btnNuevo.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private bool ValidarTextBox()
        {
            if (txtStock.Text == string.Empty)
            {
                txtStock.Text = "0";
            }
            bool validado = true;
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    TextBox text = (TextBox)c;
                    if (text.Text == string.Empty)
                    {
                        MessageBox.Show("El campo " + text.Name.Replace("txt", "") + " no puede estar vacío");
                        validado = false;
                    }
                }
            }
            return validado;
        }



        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarTextBox())
            {
                decimal p1 = 0;
                decimal p2 = 0;
                decimal p3 = 0;
                if(optMonto.Checked)
                {
                    p1 = Convert.ToDecimal(txtPrecio1.Text);
                    p2 = Convert.ToDecimal(txtPrecio2.Text);
                    p3 = Convert.ToDecimal(txtPrecio3.Text);
                }else if(optPorcentaje.Checked)
                {
                    p1 = Convert.ToDecimal(txtCosto.Text) +((Convert.ToDecimal(txtCosto.Text) * Convert.ToDecimal(txtPrecio1.Text)) / 100);
                    p2 = Convert.ToDecimal(txtCosto.Text) +((Convert.ToDecimal(txtCosto.Text) * Convert.ToDecimal(txtPrecio2.Text)) / 100);
                    p3 = Convert.ToDecimal(txtCosto.Text) +((Convert.ToDecimal(txtCosto.Text) * Convert.ToDecimal(txtPrecio3.Text)) / 100);
                }

                habilitarTextBox(false);
                btnNuevo.Enabled = true;
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Enabled = false;
                btnGuardar.Enabled = false;
                if (esNuevo)
                {
                    metodos_PRODUCTOS.insertarPRODUCTOS(0, txtClave.Text.Split('-')[0].Trim(), txtDescripcion.Text, txtLinea.SelectedValue.ToString(), txtCveUnidadSAT.Text
                        , Convert.ToInt32(txtMin.Text), Convert.ToInt32(txtMax.Text), txtMoneda.Text, 0, Convert.ToDecimal(txtCosto.Text), 0, p1, p2, p3,"", txtObservaciones.Text, Convert.ToInt32(cmbProveedor.SelectedValue) ,txtMarca.SelectedValue.ToString(), txtCveUnidadSAT.SelectedValue.ToString(), txtCveDervicioSAT.SelectedValue.ToString(),"","","");
                }
                else
                {
                    metodos_PRODUCTOS.actualizarPRODUCTOS(Convert.ToInt32( lblCveCte.Text), txtClave.Text.Split('-')[0].Trim(), txtDescripcion.Text, txtLinea.SelectedValue.ToString(), txtCveUnidadSAT.Text
    , Convert.ToInt32(txtMin.Text), Convert.ToInt32(txtMax.Text), txtMoneda.Text, 0, Convert.ToDecimal(txtCosto.Text), 0, p1, p2, p3, "", txtObservaciones.Text, Convert.ToInt32(cmbProveedor.SelectedValue), txtMarca.SelectedValue.ToString(), txtCveUnidadSAT.SelectedValue.ToString(), txtCveDervicioSAT.SelectedValue.ToString(), "", "", "");
                }
                dgProductos.DataSource = metodos_PRODUCTOS.seleccionarPRODUCTOS();
                limparTextBox();
            }
        }

        private void limparTextBox()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    TextBox text = (TextBox)c;
                    text.Text = string.Empty;
                }
            }
        }


        private void habilitarTextBox(bool habil)
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox || c is ComboBox)
                {
                    if(c is TextBox)
                    {
                        TextBox text = (TextBox)c; text.Enabled = habil;
                    }else if(c is ComboBox)
                    {
                        ComboBox text = (ComboBox)c; text.Enabled = habil;
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limparTextBox();
            habilitarTextBox(false);
            btnNuevo.Enabled = true;
            btnEditar.Enabled = true;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = false;
            btnGuardar.Enabled = false;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Esta seguro de eliminar el cliente " + txtClave.Text + "?", "Eliminar...", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                habilitarTextBox(false);
                metodos_PRODUCTOS.elimiarPRODUCTOS(Convert.ToInt32(lblCveCte.Text));
                dgProductos.DataSource = metodos_PRODUCTOS.seleccionarPRODUCTOS();
            }
        }

        private void frmNvoProducto_Load(object sender, EventArgs e)
        {
            habilitarTextBox(false);
            actualizar();
        }

        private void actualizar()
        {
            dgProductos.DataSource = metodos_PRODUCTOS.seleccionarPRODUCTOS();
            cmbProveedor.DataSource = metodos_PROVEEDORES.seleccionarPROVEEDORES();
            cmbProveedor.DisplayMember="NOMBRE" ;
            cmbProveedor.ValueMember = "ID_PROVEEDOR";

            txtClave.DataSource = metodos_PRODUCTOS.obtenerNumParte();

            txtCveUnidadSAT.DataSource = metodos_C_CVEUNIDAD.seleccionarC_CVEUNIDAD(); 
            txtCveUnidadSAT.DisplayMember = "Nombre";
            txtCveUnidadSAT.ValueMember = "c_claveunidad";

            txtCveDervicioSAT.DataSource = metodos_C_CLAVESERVICIO.seleccionarC_CLAVESERVICIO();
            txtCveDervicioSAT.DisplayMember = "Descripcion";
            txtCveDervicioSAT.ValueMember = "c_claveprodserv";

            metodos m = new metodos();
            txtLinea.DataSource = m.getLineas();
            txtLinea.DisplayMember = "linea";
            txtLinea.ValueMember = "id";

            txtMarca.DataSource = m.getMarca();
            txtMarca.DisplayMember = "marca";
            txtMarca.ValueMember = "id";
        }
        private void dgProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //id, clave, descripcion, linea, unidadEntrada, minimo, maximo, moneda, existencias, costo, cveUnidad, precio1, precio2, precio3, foto, observaciones
                lblCveCte.Text = this.dgProductos.Rows[e.RowIndex].Cells["id"].Value.ToString().PadLeft(5, '0');
                //txtCveUnidad.Text       = this.dgProductos.Rows[e.RowIndex].Cells["cveUnidad"].Value.ToString();
                txtLinea.Text = this.dgProductos.Rows[e.RowIndex].Cells["linea"].Value.ToString();
                txtMin.Text = this.dgProductos.Rows[e.RowIndex].Cells["minimo"].Value.ToString();
                txtMax.Text = this.dgProductos.Rows[e.RowIndex].Cells["maximo"].Value.ToString();
                txtMoneda.Text = this.dgProductos.Rows[e.RowIndex].Cells["moneda"].Value.ToString();
                txtCosto.Text = this.dgProductos.Rows[e.RowIndex].Cells["costo"].Value.ToString();
                //txtCveUnidad.Text       = this.dgProductos.Rows[e.RowIndex].Cells["cveUnidad"].Value.ToString();

                txtPrecio1.Text = this.dgProductos.Rows[e.RowIndex].Cells["precio1"].Value.ToString();
                txtPrecio2.Text = this.dgProductos.Rows[e.RowIndex].Cells["precio2"].Value.ToString();
                txtPrecio3.Text = this.dgProductos.Rows[e.RowIndex].Cells["precio3"].Value.ToString();
                txtClave.Text = this.dgProductos.Rows[e.RowIndex].Cells["clave"].Value.ToString();
                txtDescripcion.Text = this.dgProductos.Rows[e.RowIndex].Cells["descripcion"].Value.ToString();
                txtObservaciones.Text = this.dgProductos.Rows[e.RowIndex].Cells["observaciones"].Value.ToString();
                //txtImagen.Text          = this.dgProductos.Rows[e.RowIndex].Cells["foto"].Value.ToString();
                txtMarca.Text = this.dgProductos.Rows[e.RowIndex].Cells["marca"].Value.ToString();
                txtStock.Text = this.dgProductos.Rows[e.RowIndex].Cells["existencias"].Value.ToString();
                txtCveDervicioSAT.Text = this.dgProductos.Rows[e.RowIndex].Cells["claveServicio"].Value.ToString();
                txtMarca.Text = this.dgProductos.Rows[e.RowIndex].Cells["claveUnidad"].Value.ToString();

                txtLinea.SelectedValue = Convert.ToInt32(this.dgProductos.Rows[e.RowIndex].Cells[5].Value.ToString());
                // txtCveUnidadSAT.SelectedValue= (Convert.ToInt64(this.dgProductos.Rows[e.RowIndex].Cells["claveUnidad"].Value.ToString())).ToString();
            }catch(Exception err)
            {

            }
        }

        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }


            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtCosto.Text.Length; i++)
            {
                if (txtCosto.Text[i] == '.')
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

        private void txtCveUnidad_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtPrecio1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }


            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtPrecio1.Text.Length; i++)
            {
                if (txtPrecio1.Text[i] == '.')
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

        private void txtPrecio2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }


            bool IsDec = false;
            int nroDec = 0;

            for (int i = 0; i < txtPrecio2.Text.Length; i++)
            {
                if (txtPrecio2.Text[i] == '.')
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

        private void txtPrecio3_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtMin_KeyPress(object sender, KeyPressEventArgs e)
        {
 

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtMax_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void dgProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtPrecio3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void txtPrecio2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void txtPrecio1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void txtCosto_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void txtClave_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDescripcion.Text = txtClave.Text.Substring( txtClave.Text.IndexOf('-')+1);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            actualizar();
        }
    }
}
