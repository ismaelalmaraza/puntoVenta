using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modelo;
using System.Configuration;
using System.IO;
using System.Xml;
using Vendo.StampSOAP;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Security.Cryptography.X509Certificates;
using Gma.QrCodeNet.Encoding;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Net;
using System.Diagnostics;

namespace Vendo
{
    public partial class frmVenta : Form
    {
        metodos_SERIE serie;
        List<CLIENTES> clientes;
        List<C_MONEDA> monedas;
        List<C_METODOPAGO> metodosPago;
        List<C_FORMAPAGO> formasPago;
        List<PRODUCTOS> productos;
        string tipoPrecio;
        string tipoVenta;
        Int32 idCliente;
        Int32 idCotizacion=0;

        string e_mail = string.Empty;

        public frmVenta()
        {
            InitializeComponent();
            habilitarTextBox(false);

            dgvClientes.Width=365;
            dgvClientes.Height = 258;
            dgvMoneda.Width=302;
            dgvMoneda.Height = 320;
            dgvMetododPago.Width=330;
            dgvMetododPago.Height=300;

            dgvFormaPago.Width = 330;
            dgvFormaPago.Height = 300;
            serie = new metodos_SERIE();
            txtSerie.DataSource = serie.seleccionarSERIE();
            txtSerie.DisplayMember = "serie";
            txtSerie.ValueMember = "IDSERIE";
            tipoVenta = "NOTA";


            txtMetodoPago.DataSource = metodos_C_METODOPAGO.seleccionarC_METODOPAGO();
            txtMetodoPago.DisplayMember ="descripcion";
            txtMetodoPago.ValueMember ="c_metodopago";

            txtFormaPago.DataSource = metodos_C_FORMAPAGO.seleccionarC_FORMAPAGO();
            txtFormaPago.DisplayMember = "descripcion";
            txtFormaPago.ValueMember = "c_formapago";
        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            clientes = metodos_CLIENTES.seleccionarCLIENTES(txtCliente.Text.Trim());

            if (txtCliente.Text.Trim().Length > 0)
            {
                if(clientes.Count()>0)
                    dgvClientes.Visible = true;
            }
            else
            {
                dgvClientes.Visible = false;

            }
            dgvClientes.Rows.Clear();
            foreach(CLIENTES cliente in  clientes)
            {
                dgvClientes.Rows.Add(cliente.rfc, cliente.nombre);
            }

        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var cte = (from item in clientes
                            where item.rfc == this.dgvClientes.Rows[e.RowIndex].Cells[0].Value.ToString()
                            select item).First();
                lblPrecioCte.Text = cte.precio.Replace("PRECIO ","");
                idCliente = cte.id_cliente;
                lblNombre.Text= cte.nombre;
                lblLimiteCredito.Text = cte.credito.ToString();
                lblRFC.Text = cte.rfc;
                e_mail = cte.email;
                tipoPrecio = cte.precio;
                dgvClientes.Visible = false;
                txtCliente.Text = string.Empty;
                txtProducto.Enabled = true;
                txtCantidad.Enabled = true;
                txtProducto.Focus();
                List<VENTA> cotizaciones = metodos_VENTA.obtenerCotizaciones(idCliente);
                if (cotizaciones.Count > 0)
                    pcbContizaciones.Visible = true;
                else
                    pcbContizaciones.Visible = false;
            }
            catch (Exception err)
            {

            }
        }

        private void lblNombre_Click(object sender, EventArgs e)
        {

        }

        private void frmVenta_Load(object sender, EventArgs e)
        {

        }

        private void txtMoneda_TextChanged(object sender, EventArgs e)
        {

            monedas = metodos_C_MONEDA.seleccionarC_MONEDA(txtMoneda.Text.Trim());

            if (txtMoneda.Text.Trim().Length > 0)
            {
                if (monedas.Count() > 0)
                    dgvMoneda.Visible = true;
            }
            else
            {
                dgvMoneda.Visible = false;

            }
            dgvMoneda.Rows.Clear();
            foreach (C_MONEDA moneda in monedas)
            {
                dgvMoneda.Rows.Add(moneda.c_moneda, moneda.descripcion);
            }
        }

        private void dgvMoneda_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtMoneda.Text=this.dgvMoneda.Rows[e.RowIndex].Cells[0].Value.ToString();
                dgvMoneda.Visible = false;

                if (txtMoneda.Text.Trim() == "MXN")
                {
                    txtTC.Text = "1";
                    txtTC.Enabled = false;
                }else
                {
                    txtTC.Text = string.Empty;
                    txtTC.Enabled = true;
                }
            }
            catch (Exception err)
            {

            }
        }

        private void txtMetodoPago_TextChanged(object sender, EventArgs e)
        {
            //metodosPago = metodos_C_METODOPAGO.seleccionarC_METODOPAGO(txtMetodoPago.Text.Trim());

            //if (txtMetodoPago.Text.Trim().Length > 0)
            //{
            //    if (metodosPago.Count() > 0)
            //        dgvMetododPago.Visible = true;
            //}
            //else
            //{
            //    dgvMetododPago.Visible = false;

            //}
            //dgvMetododPago.Rows.Clear();
            //foreach (C_METODOPAGO metodoPago in metodosPago)
            //{
            //    dgvMetododPago.Rows.Add(metodoPago.c_metodopago, metodoPago.descripcion);
            //}
        }

        private void dgvMetododPago_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtMetodoPago.Text = this.dgvMetododPago.Rows[e.RowIndex].Cells[0].Value.ToString() + "-" +
                    this.dgvMetododPago.Rows[e.RowIndex].Cells[1].Value.ToString();
                dgvMetododPago.Visible = false;

            }
            catch (Exception err)
            {

            }
        }

        private void txtFormaPago_TextChanged(object sender, EventArgs e)
        {
            //formasPago = metodos_C_FORMAPAGO.seleccionarC_FORMAPAGO(txtFormaPago.Text.Trim());

            //if (txtFormaPago.Text.Trim().Length > 0)
            //{
            //    if (formasPago.Count() > 0)
            //        dgvFormaPago.Visible = true;
            //}
            //else
            //{
            //    dgvFormaPago.Visible = false;

            //}
            //dgvFormaPago.Rows.Clear();
            //foreach (C_FORMAPAGO formaPago in formasPago)
            //{
            //    dgvFormaPago.Rows.Add(formaPago.c_formapago, formaPago.descripcion);
            //}
        }

        private void dgvFormaPago_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtFormaPago.Text = this.dgvFormaPago.Rows[e.RowIndex].Cells[0].Value.ToString() + "-" +
                    this.dgvFormaPago.Rows[e.RowIndex].Cells[1].Value.ToString();
                dgvFormaPago.Visible = false;

            }
            catch (Exception err)
            {

            }
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {
            productos = metodos_PRODUCTOS.seleccionarPRODUCTOSLst(txtProducto.Text.Trim());
            if (txtProducto.Text.Trim().Length > 0)
            {
                if (productos.Count() > 0)
                    dgProductosLst.Visible = true;
            }
            else
            {
                dgProductosLst.Visible = false;

            }
            dgProductosLst.Rows.Clear();
            
            foreach(PRODUCTOS p in productos)
            {
                dgProductosLst.Rows.Add(p.clave, p.descripcion);
            }
            //seleccionarCLIENTES

        }

        private void totales()
        {
            try
            {
                if(lblDescuento.Text.Trim()==string.Empty)
                {
                    lblDescuento.Text = "0.00";
                }
                decimal total = 0;
                decimal descuento = 0;
                foreach (DataGridViewRow row in dgvFactura.Rows)
                {
                    string t = row.Cells["Total"].Value.ToString();
                    string d = row.Cells["Descuento"].Value.ToString();
                    total += Convert.ToDecimal(t) ;
                    descuento += Convert.ToDecimal(d);
                }

                lblTotDesc.Text = (Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", descuento.ToString()))).ToString();
                lblSubTotal.Text = (Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", total.ToString()))).ToString();
                lblIVA.Text = (Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ((Convert.ToDecimal(lblSubTotal.Text)- Convert.ToDecimal(descuento)) * Convert.ToDecimal(0.160000))))).ToString();
                lblTotal.Text = (Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", (Convert.ToDecimal(lblSubTotal.Text) + Convert.ToDecimal(lblIVA.Text)-Convert.ToDecimal(descuento))))).ToString();
            }catch(Exception err)
            {

            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                txtCantidad.Focus();
            }
        }

        private void dgvFactura_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
            totales();
        }

        private void dgvFactura_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            totales();
        }

        private void habilitarTextBox(bool habil)
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    TextBox text = (TextBox)c;
                    text.Enabled = habil;
                }
                if(c is RadioButton)
                {
                    RadioButton text = (RadioButton)c;
                    text.Enabled = habil;
                }
                if (c is ComboBox)
                {
                    ComboBox text = (ComboBox)c;
                    text.Enabled = habil;
                }
            }
            txtFolio.Enabled = false;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            habilitarTextBox(true);
            txtCantidad.Enabled     = false;
            txtProducto.Enabled     = false;
            btnNuevo.Enabled        = false;
            btnEliminar.Enabled     = false;
            btnCancelar.Enabled     = true;
            btnGuardar.Enabled      = true;

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Process calc = new Process { StartInfo = { FileName = @"calc.exe" } };
            calc.Start();
        }

        private void eliminarCotizaciones()
        {
            metodos_VENTA.eliminarVENTA(idCotizacion);
            metodos_DETALLEVENTA.eliminarVENTA(idCotizacion);
        }

        private bool Timbrar()
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
                MessageBox.Show("La factura no pudo timbrarse. \n\nCodigo de Error: " + selloResponse.stampResult.Incidencias[0].CodigoError + "\nDescripción: " + selloResponse.stampResult.Incidencias[0].MensajeIncidencia,"Error al timbrar",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
            else
            {
                doc.LoadXml(xml);
                if (!Directory.Exists(ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim()))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim());
                doc.Save(ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim() + "\\" + txtSerie.Text + txtFolio.Text + ".xml");
                metodos_VENTA.guardarTimbre(ConfigurationManager.AppSettings["noCertificado"], Convert.ToInt64(metodos_VENTA.ultimoIdVenta()), selloResponse.stampResult.Fecha, selloResponse.stampResult.NoCertificadoSAT, selloResponse.stampResult.SatSeal, selloResponse.stampResult.UUID, selloResponse.stampResult.xml, ConfigurationManager.AppSettings["noCertificado"], generaSello(), cmbCFDI.Text);

                string cadenaORiginalSAT = "?re=" + ConfigurationManager.AppSettings["rfcEmisor"] + "&rr=" + lblRFC.Text.Trim() + "&tt=" + lblTotal.Text.Trim() + "&id=" + selloResponse.stampResult.UUID;
                gen_qr_file(ConfigurationManager.AppSettings["rutaXML"] + selloResponse.stampResult.UUID+".png", cadenaORiginalSAT, selloResponse.stampResult.UUID + ".png");

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
            //Upload("ftp://drimexico.com.mx", "pventa@drimexico.com.mx", "dri15032018", new_file_name, "public_html/cbb/", uuid);
            Upload("ftp://eostechnology.com.mx", "pventa@eostechnology.com.mx", "dri15032018", new_file_name, "public_html/cbb/", uuid);
        }

        private int Upload(string server, string user, string pass, string origen, string rutadestino, string nombredestino)

        {
            try
            {
                string ruta = server + "/" + rutadestino + "/" + nombredestino;
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(server + "/" +nombredestino);
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

        private bool validarCajas()
        {
            bool valido = true;
            if (txtMetodoPago.Text == "")
            {
                MessageBox.Show("No olvide capturar el método de pago");return false;
            }
            if (txtFormaPago.Text == "")
            {
                MessageBox.Show("No olvide capturar la Forma de pago"); return false;
            }
            if (txtSerie.Text == "")
            {
                MessageBox.Show("No Olvide capturar la serie"); return false;
            }
            if (txtTC.Text == "")
            {
                MessageBox.Show("No Olvide capturar el tipo de cambio"); return false;
            }
            if (txtFolio.Text == "")
            {
                MessageBox.Show("Asigne una serie para obtener el folio"); return false;
            }
            if (txtMoneda.Text == "")
            {
                MessageBox.Show("Seleccione la moneda para le venta"); return false;
            }
            return valido;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string rutaPdf= ConfigurationManager.AppSettings.Get("nota");
            if (validarCajas())
            {
                if (lblNombre.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("¡Algo anda mal!, Indicanos quien es tu cliente", "Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (dgvFactura.Rows.Count < 1)
                    {
                        MessageBox.Show("¡Espera!, aún no has agregado productos", "Ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        eliminarCotizaciones();
                        txtFolio.Text= serie.seleccionarFOLIO(Convert.ToInt32(txtSerie.SelectedValue)).ToString();
                        metodos_VENTA.insertarVENTA(0, idCliente, DateTime.Now, Convert.ToDouble(lblSubTotal.Text), Convert.ToDouble(lblIVA.Text), Convert.ToDouble(lblTotDesc.Text), Convert.ToDouble(lblTotal.Text), tipoVenta, txtMetodoPago.Text, cmbCondPago.Text, txtFormaPago.Text, lblRFC.Text.Trim(), txtSerie.SelectedValue.ToString(), txtFolio.Text, metodos_PRODUCTOS.seleccionarTipoCambio(),txtComentarios.Text);
                        guardarDetalle();
                        serie.actualizarFOLIO(Convert.ToInt32(txtSerie.SelectedValue), Convert.ToInt32(txtFolio.Text));
                        
                        if (rdbFactura.Checked)
                        {
                            if(generarFactura())
                            {
                                Int32 folioUltimo = metodos_VENTA.ultimoIdVenta();
                                rutaPdf = ConfigurationManager.AppSettings.Get("rutafactura");
                                System.Diagnostics.Process.Start(rutaPdf + folioUltimo);

                                frmMail mail = new frmMail(ConfigurationManager.AppSettings["rutaXML"] + lblNombre.Text.Trim() + "\\" + txtSerie.Text + txtFolio.Text + ".xml", lblRFC.Text.Trim() + "_" + txtSerie.Text + "_" + folioUltimo, e_mail);
                                mail.ShowDialog();
                            }
                            reiniciarControles();
                               
                        }
                        else
                        {
                            if (rdbNota.Checked)
                            {
                                rutaPdf = ConfigurationManager.AppSettings.Get("nota");
                            }else if(rdbPresupuesto.Checked)
                            {
                                rutaPdf = ConfigurationManager.AppSettings.Get("presupuesto");
                            }
                            System.Diagnostics.Process.Start(rutaPdf + metodos_VENTA.ultimoIdVenta());
                            serie.actualizarFOLIO(Convert.ToInt32(txtSerie.SelectedValue), Convert.ToInt32(txtFolio.Text));
                            txtFolio.Text = serie.seleccionarFOLIO(Convert.ToInt32(txtSerie.SelectedValue)).ToString();
                            reiniciarControles();
                        }
                        txtFolio.Text = serie.seleccionarFOLIO(Convert.ToInt32(txtSerie.SelectedValue)).ToString();
                    }
                }
            }
        }

        private void reiniciarControles()
        {
                habilitarTextBox(false);
                btnNuevo.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Enabled = false;
                btnGuardar.Enabled = false;
                lblNombre.Text = string.Empty;
                lblRFC.Text = string.Empty;
                lblLimiteCredito.Text = string.Empty;
                lblSubTotal.Text = "0.00";
                lblIVA.Text = "0.00";
                lblTotal.Text = "0.00";
            lblTotDesc.Text= "0.00";
            lblDescuento.Text = "0.00";
            //lblDescuento.Text = "0.00";
            dgvFactura.Rows.Clear();
                pcbContizaciones.Visible = false;
        }

        private void guardarDetalle()
        {
            Int32 id = metodos_VENTA.ultimoIdVenta();
            foreach(DataGridViewRow row in dgvFactura.Rows)
            {
                metodos_DETALLEVENTA.insertarDETALLEVENTA(0, id, row.Cells[0].Value.ToString(), Convert.ToDecimal(row.Cells[6].Value.ToString()), Convert.ToDecimal(row.Cells[5].Value.ToString()), 0,
                    Convert.ToDecimal(row.Cells[6].Value.ToString()) * Convert.ToDecimal(row.Cells[5].Value.ToString()), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[4].Value.ToString(), Convert.ToDecimal(row.Cells[7].Value.ToString()));

                metodos_PRODUCTOS.restarPRODUCTOS(Convert.ToInt32(row.Cells[0].Value.ToString()), Convert.ToInt32(row.Cells[5].Value.ToString()));
            }
        }

        private void controles_factura(bool habil)
        {
            txtFolio.Enabled = habil;
            txtSerie.Enabled = habil;
            cmbCFDI.Enabled = habil;
            txtMetodoPago.Enabled = habil;
            cmbCondPago.Enabled = habil;
            txtFormaPago.Enabled = habil;
            txtMoneda.Enabled = habil;
            txtTC.Enabled = habil;
        }
        private void tipoDocto()
        {
            bool habil;
            if(rdbPresupuesto.Checked)
            {
                controles_factura(false);
            }
            if (rdbFactura.Checked)
            {
                controles_factura(true);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            habilitarTextBox(false);
            btnNuevo.Enabled = true;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = false;
            btnGuardar.Enabled = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            habilitarTextBox(false);
            btnNuevo.Enabled = true;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = false;
            btnGuardar.Enabled = false;
            lblNombre.Text = string.Empty;
            lblRFC.Text = string.Empty;
            lblLimiteCredito.Text = string.Empty;
            txtProducto.Text = string.Empty;
            txtCantidad.Text = "1";
            pcbContizaciones.Visible = false;
        }

        private void rdbNota_CheckedChanged(object sender, EventArgs e)
        {
            tipoVenta = "NOTA";
            tipoDocto();
        }

        private void rdbFactura_CheckedChanged(object sender, EventArgs e)
        {
            tipoVenta = "FACTURA";
            tipoDocto();
        }

        private void rdbPresupuesto_CheckedChanged(object sender, EventArgs e)
        {
            tipoVenta = "PRESUPUESTO";
            tipoDocto();
        }

        private  decimal costoNacional(PRODUCTOS producto)
        {
            decimal tipoCambio = 1;
            if(producto.moneda!="MXN" && producto.moneda != "MN")
            {
                tipoCambio=metodos_PRODUCTOS.seleccionarTipoCambio();
            }
            return tipoCambio;
        }

        private void txtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                if(lblDescuento.Text.Trim()==string.Empty)
                {
                    lblDescuento.Text = "0.00";
                }
                decimal precio = 0;
                PRODUCTOS producto = metodos_PRODUCTOS.seleccionarPRODUCTOS(lblProducto.Text.Trim());
                if (producto.clave != null)
                {
                    switch (tipoPrecio)
                    {
                        case "PRECIO 1":
                            precio = producto.precio1;
                            break;
                        case "PRECIO 2":
                            precio = producto.precio2;
                            break;
                        case "PRECIO 3":
                            precio = producto.precio3;
                            break;

                    }
                    if (Convert.ToDecimal(lblDescuento.Text) >= 99)
                    {
                        MessageBox.Show("El descuento no puede ser mayor al precio de lista", "Descuentos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        precio = Convert.ToDecimal( String.Format("{0:###0.00;(###0.00);0.00}", precio * costoNacional(producto)) ) ;
                        decimal descuentoTotal = Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}",(Convert.ToInt32(txtCantidad.Text) * precio * Convert.ToDecimal(lblDescuento.Text)) / 100));
                        dgvFactura.Rows.Add(producto.id, producto.clave, producto.claveServicio, producto.claveUnidad,
                            producto.descripcion, txtCantidad.Text, String.Format("{0:###0.00;(###0.00);0.00}", precio.ToString()), String.Format("{0:###0.00;(###0.00);0.00}", descuentoTotal.ToString()) ,
                            String.Format("{0:###0.00;(###0.00);0.00}", (Convert.ToInt32(txtCantidad.Text) * precio)));
                        lblDescuento.Text = "0.00"; 
                        txtProducto.Focus();
                        txtProducto.Text = string.Empty;
                        txtCantidad.Text = "1";
                    }
                }
                else
                {
                    MessageBox.Show("El producto no esta en el inventario");
                    txtProducto.Text = string.Empty;
                    txtCantidad.Text = "1";
                }
                lblProducto.Text = string.Empty;
            }
        }

        private void dgvFactura_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pcbContizaciones_Click(object sender, EventArgs e)
        {
            frmCotizaciones cotizaciones = new frmCotizaciones(idCliente);
            DialogResult result= cotizaciones.ShowDialog();
            if(result== DialogResult.OK)
            {
                dgvFactura.Rows.Clear();
                List<DETALLEVENTA> ventasCot = metodos_DETALLEVENTA.seleccionarDETALLEVENTA(cotizaciones.idVenta);
                foreach(DETALLEVENTA detVta in ventasCot)
                {
                    idCotizacion = detVta.id_venta_fk;
                    dgvFactura.Rows.Add(detVta.id_detalleventa, detVta.clave, detVta.claveunidad, detVta.claveservicio, detVta.descripcion, detVta.cantidad, detVta.precio, detVta.total);
                }
                totales();
            }
        }

        private void txtSerie_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtFolio.Text = serie.seleccionarFOLIO(Convert.ToInt32(txtSerie.SelectedValue)).ToString();
            }catch(Exception err)
            {

            }
        }

        private void txtMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(txtMoneda.Text=="MXN")
            {
                txtTC.Text = "1.00";
                txtTC.Enabled = false;
            }else
            {
                txtTC.Text = "0.00";
                txtTC.Enabled = true;
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


        private bool generarFactura()
        {
            Comprobante objetoComprobante = new Comprobante();
            
            // Nodo comprobante
            objetoComprobante.Version = "3.3";
            objetoComprobante.Serie = txtSerie.Text.Trim();
            objetoComprobante.Folio = txtFolio.Text.Trim();
            objetoComprobante.Fecha = Convert.ToDateTime(DateTime.Now.ToString("s"));

            objetoComprobante.TipoDeComprobante = c_TipoDeComprobante.I;

            objetoComprobante.MetodoPagoSpecified = true;
            objetoComprobante.MetodoPago = txtMetodoPago.SelectedValue.ToString();
            objetoComprobante.FormaPagoSpecified = true;
            objetoComprobante.FormaPago = txtFormaPago.SelectedValue.ToString();

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
            objetoComprobante.Receptor.Rfc =  lblRFC.Text.Trim();
            objetoComprobante.Receptor.Nombre = lblNombre.Text.Trim();
            objetoComprobante.Receptor.UsoCFDI = cmbCFDI.Text.Split('-')[0];


            objetoComprobante.Conceptos = new ComprobanteConcepto[dgvFactura.Rows.Count];


            int indice = 0;
            decimal impuesto = 0;
            decimal descuento = 0;
            decimal total = 0;
            foreach (DataGridViewRow ROW in dgvFactura.Rows)
            {
                objetoComprobante.Conceptos[indice] = new ComprobanteConcepto();

                objetoComprobante.Conceptos[indice].Impuestos = new ComprobanteConceptoImpuestos();



                objetoComprobante.Conceptos[indice].Impuestos.Traslados = new ComprobanteConceptoImpuestosTraslado[1];

                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0] = new                    ComprobanteConceptoImpuestosTraslado();
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Base =                   Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ROW.Cells[8].Value.ToString()))- Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ROW.Cells[7].Value.ToString())); //TODO calcular interes + carga - descuento
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Importe =                Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Base * Convert.ToDecimal(0.160000)))  ; //TODO IVA
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TipoFactor =             c_TipoFactor.Tasa;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Impuesto =               c_Impuesto.Item002; //IVA
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TasaOCuota =             0.160000m;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].ImporteSpecified =       true;
                objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].TasaOCuotaSpecified =    true;

                objetoComprobante.Conceptos[indice].ClaveProdServ =                                 ROW.Cells[3].Value.ToString();
                objetoComprobante.Conceptos[indice].ClaveUnidad =                                   ROW.Cells[2].Value.ToString();
                objetoComprobante.Conceptos[indice].Cantidad =                                      Convert.ToDecimal(ROW.Cells[5].Value);
                objetoComprobante.Conceptos[indice].Unidad =                                        metodos_C_CVEUNIDAD.seleccionarC_CVEUNIDAD(ROW.Cells[2].Value.ToString()).nombre;
                objetoComprobante.Conceptos[indice].Descripcion =                                   ROW.Cells[4].Value.ToString();
                objetoComprobante.Conceptos[indice].ValorUnitario =                                 Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ROW.Cells[6].Value.ToString()));
                objetoComprobante.Conceptos[indice].Importe =                                       Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ROW.Cells[6].Value.ToString()))* Convert.ToDecimal(ROW.Cells[5].Value);
                objetoComprobante.Conceptos[indice].Descuento =                                     Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", ROW.Cells[7].Value.ToString()));
                if(objetoComprobante.Conceptos[indice].Descuento>0)
                    objetoComprobante.Conceptos[indice].DescuentoSpecified = true;
                else
                    objetoComprobante.Conceptos[indice].DescuentoSpecified = false;

                impuesto +=                                                                         objetoComprobante.Conceptos[indice].Impuestos.Traslados[0].Importe;

                descuento +=                                                                        objetoComprobante.Conceptos[indice].Descuento;
                total +=                                                                            objetoComprobante.Conceptos[indice].Importe;
                indice++;
            }


            //Nodo Impuestos
            objetoComprobante.Impuestos =                                                           new ComprobanteImpuestos();
            objetoComprobante.Impuestos.TotalImpuestosTrasladadosSpecified =                        true;
            //
            // Nodo Impuestos Trasladados
            objetoComprobante.Impuestos.Traslados =                                                 new ComprobanteImpuestosTraslado[1];


            objetoComprobante.Impuestos.Traslados[0] =                                              new ComprobanteImpuestosTraslado();
            objetoComprobante.Impuestos.Traslados[0].Importe =                                      Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", impuesto));
            objetoComprobante.Impuestos.Traslados[0].TasaOCuota =                                   0.160000m;
            objetoComprobante.Impuestos.Traslados[0].Impuesto =                                     c_Impuesto.Item002;

            objetoComprobante.Impuestos.TotalImpuestosTrasladados =                                 Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", impuesto)); ;
            objetoComprobante.SubTotal =                                                            Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", total));
            objetoComprobante.Moneda =                                                              c_Moneda.MXN;
            objetoComprobante.Total =                                                               Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", total+ impuesto));
            objetoComprobante.Descuento =                                                           Convert.ToDecimal(String.Format("{0:###0.00;(###0.00);0.00}", descuento));
            if(objetoComprobante.Descuento>0)
                objetoComprobante.DescuentoSpecified =                                              true;
            else
                objetoComprobante.DescuentoSpecified =                                              false;


            // Creas los namespaces requeridos
            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();

            xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
            xmlNameSpace.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
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
            return Timbrar();
            //archivoXml = documentoXml.OuterXml;
            //return "";
        }

        private string generarCadenaOriginal(string myxml)
        {
            string rutaXSLT = @"http://www.eostechnology.com.mx/XSLT/cadenaoriginal_3_3.xslt";
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


        void validarNumeros(KeyPressEventArgs e)
        {

            if (e.KeyChar >= 48 && e.KeyChar <= 57)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == 13)
            {
                //totales();
            }
            else
            {
                MessageBox.Show("Solo se Aceptan Numeros", "Mensaje del Sistema");
                e.Handled = true;
            }
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgProductosLst_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string precioSegunCte = "";
                var cte = (from item in productos
                           where item.clave == this.dgProductosLst.Rows[e.RowIndex].Cells[0].Value.ToString()
                           select item).First();

                switch (tipoPrecio)
                {
                    case "PRECIO 1":
                        precioSegunCte = cte.precio1.ToString();
                        break;
                    case "PRECIO 2":
                        precioSegunCte = cte.precio2.ToString();
                        break;
                    case "PRECIO 3":
                        precioSegunCte = cte.precio3.ToString();
                        break;

                }

                lblProducto.Text = cte.clave;
                lblPrecioPublico.Text = "PRECIO: $"+precioSegunCte;
                txtCantidad.Focus();
                dgProductosLst.Visible = false;
                txtProducto.Text = string.Empty;
            }
            catch (Exception err)
            {

            }
        }

        private void lblDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarNumeros(e);
        }
    }
}