using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmAlmacen : Form
    {
        public frmAlmacen()
        {
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                


            }
        }

        private void txtCveProducto_TextChanged(object sender, EventArgs e)
        {
            dgvProductos.Rows.Clear();
                List<PRODUCTOS> productos = metodos_PRODUCTOS.seleccionarPRODUCTO(txtCveProducto.Text);
                foreach(PRODUCTOS producto in productos)
                {
                    dgvProductos.Rows.Add(producto.clave,producto.descripcion, producto.minimo, producto.maximo, producto.existencias, producto.precio1, producto.precio2, producto.precio3, producto.observaciones, producto.marca);
                }
        }

        private void btnExpExcel_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Rows.Count > 0)
            {
                string ruta = string.Empty;
                folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.Desktop;
                folderBrowserDialog1.ShowNewFolderButton = false;
                folderBrowserDialog1.Description = "Selecciona la carpeta";
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    ruta = folderBrowserDialog1.SelectedPath;
                }

                writeCSV(dgvProductos, ruta);
                MessageBox.Show("La información se exportó correctamente", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show( "No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void writeCSV(DataGridView gridIn, string outputFile)
        {
            if (gridIn.RowCount > 0)
            {
                string value = "";
                DataGridViewRow dr = new DataGridViewRow();
                StreamWriter swOut = new StreamWriter(outputFile+ @"\"+ DateTime.Today.ToString("YYYYmmddHHMMss")+".csv");
                for (int i = 0; i <= gridIn.Columns.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        swOut.Write(",");
                    }
                    swOut.Write(gridIn.Columns[i].HeaderText);
                }
                swOut.WriteLine();
                for (int j = 0; j <= gridIn.Rows.Count - 1; j++)
                {
                    if (j > 0)
                    {
                        swOut.WriteLine();
                    }        
                    dr = gridIn.Rows[j];
                    for (int i = 0; i <= gridIn.Columns.Count - 1; i++)
                    {
                        if (i > 0)
                        {
                            swOut.Write(",");
                        }
                        value = dr.Cells[i].Value.ToString();
                        value = value.Replace(',', ' ');
                        value = value.Replace(Environment.NewLine, " ");
                        swOut.Write(value);
                    }
                }
                swOut.Close();
            }
        }
    }
}
