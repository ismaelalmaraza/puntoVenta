using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    public class marca
    {
        int ID;
        string MARCA;
        public string Marca
        {
            get { return this.MARCA; }
            set { this.MARCA = value; }
        }

        public int Id
        {
            get { return this.ID; }
            set { this.ID = value; }
        }
    }


    public class linea
    {
        int ID;
        string LINEA;
        string DESCRIPCION;
        public string descripcion
        {
            get { return this.DESCRIPCION; }
            set { this.DESCRIPCION = value; }
        }

        public string Linea
        {
            get { return this.LINEA; }
            set { this.LINEA = value; }
        }
        public int Id
        {
            get { return this.ID; }
            set { this.ID = value; }
        }
    }

    public class metodos
    {
        public string getLineasStr(string str)
        {
            string linea=string.Empty;
            String Qry = @"SELECT * from linea where linea=@id";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);
            d.AsignarParametroCadena("@id", str);
            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                linea = datosItems["id"].ToString();
            }
            datosItems.Close();
            d.Desconectar();

            return linea;
        }

        public List<linea> getLineas(string id)
        {
            List<linea> lineas = new List<linea>();
            String Qry = @"SELECT * from linea where id=@id";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);
            d.AsignarParametroCadena("@id", id);
            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                linea l = new linea();
                l.Id = Convert.ToInt32(datosItems["id"].ToString());
                l.Linea = datosItems["linea"].ToString();
                lineas.Add(l);
            }
            datosItems.Close();
            d.Desconectar();

            return lineas;
        }

        public List<linea> getLineas()
        {
            List<linea> lineas = new List<linea>();
            String Qry = @"SELECT * from linea";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);
            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                linea l = new linea();
                l.Id = Convert.ToInt32(datosItems["id"].ToString());
                l.Linea = datosItems["linea"].ToString() +" - "+datosItems["descripcion"].ToString() ;
                lineas.Add(l);
            }
            datosItems.Close();
            d.Desconectar();

            return lineas;
        }

        public List<marca> getMarca(string id)
        {
            List<marca> marcas = new List<marca>();
            String Qry = @"SELECT * from marca where id=@id";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);
            d.AsignarParametroCadena("@id",id);
            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                marca m = new marca();
                m.Id = Convert.ToInt32(datosItems["id"].ToString());
                m.Marca = datosItems["marca"].ToString();
                marcas.Add(m);
            }
            datosItems.Close();
            d.Desconectar();

            return marcas;
        }

        public List<marca> getMarca()
        {
            List<marca> marcas = new List<marca>();
            String Qry = @"SELECT * from marca";
            BaseDatos d = new BaseDatos();
            d.Conectar();
            d.CrearComando(Qry);
            DbDataReader datosItems = d.EjecutarConsulta();
            while (datosItems.Read())
            {
                marca m = new marca();
                m.Id = Convert.ToInt32(datosItems["id"].ToString());
                m.Marca = datosItems["marca"].ToString();
                marcas.Add(m);
            }
            datosItems.Close();
            d.Desconectar();

            return marcas;
        }
    }
}
