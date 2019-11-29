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
    public partial class frmCotizaciones : Form
    {
        List<VENTA> cotizaciones;
        Int32 IdCliente;
        public Int32 idVenta=0;
        public frmCotizaciones(int IdCliente)
        {
            InitializeComponent();
            this.IdCliente = IdCliente;
        }

        private void frmCotizaciones_Load(object sender, EventArgs e)
        {
            cotizaciones = metodos_VENTA.obtenerCotizaciones(IdCliente);

            dgvCotizaciones.Rows.Clear();
            foreach (VENTA cotizacion in cotizaciones)
            {
                dgvCotizaciones.Rows.Add(cotizacion.id_venta, cotizacion.fecha,cotizacion.total);
            }
        }

        private void dgvCotizaciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idVenta = Convert.ToInt32( this.dgvCotizaciones.Rows[e.RowIndex].Cells[0].Value.ToString());
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}