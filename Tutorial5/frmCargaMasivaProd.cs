using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmCargaMasivaProd : Form
    {
        int numReg = 0;
        DataSet dataSet = null;
        DataTable dt;

        public frmCargaMasivaProd()
        {
            InitializeComponent();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {

            string file_name = string.Empty;
            string dir="";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Excel Worksheets|*.xls; *.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dir = openFileDialog1.FileName;
            }
            LLenarGrid(dir);
        }

        OleDbConnection conexion = null; //ToDo
        DataTable schemaTable;
        private void LLenarGrid(string archivo)
        {
            numReg = 0;


            OleDbDataAdapter dataAdapter = null;
            string consultaHojaExcel;
            string cadenaConexionArchivoExcel = "provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + archivo + "';Extended Properties=Excel 12.0;";
            try
            {
                conexion = new OleDbConnection(cadenaConexionArchivoExcel);
                conexion.Open();

                schemaTable = conexion.GetOleDbSchemaTable(
                OleDbSchemaGuid.Tables,
                new object[] { null, null, null, "TABLE" });
                string nombreHoja = schemaTable.Rows[0]["TABLE_NAME"].ToString();

                consultaHojaExcel = "Select * from [" + nombreHoja + "]";


                dataAdapter = new OleDbDataAdapter(consultaHojaExcel, conexion);
                dataSet = new DataSet();
                //TODO VERIFICAR NOMBRE DE LA HOJA

                dataAdapter.Fill(dataSet, nombreHoja.Replace("$", ""));
                dt = dataSet.Tables[0];
                if(dt.Columns.Count!=17)
                {
                    MessageBox.Show("El formato del archivo no contiene las columnas necesarias para ejecutar este proceso", "Carga Masiva de Productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    numReg++;
                }

                progressBar1.Maximum = numReg;
                lblCantReg.Text = "Estructura del layout correcto\n\n" + numReg.ToString() + " registros identificados";
                dataGridView1.DataSource = dt;
                //conexion.Close();
                //btnProcesar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, Verificar el archivo o el nombre de la hoja", ex.Message);
            }
        }

        private void btnCargaraBD_Click(object sender, EventArgs e)
        {
            String clave;
            int nuevos = 0;
            int actualizados = 0;
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                Int32 id=0;
                try
                {
                    clave = r.Cells[0].Value.ToString().ToUpper();
                }catch(Exception Err)
                {
                    MessageBox.Show("Se cargaron " + nuevos.ToString() + " nuevos articulos\n\nSe actualizaron " + actualizados.ToString() + " articulos\n\n\n\nLa operación ha finalizado con Éxito", "Carga Masiva de Productos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Value = 0;
                    dataGridView1.DataSource = null;
                    return;
                }
                String descripcion                          = r.Cells[1].Value.ToString().ToUpper();
                String linea                                = r.Cells[2].Value.ToString().ToUpper();
                String unidadentrada                        = r.Cells[3].Value.ToString().ToUpper();
                Int32 minimo                                = string.IsNullOrEmpty(r.Cells[4].Value.ToString()) ? 0 : Int32.Parse(r.Cells[4].Value.ToString());
                Int32 maximo                                = string.IsNullOrEmpty(r.Cells[5].Value.ToString()) ? 0 : Int32.Parse(r.Cells[5].Value.ToString());
                String moneda                               = r.Cells[6].Value.ToString();
                Int32 existencias                           = string.IsNullOrEmpty(r.Cells[7].Value.ToString()) ? 0 : Int32.Parse(r.Cells[7].Value.ToString()); 
                Decimal costo                               = string.IsNullOrEmpty(r.Cells[8].Value.ToString()) ? 0 : decimal.Parse(r.Cells[8].Value.ToString());
                Int32 cvesat                                = string.IsNullOrEmpty(r.Cells[9].Value.ToString()) ? 0 : Int32.Parse(r.Cells[9].Value.ToString());
                Decimal precio1                             = string.IsNullOrEmpty(r.Cells[10].Value.ToString()) ? 0 : decimal.Parse(r.Cells[10].Value.ToString());
                Decimal precio2                             = string.IsNullOrEmpty(r.Cells[11].Value.ToString()) ? 0 : decimal.Parse(r.Cells[11].Value.ToString());
                Decimal precio3                             = string.IsNullOrEmpty(r.Cells[12].Value.ToString()) ? 0 : decimal.Parse(r.Cells[12].Value.ToString());
                string foto                                 = "";
                string observaciones                        = "";
                int fk_proveedor                            = 0;
                String marca                                = r.Cells[13].Value.ToString().ToUpper();
                String ClaveUnidad                          = r.Cells[15].Value.ToString().ToUpper();
                String ClaveServicio                        = r.Cells[9].Value.ToString().ToUpper();
                String foliofactura                         = "";
                String emision                              = "";
                String diascredito                          = "";
                progressBar1.Value += 1;
                PRODUCTOS p = metodos_PRODUCTOS.seleccionarPRODUCTOS(clave);
                if (p.clave == null)
                {
                    nuevos++;
                    metodos_PRODUCTOS.insertarPRODUCTOS(id, clave, descripcion, linea, unidadentrada, minimo, maximo, moneda, existencias, costo, cvesat, precio1, precio2, precio3, foto, observaciones, fk_proveedor, marca, ClaveUnidad, ClaveServicio, foliofactura, emision, diascredito);
                }
                else if (p != null)
                {
                    actualizados++;
                    metodos_PRODUCTOS.actualizarPRODUCTOSMasiva(id, clave, descripcion, linea, unidadentrada, minimo, maximo, moneda, existencias, costo, cvesat, precio1, precio2, precio3, foto, observaciones, fk_proveedor, marca, ClaveUnidad, ClaveServicio, foliofactura, emision, diascredito);
                }
            }
            MessageBox.Show("Se cargaron "+ nuevos.ToString()+ " nuevos articulos\n\nSe actualizaron "+ actualizados.ToString()+ " articulos\n\n\n\nLa operación ha finalizado con Éxito","Carga Masiva de Productos",MessageBoxButtons.OK,MessageBoxIcon.Information);
            progressBar1.Value = 0;
            dataGridView1.DataSource = null;

        }
    }
}
