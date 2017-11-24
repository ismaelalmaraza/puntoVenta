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
        public frmCompras()
        {
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            dgProductos.DataSource = metodos_PRODUCTOS.seleccionarPRODUCTOS();
        }
    }
}
