using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode;
namespace Modelo
{
    public class QrCBB
    {

        private Int32 AdjustQRVersion(String TextEncode, ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION qrCodeErrCorrect)
        {
            Byte[] textb = System.Text.UTF8Encoding.UTF8.GetBytes(TextEncode);
            Int32 ibits = ((textb.Length + 1) * 8) + 8;

            //return -1;
            switch(qrCodeErrCorrect)
            {
                case QRCodeEncoder.ERROR_CORRECTION.Q:

                    if (ibits <= 104)
                        return 1;
                    if (ibits <= 176)
                        return 2;
                    if (ibits <= 272)
                        return 3;
                    if (ibits <= 384)
                        return 4;
                    if (ibits <= 496)
                        return 5;
                    if (ibits <= 608)
                        return 6;
                    if (ibits <= 704)
                        return 7;
                    if (ibits <= 880)
                        return 8;
                    if (ibits <= 1056)
                        return 9;
                    if (ibits <= 1232)
                        return 10;
                    if (ibits <= 1440)
                        return 11;
                    if (ibits <= 1648)
                        return 12;
                    if (ibits <= 1952)
                        return 13;
                    if (ibits <= 2088)
                        return 14;
                    if (ibits <= 2360)
                        return 15;
                    if (ibits <= 2600)
                        return 16;
                    if (ibits <= 2936)
                        return 17;
                    if (ibits <= 3176)
                        return 18;
                    if (ibits <= 3560)
                        return 19;
                    if (ibits <= 3880)
                        return 20;
                    if (ibits <= 4096)
                        return 21;
                    if (ibits <= 4544)
                        return 22;
                    if (ibits <= 4912)
                        return 23;
                    if (ibits <= 5312)
                        return 24;
                    if (ibits <= 5744)
                        return 25;
                    if (ibits <= 6032)
                        return 26;
                    if (ibits <= 6464)
                        return 27;
                    if (ibits <= 6968)
                        return 28;
                    if (ibits <= 7288)
                        return 29;
                    if (ibits <= 7880)
                        return 30;
                    if (ibits <= 8264)
                        return 31;
                    if (ibits <= 8920)
                        return 32;
                    if (ibits <= 9368)
                        return 33;
                    if (ibits <= 9848)
                        return 34;
                    if (ibits <= 10288)
                        return 35;
                    if (ibits <= 10832)
                        return 36;
                    if (ibits <= 11408)
                        return 37;
                    if (ibits <= 12016)
                        return 38;
                    if (ibits <= 12656)
                        return 39;
                    if (ibits <= 13328)
                        return 40;
                    else
                        return -1;
            }
            return -1;
        }

        public bool crear(string Emisor, string receptor, string total, string uuid)
        {


            Int32 EscalaPixel = 104;


            //    Dim generarCodigoQR As QRCodeEncoder = New QRCodeEncoder
            QRCodeEncoder generarCodigoQR = new QRCodeEncoder();
            //    generarCodigoQR.QRCodeEncodeMode = Codec.QRCodeEncoder.ENCODE_MODE.BYTE
            generarCodigoQR.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            generarCodigoQR.QRCodeScale = EscalaPixel;


            generarCodigoQR.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.Q;


            //    'La versión "8" calcula automáticamente el tamaño
            generarCodigoQR.QRCodeVersion = 8;
            try
            {
                String txtTextoQR = "?re=" + Emisor + "&rr=" + receptor + "&tt=" + total + "&id=" + uuid;
               // generarCodigoQR.Encode(txtTextoQR, System.Text.Encoding.UTF8).
                return true;
            }
            catch(Exception err)
            {
                //        MsgBox(ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly)
                return false;
            }
        }
    }
}
