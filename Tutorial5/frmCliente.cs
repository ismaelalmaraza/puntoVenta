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
    public partial class frmCliente : Form
    {
        bool esNuevo = false;
        public frmCliente()
        {
            InitializeComponent();
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            dgClientes.DataSource = metodos_CLIENTES.seleccionarCLIENTES();
            habilitarTextBox(false);
        }

        private bool ValidarTextBox()
        {
            bool validado = true;

            if (txtNombre.Text == string.Empty)
            { errorProvider1.SetError(txtNombre, "El nobre del cliente no puede estar vacío"); validado = false; }
            if (txtDireccion.Text == string.Empty)
            { errorProvider1.SetError(txtDireccion, "Falta la dirección del cliente"); validado = false; }
            if (txtTelefono.Text == string.Empty)
            { errorProvider1.SetError(txtTelefono, "Falta el teléfono del cliente"); validado = false; }
            if (txtCredito.Text == string.Empty)
            { errorProvider1.SetError(txtCredito, "Ingrese el limite de crádito"); validado = false; }
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
                    metodos_CLIENTES.insertarCLIENTES(0, txtNombre.Text.Trim(), txtDireccion.Text.Trim(), txtCP.Text.Trim(),
                        txtTelefono.Text.Trim(), txtContacto.Text.Trim(), Convert.ToDouble(txtCredito.Text), txtRFC.Text, cmbPrecio.Text, txtEmail.Text);
                }
                else
                {
                    metodos_CLIENTES.actualizarCLIENTES(Convert.ToInt32(lblCveCte.Text), txtNombre.Text.Trim(), txtDireccion.Text.Trim(), txtCP.Text.Trim(),
    txtTelefono.Text.Trim(), txtContacto.Text.Trim(), Convert.ToInt32(txtCredito.Text), txtRFC.Text, cmbPrecio.Text, txtEmail.Text);
                }
                dgClientes.DataSource = metodos_CLIENTES.seleccionarCLIENTES();
            }
        }

        private void habilitarTextBox(bool habil)
        {
            cmbPrecio.Enabled = habil;
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
                metodos_CLIENTES.elimiarCLIENTES(Convert.ToInt32(lblCveCte.Text));
                dgClientes.DataSource = metodos_CLIENTES.seleccionarCLIENTES();
            }
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

        private void dgClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtEmail.Text       =   this.dgClientes.Rows[e.RowIndex].Cells[0].Value.ToString();
                cmbPrecio.Text      =   this.dgClientes.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtNombre.Text      =   this.dgClientes.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtCP.Text          =   this.dgClientes.Rows[e.RowIndex].Cells[5].Value.ToString();
                txtCredito.Text     =   this.dgClientes.Rows[e.RowIndex].Cells[8].Value.ToString();
                txtDireccion.Text   =   this.dgClientes.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtRFC.Text         =   this.dgClientes.Rows[e.RowIndex].Cells[9].Value.ToString();
                txtTelefono.Text    =   this.dgClientes.Rows[e.RowIndex].Cells[6].Value.ToString();
                txtContacto.Text    =   this.dgClientes.Rows[e.RowIndex].Cells[7].Value.ToString();
                lblCveCte.Text      =   this.dgClientes.Rows[e.RowIndex].Cells[1].Value.ToString().PadLeft(5, '0');

            }catch(Exception err)
            {

            }
        }

        private void dgClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
