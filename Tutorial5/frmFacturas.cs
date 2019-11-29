using Microsoft.VisualBasic.FileIO;
using Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vendo
{
    public partial class frmFacturas : Form
    {
        VENTA venta;

        public frmFacturas()
        {
            InitializeComponent();
        }

        private void frmFacturas_Load(object sender, EventArgs e)
        {
            getFacturas(DateTime.Now, DateTime.Now.AddDays(1),"");
        }

        private void getFacturas(DateTime inicio, DateTime fin, string rfc)
        {
            bool facturado = true;
            String boton="";
            string btnEnviar = "";
            Color c=Color.Yellow;
            List<VENTA> listaFacturas = metodos_VENTA.seleccionarVENTA(inicio, fin, rfc);
            dgvFactura.Rows.Clear();
            foreach (VENTA v in listaFacturas)
            {
                if (v.uuid == string.Empty)
                {
                    facturado = false;
                    boton = "Facturar";
                    btnEnviar = "";
                    c = Color.Green;
                }
                if (v.uuid != string.Empty)
                {
                    facturado = true;
                    boton = "Cancelar";
                    btnEnviar = "Enviar";
                    c = Color.Yellow;
                }
                if (v.cancelacion != string.Empty)
                {
                    boton = "";
                    c = Color.Red;
                }
                dgvFactura.Rows.Add(v.id_venta,v.serie,v.folio, v.receptor, v.fecha, v.subtotal, v.iva, v.total, v.tipo, facturado, boton,v.uuid,"PDF", btnEnviar);
                dgvFactura.Rows[dgvFactura.Rows.Count-1].DefaultCellStyle.BackColor = c;
            }
        }

        private void dgvFactura_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            getFacturas(dateDesde.Value, dateHasta.Value, txtRFC.Text.Trim());
        }

        private void dgvFactura_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.dgvFactura.Rows[e.RowIndex].Selected = true;
                this.dgvFactura.CurrentCell = this.dgvFactura.Rows[e.RowIndex].Cells[1];

            }
        }

        private void dgvFactura_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                if(dgvFactura[e.ColumnIndex,e.RowIndex].Value=="PDF")
                {
                    System.Diagnostics.Process.Start(ConfigurationManager.AppSettings.Get("rutafactura") + dgvFactura[0, e.RowIndex].Value.ToString());
                }
                if(dgvFactura[e.ColumnIndex,e.RowIndex].Value=="Facturar")
                {
                    if (MessageBox.Show("Facturar " + dgvFactura[0, e.RowIndex].Value, "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        factura factura = new factura();
                        string msg = factura.Nueva(dgvFactura[0, e.RowIndex].Value.ToString());
                        MessageBox.Show(msg, "Generar Factura", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if(!msg.Contains("Error"))
                            System.Diagnostics.Process.Start(ConfigurationManager.AppSettings.Get("rutafactura") + dgvFactura[0, e.RowIndex].Value.ToString());
                        getFacturas(dateDesde.Value, dateHasta.Value, txtRFC.Text.Trim());
                    }
                }
                else if (dgvFactura[e.ColumnIndex, e.RowIndex].Value == "Cancelar")
                {
                    if (MessageBox.Show("¿Esta seguro de cancelar La factura  " + dgvFactura[0, e.RowIndex].Value + " con folio fiscal " + dgvFactura[9, e.RowIndex].Value + "?","Cancelar Factura",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cancelar(dgvFactura[11, e.RowIndex].Value.ToString());
                        MessageBox.Show("La factura  " + dgvFactura[0, e.RowIndex].Value + " con folio fiscal " + dgvFactura[11, e.RowIndex].Value + " fue cancelada con éxito", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        getFacturas(dateDesde.Value, dateHasta.Value, txtRFC.Text.Trim());
                    }
                }
                else if(dgvFactura[e.ColumnIndex, e.RowIndex].Value == "Enviar")
                {

                    string folioConsultar = dgvFactura[0, e.RowIndex].Value.ToString();
                    VENTA v = metodos_VENTA.seleccionarVENTA(folioConsultar);

                    List<CLIENTES>  clientes = metodos_CLIENTES.seleccionarCLIENTES(dgvFactura[3, e.RowIndex].Value.ToString());
                    if(!Directory.Exists(ConfigurationManager.AppSettings["rutaXML"] + "\\temporal\\"))
                    {
                        Directory.CreateDirectory(ConfigurationManager.AppSettings["rutaXML"] + "\\temporal\\");
                    }
                    string ruta_xml =ConfigurationManager.AppSettings["rutaXML"] +"temporal\\" + clientes[0].rfc + "_" + v.uuid+".xml";
                    File.WriteAllText(ruta_xml, v.xml);
                    frmMail mail = new frmMail(ruta_xml,dgvFactura[3, e.RowIndex].Value.ToString() + "_" + dgvFactura[1, e.RowIndex].Value.ToString() + "_" + dgvFactura[0, e.RowIndex].Value.ToString(), clientes[0].email);
                    mail.ShowDialog();
                }
            }
        }

        private void cancelar(string uuid)
        {

            String DireccionCer = ConfigurationManager.AppSettings["rutaCer"];
            String DireccionKey = ConfigurationManager.AppSettings["rutaKey"];
            String PasswordFinkok = ConfigurationManager.AppSettings["pswFinkok"];
            String PasswordCer = ConfigurationManager.AppSettings["keypass"];
            String username = ConfigurationManager.AppSettings["userFinkok"];

            try
            {
                String url = ConfigurationManager.AppSettings["rutaXML"];
                String ruta;
                ruta = url + "CERyKEY.bat";
                if (!File.Exists(ruta))
                {
                    FabricaPEM(DireccionCer, DireccionKey, PasswordFinkok, PasswordCer);
                }
                String cer;
                String key;

                //Para importar clase TextFieldParser, ingresas al menú Proyecto --> Agregar Referencia --> Ensamblados --> Seleccionar Microsotf.VisualBasic --> Aceptar
                using (TextFieldParser fileReader = new TextFieldParser(DireccionCer + ".pem"))
                    cer = fileReader.ReadToEnd();


                using (TextFieldParser fileReader = new TextFieldParser(DireccionKey+".enc"))
                    key = fileReader.ReadToEnd();

                Cancelacion.CancelSOAP cancela = new Cancelacion.CancelSOAP();
                Cancelacion.cancel can = new Cancelacion.cancel();
                Cancelacion.UUIDS nim = new Cancelacion.UUIDS();
                string[] struuid = new string[] { uuid };
                String rfc;
                String status;
                String acuse;
                String RFCemisor;
                String fecha;
                rfc = ConfigurationManager.AppSettings["rfcEmisor"];
                nim.uuids = struuid.ToArray();
                can.UUIDS = nim;
                can.username = username;
                can.password = PasswordFinkok;
                can.taxpayer_id = rfc;
                can.cer = stringToBase64ByteArray(cer);
                can.key = stringToBase64ByteArray(key);
                Cancelacion.cancelResponse cancelresponse = new Cancelacion.cancelResponse();
                cancelresponse = cancela.cancel(can);
                status = cancelresponse.cancelResult.CodEstatus;
                fecha = cancelresponse.cancelResult.Fecha;
                acuse = cancelresponse.cancelResult.Acuse;
                RFCemisor = cancelresponse.cancelResult.RfcEmisor;
                acuse = cancelresponse.cancelResult.Acuse;
                System.IO.File.WriteAllText(ConfigurationManager.AppSettings["rutaXML"] + "\\acuse.xml", acuse);
                metodos_VENTA.cancelarFacturaVENTA(uuid, acuse);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se cancelo la factura " + ex.Message);
            }
        }


        void FabricaPEM(String cer, String key, String pass, String passCSDoFIEL)
        {
            Dictionary<String, String> DicArchivos = new Dictionary<String, String>();
            String ConvierteCerAPem;
            String ConvierteKeyAPem;
            String EncriptaKey;
            String ArchivoCer = cer;
            String ArchivoKey = key;
            String NombreArchivoCertificado = Path.GetFileName(ArchivoCer);
            String NombreArchivoLlave = Path.GetFileName(ArchivoKey);
            //String usuario;
            //usuario = Environment.UserName;
            //MessageBox.Show(usuario);
            String url;
            url = ConfigurationManager.AppSettings["rutaXML"];
            String ruta;
            ruta = url + "\\OpenSSL\\bin\\";//;//Esta ruta es donde tiene ubicado el .exe del OpenSSL
            ConvierteCerAPem = ruta + "openssl.exe x509 -inform DER -outform PEM -in " + ArchivoCer + " -pubkey -out " + url + NombreArchivoCertificado + ".pem";
            ConvierteKeyAPem = ruta + "openssl.exe pkcs8 -inform DER -in " + ArchivoKey + " -passin pass:" + passCSDoFIEL + " -out " + url + NombreArchivoLlave + ".pem";
            EncriptaKey = ruta + "openssl.exe rsa -in " + url + NombreArchivoLlave + ".pem" + " -des3 -out " + url + NombreArchivoLlave + ".enc -passout pass:" + pass;

            //Crea el archivo Certificado.BAT
            System.IO.StreamWriter oSW = new System.IO.StreamWriter(url + "CERyKEY.bat");
            oSW.WriteLine(ConvierteCerAPem);
            oSW.WriteLine(ConvierteKeyAPem);
            oSW.WriteLine(EncriptaKey);
            oSW.Flush();
            oSW.Close();
            Process.Start(url + "CERyKEY.bat").WaitForExit();
        }

        public byte[] stringToBase64ByteArray(String input)
        {
            Byte[] ret = Encoding.UTF8.GetBytes(input);
            String s = Convert.ToBase64String(ret);
            ret = Convert.FromBase64String(s);
            return ret;
        }
    }
}