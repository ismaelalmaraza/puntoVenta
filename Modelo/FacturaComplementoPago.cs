using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

using AccesoDatos;
using System.Windows.Forms;
using Modelo;
using Modelo.StampSOAP;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace Logica
{
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(Encoding encoding)
            : base()
        {
            this.m_Encoding = encoding;
        }
        private readonly Encoding m_Encoding;
        public override Encoding Encoding
        {
            get
            {
                return this.m_Encoding;
            }
        }
    }

    public class ComplementoPago
    {
        string uuid;
        string monto;
        string fecha;
        string folio;
        string serie;
        string emisor;
        string receptor;
        string mPago;

        public string metodoPago
        {
            get { return this.mPago; }
            set { this.mPago = value; }
        }
        public string Emisor
        {
            get { return this.emisor; }
            set { this.emisor = value; }
        }

        public string Receptor
        {
            get { return this.receptor; }
            set { this.receptor = value; }
        }

        public string UUIDRELACIONADO
        {
            get { return this.uuid; }
            set { this.uuid = value; }
        }
        public string MONTO
        {
            get { return this.monto; }
            set { this.monto = value; }
        }

        public string FECHA
        {
            get { return this.fecha; }
            set { this.fecha = value; }
        }

        public string Serie
        {
            get { return this.folio; }
            set { this.folio = value; }
        }

        public string Folio
        {
            get { return this.serie; }
            set { this.serie = value; }
        }
    }


    public class FacturaComplementoPago
    {
        ComplementoPago comprobanteP;
        Comprobante comprobante;
        public FacturaComplementoPago()
        {
        }
        /// <summary>
        /// Constructor de la clase Factura
        /// </summary>
        /// <param name="comprobante">Factura emitida con el tipo de pago POR DEFINIR</param>
        public FacturaComplementoPago(ComplementoPago comprobante)
        {
            this.comprobanteP = comprobante;

            //if (!verificaFacturaExiste(comprobante.TimbreFiscalDigital.UUID))
            //{
            insertaFctura(this.comprobanteP);
            //}

        }

        public ComplementoPago getFactura(double id)
        {
            AccesoDatos.BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(@"SELECT receptor, UUID,total,fecha, serie.serie, folio,metodo_pago from venta, serie WHERE id_venta = @id and venta.serie = serie.idserie and (venta.cobrada is null or venta.cobrada=0)");
            d.AsignarParametroDouble("@id", id);
            ComplementoPago cp = null;
            DbDataReader dr = d.EjecutarConsulta();
            while (dr.Read())
            {
                cp = new ComplementoPago();
                cp.Receptor = dr[0].ToString();
                cp.UUIDRELACIONADO = dr[1].ToString();
                cp.MONTO = dr[2].ToString();

                cp.FECHA = dr[3].ToString();
                cp.Serie = dr[4].ToString();
                cp.Folio = dr[5].ToString();
                cp.metodoPago= dr[6].ToString();
            }
            dr.Close();
            d.Desconectar();
            return cp;

        }

        public List<ComplementoPago> getPagos(string uuid)
        {
            ComplementoPago cp = null;
            List<ComplementoPago> cps = new List<ComplementoPago>();
            AccesoDatos.BaseDatos d = new AccesoDatos.BaseDatos();
            d.Conectar();
            d.CrearComando("SELECT FECHA,UUIDRELACIONADO,MONTO FROM COMPLEMENTODETALLE  WHERE UUIDRELACIONADO = @UUID");
            d.AsignarParametroCadena("@UUID", uuid);

            DbDataReader dr = d.EjecutarConsulta();
            while (dr.Read())
            {
                cp = new ComplementoPago();
                cp.FECHA = dr[0].ToString();
                cp.UUIDRELACIONADO = dr[1].ToString();
                cp.MONTO = dr[2].ToString();
                cps.Add(cp);
            }
            dr.Close();
            d.Desconectar();
            return cps;
        }

        private bool Timbrar(Comprobante oComprobante, string mPago, string serie, string folio)
        {
            StampSOAP selloSOAP = new StampSOAP();
            stamp oStamp = new stamp();
            stampResponse selloResponse = new stampResponse();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"C:\CFDI\pago.xml");//ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim() + "\\" + txtSerie.Text + txtFolio.Text + ".xml");

            byte[] byteXmlDocument = Encoding.UTF8.GetBytes(xmlDocument.OuterXml);
            string stringByteXmlDocument = Convert.ToBase64String(byteXmlDocument);
            byteXmlDocument = Convert.FromBase64String(stringByteXmlDocument);

            oStamp.xml = byteXmlDocument;
            oStamp.username = ConfigurationManager.AppSettings.Get("userFinkok");
            oStamp.password = ConfigurationManager.AppSettings.Get("pswFinkok");

            selloResponse = selloSOAP.stamp(oStamp);
            string xml = selloResponse.stampResult.xml.Replace("\\", "");
            xml = xml.Replace("\n", "");
            XmlDocument doc = new XmlDocument();
            if (xml == "")
            {
                MessageBox.Show("La factura no pudo timbrarse. \n\nCodigo de Error: " + selloResponse.stampResult.Incidencias[0].CodigoError + "\nDescripción: " + selloResponse.stampResult.Incidencias[0].MensajeIncidencia, "Error al timbrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                doc.LoadXml(xml);
                if (!Directory.Exists(ConfigurationManager.AppSettings["rutaXML"] + oComprobante.Receptor.Rfc))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["rutaXML"] + oComprobante.Receptor.Rfc);
                doc.Save(ConfigurationManager.AppSettings["rutaXML"] + oComprobante.Receptor.Rfc + "\\" + serie + folio + ".xml");
                oComprobante.FormaPago = "PUE";
                oComprobante.MetodoPago = mPago;

                metodos_VENTA.guardarTimbre(oComprobante, ConfigurationManager.AppSettings["noCertificado"], 0, selloResponse.stampResult.Fecha, selloResponse.stampResult.NoCertificadoSAT, selloResponse.stampResult.SatSeal, selloResponse.stampResult.UUID, selloResponse.stampResult.xml, ConfigurationManager.AppSettings["noCertificado"], generaSello(),"");
                Int32 id = metodos_VENTA.ultimoIdVenta();
                metodos_DETALLEVENTA.insertarDETALLEVENTA(0, id, "16477",0,1, 0,0, "CTA", "84111506","0","PAGO", 0);

                string cadenaORiginalSAT = "?re=" + ConfigurationManager.AppSettings["rfcEmisor"] + "&rr=" + oComprobante.Emisor.Rfc + "&tt=" + oComprobante.Total + "&id=" + selloResponse.stampResult.UUID;
                gen_qr_file(ConfigurationManager.AppSettings["rutaXML"] + selloResponse.stampResult.UUID + ".png", cadenaORiginalSAT, selloResponse.stampResult.UUID + ".png");

                MessageBox.Show(selloResponse.stampResult.CodEstatus, "Timbrado exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
        }

        private void gen_qr_file(string file_name, string content, string uuid)
        {

            string new_file_name = file_name;
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(content, out qrCode);

            GraphicsRenderer renderer = new GraphicsRenderer(
                new FixedCodeSize(400, QuietZoneModules.Zero),
                Brushes.Black,
                Brushes.White);
            MemoryStream ms = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            var imageTemp = new Bitmap(ms);
            var image = new Bitmap(imageTemp, new Size(new Point(200, 200)));
            image.Save(new_file_name, ImageFormat.Png);
            Upload("ftp.eostechnology.com.mx", "pventa@eostechnology.com.mx", "dri15032018", new_file_name, "public_html/cbb/", uuid);
        }

        private int Upload(string server, string user, string pass, string origen, string rutadestino, string nombredestino)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(server + "/" + rutadestino + "/" + nombredestino);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, pass);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = true;
                FileStream stream = File.OpenRead(origen);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Flush();
                reqStream.Close();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Genera un nuevo complemento de pago de una factura existente
        /// </summary>
        /// <returns>Si todo ourrio bien devuelve TRUE, de contrario devuelve FALSE</returns>
        public void nuevoPago(string fecha, decimal monto, string ctaEmisor, string ctaReceptor, string folio, string serie, string uuid, double saldoanterior, double pago, double saldoInsoluto, string moneda, string mPago, string rfcReceptor, int numPago, decimal saldoAnterior, DataGridView dg)
        {
            try
            {
                double uf = generarComplementoPago(monto, ctaEmisor, ctaReceptor, folio, serie, uuid, moneda, mPago, rfcReceptor, numPago, saldoAnterior, dg);
                AccesoDatos.BaseDatos d = new AccesoDatos.BaseDatos();
                d.Conectar();

                foreach (DataGridViewRow r in dg.Rows)
                {
                    String Qry = @"INSERT INTO COMPLEMENTODETALLE(FECHA, UUID, UUIDRELACIONADO, MONTO,metodoPago)values(@FECHA, @UUID, @UUIDRELACIONADO, @MONTO,@metodoPago)";
                    
                    d.CrearComando(Qry);
                    d.AsignarParametroCadena("@FECHA", r.Cells[1].Value.ToString());
                    d.AsignarParametroDouble("@UUID", uf);
                    d.AsignarParametroCadena("@UUIDRELACIONADO", r.Cells[0].Value.ToString());
                    d.AsignarParametroCadena("@MONTO", r.Cells[2].Value.ToString());
                    d.AsignarParametroCadena("@metodoPago", mPago);

                    d.EjecutarComando();

                    String Qry2 = @"update venta set cobrada=true where UUID = @UUID";

                    d.CrearComando(Qry2);
                    d.AsignarParametroCadena("@UUID", r.Cells[0].Value.ToString());
                    d.EjecutarComando();

                }
                d.Desconectar();
            }
            catch (Exception err)
            {

            }
        }

        private bool CreateXML(Comprobante oComprobante, string mPago, string serie, string folio)
        {
            //SERIALIZAMOS.-------------------------------------------------

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(Comprobante));

            string sXml = "";

            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {

                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }

            }

            //guardamos el string en un archivo
            System.IO.File.WriteAllText(@"C:\CFDI\pago.xml", sXml);
            desSerializarXML();
            return Timbrar(oComprobante, mPago,  serie,  folio);
        }

        private string generarCadenaOriginal(string myxml)
        {
            string rutaXSLT = @"http://eostechnology.com.mx/XSLT/cadenaoriginal_3_3.xslt";
            string rutaXML = myxml;

            // Cargar XML
            XPathDocument xml = new XPathDocument(rutaXML);
            // Cargar XSLT
            XslCompiledTransform transformador = new XslCompiledTransform();
            transformador.Load(rutaXSLT);
            // Procesamiento
            StringWriter str = new StringWriter();
            XmlTextWriter cad = new XmlTextWriter(str);
            transformador.Transform(rutaXML, cad);
            //Resultado
            string result = str.ToString();

            return result;
        }

        private void desSerializarXML()
        {

            Console.WriteLine("Reading with XmlReader");

            // Create an instance of the XmlSerializer specifying type and namespace.
            XmlSerializer serializer = new
            XmlSerializer(typeof(Comprobante));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(@"C:\CFDI\pago.xml", FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            Comprobante i;

            // Use the Deserialize method to restore the object's state.
            i = (Comprobante)serializer.Deserialize(reader);
            fs.Close();
            i.Sello = generaSello();

            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            //xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            //xmlNameSpace.Add("xsi:schemaLocation", "http://www.sat.gob.mx/cfd/3http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/TimbreFiscalDigitalhttp://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd");            
            xmlNameSpace.Add("schemaLocation", "http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd");

            XmlSerializer xmlArchivo = new XmlSerializer(typeof(Comprobante));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(@"C:\CFDI\pago.xml", Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlArchivo.Serialize(xmlTextWriter, i, xmlNameSpace);
            xmlTextWriter.Close();
        }


        private string generaSello()
        {
            String pass = ConfigurationManager.AppSettings["keypass"]; //Contraseña de la llave privada
            String llave = ConfigurationManager.AppSettings["rutaKey"]; //Archivo de la llave privada
            byte[] llave2 = File.ReadAllBytes(llave); // Convertimos el archivo anterior a byte
            String CadenaOriginal = generarCadenaOriginal(@"C:\CFDI\pago.xml");

            //1) Desencriptar la llave privada, el primer parámetro es la contraseña de llave privada y el segundo es la llave privada en formato binario.
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter asp = Org.BouncyCastle.Security.PrivateKeyFactory.DecryptKey(pass.ToCharArray(), llave2);

            //2) Convertir a parámetros de RSA
            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters key = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)asp;

            //3) Crear el firmador con SHA1
            //Org.BouncyCastle.Crypto.ISigner sig = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA1withRSA");
            //La siguiente linea es para generar el sello para la nueva versión de CFDI 3.3
            Org.BouncyCastle.Crypto.ISigner sig = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA-256withRSA");

            //4) Inicializar el firmador con la llave privada
            sig.Init(true, key);

            // 5) Pasar la cadena original a formato binario
            byte[] bytes = Encoding.UTF8.GetBytes(CadenaOriginal);

            // 6) Encriptar
            sig.BlockUpdate(bytes, 0, bytes.Length);
            byte[] bytesFirmados = sig.GenerateSignature();

            // 7) Finalmente obtenemos el sello
            String sello = Convert.ToBase64String(bytesFirmados);

            return sello;

        }

        public double generarComplementoPago(decimal monto, string ctaEmisor, string ctaReceptor, string folio, string serie, string uuid, string moneda, string mPago, string rfcReceptor, int numPago, decimal saldoAnt, DataGridView dg)
        {
            monto = 0;
            Pagos oPagos = new Pagos();
            List<PagosPago> lstPagos = new List<PagosPago>();
            PagosPago oPago = new PagosPago();
            oPago.MonedaP = c_Moneda.MXN;
            oPago.FormaDePagoP = mPago;

            oPago.FechaPago = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            int i = 0;
            PagosPagoDoctoRelacionado[] relacionado = new PagosPagoDoctoRelacionado[dg.Rows.Count];
            foreach (DataGridViewRow r in dg.Rows)
            {
                
                relacionado[i] = new PagosPagoDoctoRelacionado();

                relacionado[i].IdDocumento = r.Cells[0].Value.ToString();
                relacionado[i].Serie = r.Cells[3].Value.ToString();
                relacionado[i].Folio = r.Cells[4].Value.ToString();
                relacionado[i].MonedaDR = c_Moneda.MXN;
                relacionado[i].MetodoDePagoDR = c_MetodoPago.PPD;
                relacionado[i].NumParcialidad = "1";
                monto += Convert.ToDecimal(String.Format("{0:#.00}", Convert.ToDouble(r.Cells[2].Value)));
                relacionado[i].ImpSaldoAnt = Convert.ToDecimal(String.Format("{0:#.00}", Convert.ToDouble( r.Cells[2].Value)));   //Monto de Factura
                relacionado[i].ImpSaldoAntSpecified = true;
                relacionado[i].ImpPagado = Convert.ToDecimal(String.Format("{0:#.00}", Convert.ToDouble(r.Cells[2].Value)));   //Monto de factura
                relacionado[i].ImpPagadoSpecified = true;
                relacionado[i].ImpSaldoInsoluto = Convert.ToDecimal(String.Format("{0:#.00}", Convert.ToDouble(0)));
                relacionado[i].ImpSaldoInsolutoSpecified = true;
                i++;
                oPago.DoctoRelacionado = relacionado;

            }
            oPago.Monto = Convert.ToDecimal(String.Format("{0:#.00}", monto));
            lstPagos.Add(oPago);
            oPagos.Pago = lstPagos.ToArray();
            Comprobante comprobante = new Comprobante();
            comprobante.Complemento = new ComprobanteComplemento[1];
            comprobante.Complemento[0] = new ComprobanteComplemento();

            XmlDocument docPago = new XmlDocument();
            XmlSerializerNamespaces xmlNameSpacePago = new XmlSerializerNamespaces();
            xmlNameSpacePago.Add("pago10", "http://www.sat.gob.mx/Pagos");
            using (XmlWriter writer = docPago.CreateNavigator().AppendChild())
            {
                new XmlSerializer(oPagos.GetType()).Serialize(writer, oPagos, xmlNameSpacePago);
            }

            comprobante.Receptor = new ComprobanteReceptor();
            comprobante.Receptor.UsoCFDI = "P01";
            comprobante.Receptor.Rfc = rfcReceptor;
            comprobante.Conceptos = new ComprobanteConcepto[1];
            comprobante.Conceptos[0] = new ComprobanteConcepto();
            comprobante.Conceptos[0].Cantidad = 1;
            comprobante.Conceptos[0].ClaveProdServ = "84111506";
            comprobante.Conceptos[0].ClaveUnidad = "ACT";
            comprobante.Conceptos[0].Descripcion = "Pago";
            comprobante.Conceptos[0].ValorUnitario = 0;
            comprobante.Conceptos[0].Importe = 0;
            comprobante.Sello = "";
            comprobante.Fecha = Convert.ToDateTime(DateTime.Now.ToString("s"));
            comprobante.NoCertificado = ConfigurationManager.AppSettings["noCertificado"];
            X509Certificate2 certEmisor = new X509Certificate2();
            byte[] byteCertData = ReadFile(ConfigurationManager.AppSettings["rutaCer"]);
            certEmisor.Import(byteCertData);
            comprobante.Certificado = Convert.ToBase64String(certEmisor.GetRawCertData());
            //comprobante.TimbreFiscalDigital = null;

            comprobante.Complemento = new ComprobanteComplemento[1];
            comprobante.Complemento[0] = new ComprobanteComplemento();
            comprobante.Complemento[0].Any = new XmlElement[2];
            comprobante.Complemento[0].Any[0] = docPago.DocumentElement;


            //var dom = new XmlDocument();
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(dom.NameTable);
            //nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            //var contact = dom.CreateElement("tfd:TimbreFiscalDigital", "http://www.sat.gob.mx/TimbreFiscalDigital");
            //dom.AppendChild(contact);

            //comprobante.Complemento[0].Any[1] = dom.DocumentElement;

            comprobante.Impuestos = null;
            comprobante.Total = 0;
            comprobante.SubTotal = 0;
            comprobante.Moneda = c_Moneda.XXX;
            comprobante.FormaPagoSpecified = false;
            comprobante.TipoDeComprobante = c_TipoDeComprobante.P;
            comprobante.LugarExpedicion = ConfigurationManager.AppSettings["LugarExpedicion"];
            comprobante.Emisor = new ComprobanteEmisor();
            comprobante.Emisor.Rfc = ConfigurationManager.AppSettings["rfcEmisor"];
            comprobante.Emisor.RegimenFiscal = c_RegimenFiscal.Item612;
            double ultimoFolio = 0;
            if(CreateXML(comprobante, mPago,  serie,  folio))
            {
                ultimoFolio = guardarPago(uuid,c_MetodoPago.PPD,0m, Convert.ToDecimal(String.Format("{0:#.00}", saldoAnt)), Convert.ToDecimal(String.Format("{0:#.00}", saldoAnt - monto)), Convert.ToDecimal(String.Format("{0:#.00}", monto)),
                   Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")), Convert.ToDecimal(String.Format("{0:#.00}", monto)),mPago,dg);
            }

            return ultimoFolio; 
        }

        private double guardarPago(string uuid, c_MetodoPago metodo, decimal totalFactura, decimal saldoAnterior, decimal saldoPendiente, decimal montoPagado, DateTime fechaHora, decimal total,string mPago, DataGridView dg)
        {
            try
            {
                Int32 folioUltimo = metodos_VENTA.ultimoIdVenta();
                BaseDatos d = new BaseDatos();
                
                foreach (DataGridViewRow r in dg.Rows)
                {
                    d.Conectar();
                    d.CrearComando(@"INSERT INTO pagos( UUID, metodo, total, sAnterior, sPendiente, mPagado, fechaHora, formaPago, totalPago,id_pago_fk) 
                            VALUES (@UUID, @metodo, @total, @sAnterior, @sPendiente, @mPagado, @fechaHora, @formaPago, @totalPago,@id_pago_fk)");
                    d.AsignarParametroCadena("@UUID", r.Cells[0].Value.ToString());
                    d.AsignarParametroCadena("@metodo", "PPD");
                    d.AsignarParametroDecimal("@total", Convert.ToDecimal(r.Cells[2].Value.ToString()));
                    d.AsignarParametroDecimal("@sAnterior", Convert.ToDecimal(r.Cells[2].Value.ToString()));
                    d.AsignarParametroDecimal("@sPendiente", 0);
                    d.AsignarParametroDecimal("@mPagado", Convert.ToDecimal(r.Cells[2].Value.ToString()));
                    d.AsignarParametroCadena("@fechaHora", fechaHora.ToString("yyyy-MM-dd HH:mm:ss"));
                    d.AsignarParametroCadena("@formaPago", mPago);
                    d.AsignarParametroDecimal("@totalPago", Convert.ToDecimal(r.Cells[2].Value.ToString()));
                    d.AsignarParametroEntero("@id_pago_fk", folioUltimo);
                    d.EjecutarConsulta();
                    d.Desconectar();

                }

                return folioUltimo;
            }
            catch(Exception err)
            {
                return 0;
            }
            
        }

        internal static byte[] ReadFile(string strArchivo)
        {
            FileStream f = new FileStream(strArchivo, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();
            return data;
        }

        private bool generarXML(string UUID)
        {
            bool existe = false;
            AccesoDatos.BaseDatos d = new AccesoDatos.BaseDatos();
            d.Conectar();
            d.CrearComando("SELECT * FROM COMPLEMENTO  WHERE UUID = @UUID");
            d.AsignarParametroCadena("@UUID", UUID);

            DbDataReader dr = d.EjecutarConsulta();
            while (dr.Read())
            {
                existe = true;
            }
            dr.Close();
            d.Desconectar();
            return existe;
        }

        private bool verificaFacturaExiste(string UUID)
        {
            bool existe = false;
            AccesoDatos.BaseDatos d = new AccesoDatos.BaseDatos();
            d.Conectar();
            d.CrearComando("SELECT * FROM COMPLEMENTO  WHERE UUID = @UUID");
            d.AsignarParametroCadena("@UUID", UUID);

            DbDataReader dr = d.EjecutarConsulta();
            while (dr.Read())
            {
                existe = true;
            }
            dr.Close();
            d.Desconectar();
            return existe;
        }

        private bool insertaFctura(ComplementoPago comprobante)
        {
            try
            {
                String Qry = @"INSERT INTO COMPLEMENTO(UUID, EMISION, MONTO, EMISOR, RECEPTOR)values(@UUID, @EMISION, @MONTO, @EMISOR, @RECEPTOR)";
                AccesoDatos.BaseDatos d = new AccesoDatos.BaseDatos();
                d.Conectar();
                d.CrearComando(Qry);
                d.AsignarParametroCadena("@UUID", comprobante.UUIDRELACIONADO);
                d.AsignarParametroCadena("@EMISION", comprobante.FECHA);
                d.AsignarParametroCadena("@MONTO", comprobante.MONTO);
                d.AsignarParametroCadena("@EMISOR", comprobante.Emisor);
                d.AsignarParametroCadena("@RECEPTOR", comprobante.Receptor);

                d.EjecutarComando();
                d.Desconectar();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool consultaaFctura()
        {
            return false;
        }
    }
}
