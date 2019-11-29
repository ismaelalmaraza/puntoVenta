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
    public partial class frmUsuario : Form
    {
        bool esNuevo = false;
        string idUsuario;
        public frmUsuario()
        {
            InitializeComponent();
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            dgClientes.DataSource = metodos_USUARIO.seleccionarUSUARIO();
            habilitarTextBox(false);
        }

        private bool ValidarTextBox()
        {
            bool validado = true;
            errorProvider1.Clear();
            if (txtNombre.Text == string.Empty)
            { errorProvider1.SetError(txtNombre, "El nobre del cliente no puede estar vacío"); validado = false; }
            if (txtPass1.Text == string.Empty)
            { errorProvider1.SetError(txtPass1, "Ingrese una contraseña"); validado = false; }
            if (txtPass2.Text == string.Empty)
            { errorProvider1.SetError(txtPass2, "Repita la contrseña"); validado = false; }

            if (cmbNivel.SelectedIndex<0)
            { errorProvider1.SetError(cmbNivel, "Seleccione el nivel de acceso"); validado = false; }

            if (cmbEstatus.SelectedIndex < 0)
            { errorProvider1.SetError(cmbEstatus, "Seleccione el estado"); validado = false; }
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

        private bool validarPsw()
        {
            bool correcta = true;
            if(txtPass1.Text.Trim()!= txtPass2.Text.Trim())
            {
                errorProvider1.SetError(txtPass2, "Las cotraseñas no coinciden"); correcta = false;
                txtPass1.Text = "";
                txtPass2.Text = "";
            }
            if(txtPass1.Text.Trim().Length<6)
            {
                errorProvider1.SetError(txtPass2, "Las cotraseñas debe ser de al menos 6 caracteres"); correcta = false;
                txtPass1.Text = "";
                txtPass2.Text = "";
            }
            return correcta;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarTextBox() && validarPsw())
            {
                habilitarTextBox(false);
                btnNuevo.Enabled = true;
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Enabled = false;
                btnGuardar.Enabled = false;

                if (esNuevo)
                {
                    metodos_USUARIO.insertarUSUARIO(0, txtNombre.Text.Trim(), txtPass1.Text, cmbEstatus.Text, cmbNivel.Text);
                }
                else
                {
                    metodos_USUARIO.actualizarUSUARIO(Convert.ToInt32( idUsuario), txtNombre.Text.Trim(), txtPass1.Text, cmbEstatus.Text, cmbNivel.Text);
                }
                dgClientes.DataSource = metodos_USUARIO.seleccionarUSUARIO();
            }
        }

        private void habilitarTextBox(bool habil)
        {
            cmbEstatus.Enabled = habil;
            cmbNivel.Enabled = habil;
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
                //metodos_CLIENTES.elimiarCLIENTES(Convert.ToInt32(lblCveCte.Text));
                dgClientes.DataSource = metodos_CLIENTES.seleccionarCLIENTES();
            }
        }

        private void dgClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                idUsuario = this.dgClientes.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtNombre.Text = this.dgClientes.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtPass1.Text = this.dgClientes.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtPass2.Text = this.dgClientes.Rows[e.RowIndex].Cells[2].Value.ToString();
                cmbEstatus.Text = this.dgClientes.Rows[e.RowIndex].Cells[3].Value.ToString();
                cmbNivel.Text = this.dgClientes.Rows[e.RowIndex].Cells[4].Value.ToString();
            }
            catch(Exception err)
            {

            }
        }

        private void dgClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
