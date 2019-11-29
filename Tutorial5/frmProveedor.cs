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
    public partial class frmProveedor : Form
    {
        bool esNuevo = false;
        public frmProveedor()
        {
            InitializeComponent();
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            dgProveedor.DataSource = metodos_PROVEEDORES.seleccionarPROVEEDORES();
            habilitarTextBox(false);
        }

        private bool ValidarTextBox()
        {
            bool validado = true;

            if (txtNombre.Text == string.Empty)
            { errorProvider1.SetError(txtNombre, "El nobre del proveedor no puede estar vacío"); validado = false; }
            if (txtDireccion.Text == string.Empty)
            { errorProvider1.SetError(txtDireccion, "Falta la dirección del proveedor"); validado = false; }
            if (txtTelefono.Text == string.Empty)
            { errorProvider1.SetError(txtTelefono, "Falta el teléfono del proveedor"); validado = false; }
            return validado;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            habilitarTextBox(true);
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
            btnEditar.Enabled = false;
            btnNuevo.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarTextBox())
            {
                habilitarTextBox(false);
                btnNuevo.Enabled = true;
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Enabled = false;
                btnGuardar.Enabled = false;

                if (esNuevo)
                {
                    metodos_PROVEEDORES.insertarPROVEEDORES(0, txtNombre.Text.Trim(), txtDireccion.Text.Trim(), txtTelefono.Text.Trim(),txtContacto.Text.Trim(), txtEmail.Text.Trim(),txtRFC.Text);
                }
                else
                {
                    metodos_PROVEEDORES.actualizarPROVEEDORES(Convert.ToInt32(lblCveProv.Text), txtNombre.Text.Trim(), txtDireccion.Text.Trim(), txtTelefono.Text.Trim(), txtContacto.Text.Trim(), txtEmail.Text.Trim(), txtRFC.Text);
                }
                dgProveedor.DataSource = metodos_PROVEEDORES.seleccionarPROVEEDORES();
            }
        }

        private void habilitarTextBox(bool habil)
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    TextBox text = (TextBox)c;
                    text.Enabled = habil;
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            habilitarTextBox(false);
            btnNuevo.Enabled = true;
            btnEditar.Enabled = true;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = false;
            btnGuardar.Enabled = false;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Esta seguro de eliminar el cliente " + txtNombre.Text + "?", "Eliminar...", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                habilitarTextBox(false);
                metodos_CLIENTES.elimiarCLIENTES(Convert.ToInt32(lblCveProv.Text));
                dgProveedor.DataSource = metodos_CLIENTES.seleccionarCLIENTES();
            }
        }



        private void dgClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtNombre.Text = this.dgProveedor.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtEmail.Text = this.dgProveedor.Rows[e.RowIndex].Cells[5].Value.ToString();
                txtDireccion.Text = this.dgProveedor.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtRFC.Text = this.dgProveedor.Rows[e.RowIndex].Cells[6].Value.ToString();
                txtTelefono.Text = this.dgProveedor.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtContacto.Text = this.dgProveedor.Rows[e.RowIndex].Cells[4].Value.ToString();
                lblCveProv.Text = this.dgProveedor.Rows[e.RowIndex].Cells[0].Value.ToString().PadLeft(5, '0');
            }catch(Exception err)
            {

            }
        }
    }
}
