using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Modelo;
using System.Configuration;
using System.IO;
using System.Xml;

using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Security.Cryptography.X509Certificates;
using Gma.QrCodeNet.Encoding;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Net;
using Modelo.StampSOAP;

namespace Modelo
{
    public class factura
    {
        internal static byte[] ReadFile(string strArchivo)
        {
            FileStream f = new FileStream(strArchivo, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();
            return data;
        }

        private string generarCadenaOriginal(string myxml)
        {
            string rutaXSLT = @"http://eostechnology.com.mx/XSLT/cadenaoriginal_3_3.xslt";
            string rutaXML = myxml;

            // Cargar XML
            System.Xml.XPath.XPathDocument xml = new XPathDocument(rutaXML);
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
            FileStream fs = new FileStream(@"C:\CFDI\SinTimbrar.xml", FileMode.Open);
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
            XmlTextWriter xmlTextWriter = new XmlTextWriter(@"C:\CFDI\SinTimbrar.xml", Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlArchivo.Serialize(xmlTextWriter, i, xmlNameSpace);
            xmlTextWriter.Close();
        }


        private string generaSello()
        {
            String pass = ConfigurationManager.AppSettings["keypass"]; //Contraseña de la llave privada
            String llave = ConfigurationManager.AppSettings["rutaKey"]; //Archivo de la llave privada
            byte[] llave2 = File.ReadAllBytes(llave); // Convertimos el archivo anterior a byte
            String CadenaOriginal = generarCadenaOriginal(@"C:\CFDI\SinTimbrar.xml");

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

        public string Nueva(string IDVENTA)
        {
            VENTA venta = metodos_VENTA.seleccionarVENTA(IDVENTA);
            List<DETALLEVENTA> detalle = metodos_DETALLEVENTA.seleccionarDETALLEVENTA(Convert.ToInt32(IDVENTA));
            Comprobante objetoComprobante = new Comprobante();

            // Nodo comprobante
            objetoComprobante.Version = "3.3";
            objetoComprobante.Serie = venta.serie;
            objetoComprobante.Folio = venta.folio;
            objetoComprobante.Fecha = Convert.ToDateTime(DateTime.Now.ToString("s"));

            objetoComprobante.TipoDeComprobante = c_TipoDeComprobante.I;

            objetoComprobante.MetodoPagoSpecified = true;
            objetoComprobante.MetodoPago = venta.metodo_pago.Split('-')[0];
            objetoComprobante.FormaPagoSpecified = true;
            objetoComprobante.FormaPago = venta.forma_pago.Split('-')[0];

            objetoComprobante.LugarExpedicion = ConfigurationManager.AppSettings["LugarExpedicion"];
            X509Certificate2 certEmisor = new X509Certificate2();
            byte[] byteCertData = ReadFile(ConfigurationManager.AppSettings["rutaCer"]);
            certEmisor.Import(byteCertData);
            //objetoComprobante.Sello = generaSello();
            objetoComprobante.NoCertificado = ConfigurationManager.AppSettings["noCertificado"];
            objetoComprobante.Certificado = Convert.ToBase64String(certEmisor.GetRawCertData());


            objetoComprobante.Emisor = new ComprobanteEmisor();
            objetoComprobante.Emisor.Rfc = ConfigurationManager.AppSettings["rfcEmisor"];
            objetoComprobante.Emisor.Nombre = ConfigurationManager.AppSettings["Empresa"];
            objetoComprobante.Emisor.RegimenFiscal = c_RegimenFiscal.Item612;//  ConfigurationManager.AppSettings["RegimenFiscal"];

            objetoComprobante.Receptor = new ComprobanteReceptor();
            objetoComprobante.Receptor.Rfc = venta.receptor;
            objetoComprobante.Receptor.Nombre = venta.nombreReceptor;
            objetoComprobante.Receptor.UsoCFDI = "G03";//venta.usocfdi.Split('-')[0];


            objetoComprobante.Conceptos = new ComprobanteConcepto[detalle.Count];


            int indice = 0;
            decimal impuesto = 0;
            foreach (DETALLEVENTA v in detalle)
            {
                objetoComprobante.Conceptos[indice] = new ComprobanteConcepto();

                objetoComprobante.Conceptos[indice].Impuestos = new ComprobanteConceptoImpuestos();



                objetoComprobante.Conceptos[indice].Impuestos.Traslados = new ComprobanteConceptoImpuestosTraslado[1];

                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0] = new ComprobanteConceptoImpuestosTraslado();
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Base = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", v.precio*v.cantidad)); //TODO calcular interes + carga - descuento
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Importe = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", v.precio * v.cantidad * Convert.ToDecimal(0.16))); //TODO IVA
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TipoFactor = c_TipoFactor.Tasa;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Impuesto = c_Impuesto.Item002; //IVA
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TasaOCuota = 0.160000m;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].ImporteSpecified = true;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TasaOCuotaSpecified = true;

                objetoComprobante.Conceptos[indice].ClaveProdServ = v.claveservicio;
                objetoComprobante.Conceptos[indice].ClaveUnidad = v.claveunidad;
                objetoComprobante.Conceptos[indice].Cantidad = Convert.ToDecimal(v.cantidad);
                objetoComprobante.Conceptos[indice].Unidad = metodos_C_CVEUNIDAD.seleccionarC_CVEUNIDAD(v.claveunidad).nombre;
                objetoComprobante.Conceptos[indice].Descripcion = v.descripcion;
                objetoComprobante.Conceptos[indice].ValorUnitario = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", v.precio));
                objetoComprobante.Conceptos[indice].Importe = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", v.precio * v.cantidad));
                impuesto += Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", objetoComprobante.Conceptos[indice].Importe));
                indice++;
            }


            //Nodo Impuestos
            objetoComprobante.Impuestos = new ComprobanteImpuestos();
            objetoComprobante.Impuestos.TotalImpuestosTrasladadosSpecified = true;
            //
            // Nodo Impuestos Trasladados
            objetoComprobante.Impuestos.Traslados = new ComprobanteImpuestosTraslado[1];


            objetoComprobante.Impuestos.Traslados[0] = new ComprobanteImpuestosTraslado();
            objetoComprobante.Impuestos.Traslados[0].Importe = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}",venta.iva));
            objetoComprobante.Impuestos.Traslados[0].TasaOCuota = 0.160000m;
            objetoComprobante.Impuestos.Traslados[0].Impuesto = c_Impuesto.Item002;

            objetoComprobante.Impuestos.TotalImpuestosTrasladados = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", venta.iva)); ;
            objetoComprobante.SubTotal = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", venta.subtotal));
            objetoComprobante.Moneda = c_Moneda.MXN;
            objetoComprobante.Total = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", venta.total));

            // Creas los namespaces requeridos
            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            //xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            //xmlNameSpace.Add("xsi:schemaLocation", "http://www.sat.gob.mx/cfd/3http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/TimbreFiscalDigitalhttp://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd");            
            xmlNameSpace.Add("schemaLocation", "http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd");

            // Creas una instancia de XMLSerializer con el tipo de dato Comprobante
            XmlSerializer xmlArchivo = new XmlSerializer(typeof(Comprobante));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(@"C:\CFDI\SinTimbrar.xml", Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlArchivo.Serialize(xmlTextWriter, objetoComprobante, xmlNameSpace);
            xmlTextWriter.Close();

            XmlDocument documentoXml = new XmlDocument();
            documentoXml.Load(@"C:\CFDI\SinTimbrar.xml");
            desSerializarXML();
            return  Timbrar(IDVENTA,venta.receptor,venta.serie,venta.folio,venta.total);
        }

        private string  Timbrar(string IDVENTA, string rfcReceptor,string serie, string folio,double total)
        {
            StampSOAP.StampSOAP selloSOAP = new StampSOAP.StampSOAP();
            stamp oStamp = new stamp();
            stampResponse selloResponse = new stampResponse();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"C:\CFDI\SinTimbrar.xml");//ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim() + "\\" + txtSerie.Text + txtFolio.Text + ".xml");

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
                return "La factura no pudo timbrarse. \n\nCodigo de Error: " + selloResponse.stampResult.Incidencias[0].CodigoError + "\nDescripción: " + selloResponse.stampResult.Incidencias[0].MensajeIncidencia;
            }
            else
            {
                doc.LoadXml(xml);
                if (!Directory.Exists(ConfigurationManager.AppSettings["rutaXML"] + rfcReceptor))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["rutaXML"] + rfcReceptor);
                doc.Save(ConfigurationManager.AppSettings["rutaXML"] + rfcReceptor + "\\" + serie + folio + ".xml");
                metodos_VENTA.guardarTimbre(ConfigurationManager.AppSettings["noCertificado"], Convert.ToInt64(IDVENTA), selloResponse.stampResult.Fecha, selloResponse.stampResult.NoCertificadoSAT, selloResponse.stampResult.SatSeal, selloResponse.stampResult.UUID, selloResponse.stampResult.xml, ConfigurationManager.AppSettings["noCertificado"], generaSello(), "");

                string cadenaORiginalSAT = "?re=" + ConfigurationManager.AppSettings["rfcEmisor"] + "&rr=" + ConfigurationManager.AppSettings["rfcEmisor"] + "&tt=" + total + "&id=" + selloResponse.stampResult.UUID;
                gen_qr_file(ConfigurationManager.AppSettings["rutaXML"] + selloResponse.stampResult.UUID + ".png", cadenaORiginalSAT, selloResponse.stampResult.UUID + ".png");
                return "La factura se timbró correctamente y generó el folio fiscal " + selloResponse.stampResult.UUID;
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
            //Upload("ftp://drimexico.com.mx", "pventa@drimexico.com.mx", "dri15032018", new_file_name, "public_html/cbb/", uuid);
            Upload("ftp://eostechnology.com.mx", "pventa@eostechnology.com.mx", "dri15032018", new_file_name, "public_html/cbb/", uuid);
        }

        private int Upload(string server, string user, string pass, string origen, string rutadestino, string nombredestino)

        {
            try
            {
                string ruta = server + "/" + rutadestino + "/" + nombredestino;
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(server + "/" + nombredestino);
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
    }
}
