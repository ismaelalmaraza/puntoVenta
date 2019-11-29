using Logica;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Vendo;

namespace Win32ComplementoPago
{
    public partial class Form1 : Form
    {
        //crear un objeto el cual tendrá el resultado final, este objeto es el principal 
        //Comprobante oComprobante;
        ComplementoPago oComprobante;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnCargarCFDI_Click(object sender, EventArgs e)
        {

               // cargarValores(oComprobante);
        }

        private void limpiarValores()
        {
            txtFecha.Text = string.Empty;
            txtReceptor.Text = string.Empty;
            txtSerie.Text = string.Empty;
            txtFolio.Text = string.Empty;
            txtSubtotal.Text = string.Empty;
            txtTotal.Text = string.Empty;
            txtIVA.Text = string.Empty;
            txtUUID.Text = string.Empty;
            lblSaldo.Text = string.Empty;
            lblMetodoPago.Text = string.Empty;
            dgFacturas.Rows.Clear();
        }

        private void cargarValores(ComplementoPago result)
        {
            txtFecha.Text = result.FECHA.ToString();
            txtReceptor.Text = result.Receptor;
            txtFolio.Text = result.Folio;
            txtSerie.Text = result.Serie;
           // txtUsoCFDI.Text = result.Receptor.UsoCFDI;
            //txtSubtotal.Text = result.SubTotal.ToString();
            txtTotal.Text = result.MONTO;
            lblMetodoPago.Text = result.metodoPago.Substring(0,3);
            //txtIVA.Text = result.Impuestos.TotalImpuestosTrasladados.ToString();
            if (result.UUIDRELACIONADO != null)
            {
                txtUUID.Text = result.UUIDRELACIONADO;
                FacturaComplementoPago factura = new FacturaComplementoPago(oComprobante);
                List<ComplementoPago> cp = factura.getPagos(txtUUID.Text.Trim());
                llenardetallePagos(cp);
            }
            //foreach(ComprobanteConcepto c in result.Conceptos)
            //{
            //    dgConceptos.Rows.Add(c.ClaveProdServ,c.Cantidad,c.ClaveUnidad,c.Unidad,c.Descripcion,c.ValorUnitario,c.Importe);
            //}
        }

        private void llenardetallePagos(List<ComplementoPago> cps)
        {
            decimal saldo = 0;
            dgDetPagos.Rows.Clear();
            foreach (ComplementoPago cp in cps)
            {
                dgDetPagos.Rows.Add(cp.UUIDRELACIONADO,cp.FECHA,cp.MONTO);
                saldo += Convert.ToDecimal( cp.MONTO);
            }
            lblSaldo.Text = ( Convert.ToDecimal(txtTotal.Text)- saldo).ToString();
            if( lblSaldo.Text.Trim()=="0")
            {
                btnComplemento.Enabled = false;
            }
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            limpiarValores();
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.xml)|";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtCFDI.Text = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            limpiarValores();
            btnComplemento.Enabled = false;
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private void btnComplemento_Click(object sender, EventArgs e)
        {
          
            FacturaComplementoPago F = new FacturaComplementoPago(oComprobante);
                F.nuevoPago(DateTime.Now.ToString("yyyy-MM-dd"),Convert.ToDecimal( lblSaldo.Text), ctaEmisor.Text,ctaBeneficiario.Text,  txtFolio.Text,  
                    txtSerie.Text,  txtUUID.Text,  0, Convert.ToDouble(lblSaldo.Text),  0,  "moneda",  cmbMetodopago.Text.Substring(0,2),txtReceptor.Text.Trim(), dgDetPagos.RowCount+1, Convert.ToDecimal(lblSaldo.Text  ), dgFacturas);

                List <ComplementoPago> cp = F.getPagos(txtUUID.Text.Trim());
                llenardetallePagos(cp);
            Int32 folioUltimo = metodos_VENTA.ultimoIdVenta();
            frmMail mail = new frmMail(ConfigurationManager.AppSettings["rutaXML"] + txtReceptor.Text.Trim() + "\\" + txtSerie.Text + txtFolio.Text + ".xml", txtReceptor.Text.Trim() + "_" + txtSerie.Text + "_" + folioUltimo, txtEmail.Text,true);
            mail.ShowDialog();
            System.Diagnostics.Process.Start("http://eostechnology.com.mx/EOSFACT/pagofiscal.php?idventa=" + folioUltimo);

        }

        private Boolean ismail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void cmdBuscarFactura_Click(object sender, EventArgs e)
        {
            if (IsNumeric(txtCFDI.Text))
            {
                FacturaComplementoPago f = new FacturaComplementoPago();
                ComplementoPago cp = f.getFactura(Convert.ToDouble(txtCFDI.Text));
                cargarValores(cp);
            }
            else
            {
                MessageBox.Show("Por favor indique un folio de factura válido");
                txtCFDI.Text = string.Empty;
            }
        }


        private bool existeFactura(string folio)
        {
            bool existe = false;
            foreach(DataGridViewRow r in dgFacturas.Rows)
            {
                if(r.Cells[0].Value.ToString()==folio)
                {
                    existe = true;
                }
            }

            return existe;
        }

        private void limpiarCampos()
        {
            txtCFDI.Text = string.Empty;
            txtFecha.Text = string.Empty;
            txtUUID.Text =  string.Empty;
            txtSerie.Text = string.Empty;
            txtFolio.Text = string.Empty;
            txtReceptor.Text = string.Empty;
            dgFacturas.Rows.Clear();
            lblMetodoPago.Text = string.Empty;
            lblSaldo.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(txtCFDI.Text.Trim()==string.Empty)
            {
                MessageBox.Show("Por favor indique un número de folio");
                return;
            }
            FacturaComplementoPago f = new FacturaComplementoPago();
            ComplementoPago cp = f.getFactura(Convert.ToDouble(txtCFDI.Text));
            if(cp==null)
            {
                MessageBox.Show("El folio " + txtCFDI.Text + " no existe o ya fue cobrada anteriormente");
                return;
            }
            if(txtReceptor.Text==string.Empty || txtReceptor.Text==cp.Receptor)
            {
                cargarValores(cp);
                if(existeFactura(cp.UUIDRELACIONADO))
                {
                    MessageBox.Show("La factura ya fue agregada a este pago");
                    return;
                }
                dgFacturas.Rows.Add(cp.UUIDRELACIONADO, cp.FECHA, cp.MONTO, cp.Serie, cp.Folio);
                decimal saldo = 0;
                foreach (DataGridViewRow r in dgFacturas.Rows)
                {
                    saldo += Convert.ToDecimal(r.Cells[2].Value.ToString());
                }
                lblSaldo.Text = saldo.ToString();
                txtCFDI.Text = "";
            }
            else
            {
                MessageBox.Show("El RFC receptor no correponde con los proporcionados anteriormente");
            }

        
        }
    }
}