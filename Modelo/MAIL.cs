using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    public class MAIL
    {
        int ID;
        string EMAIL;
        string PSW;

        public string email
        {
            get { return this.EMAIL; }
            set { this.EMAIL = value; }
        }

        public Int32 id
        {
            get { return this.ID; }
            set { this.ID = value; }
        }

        public string pasw
        {
            get { return this.PSW; }
            set { this.PSW = value; }
        }

    }

    public static class metodos_MAIL
    {

        public static MAIL seleccionarYahoo()
        {
            MAIL mail = null;
            String Qry = @"SELECT * FROM mail where id=1";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);

            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                mail = new MAIL();
                mail.id = Convert.ToInt32(datosItems[0].ToString());
                mail.email = datosItems[1].ToString();
                mail.pasw = datosItems[2].ToString();
            }
            datosItems.Close();
            d.Desconectar();
            return mail;
        } 
        public static MAIL seleccionarGMAIL()
        {
            MAIL mail=null;
            String Qry = @"SELECT * FROM mail where id=2";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);

            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                 mail = new MAIL();
                mail.id = Convert.ToInt32(datosItems[0].ToString());
                mail.email = datosItems[1].ToString();
                mail.pasw = datosItems[2].ToString();
            }
            datosItems.Close();
            d.Desconectar();
            return mail;
        }
    }
}
