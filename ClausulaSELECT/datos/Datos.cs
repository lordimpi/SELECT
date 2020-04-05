using System;
using Oracle.DataAccess.Client;
using System.Data;

namespace ClausulaSELECT.datos
{
    /// <summary>
    /// Clase encargada de realizar conexion con la base de Datos para utlizar sus procedimientos almacenados.
    /// </summary>
    public class Datos
    {
        #region Argunmentos
        /// <summary>
        /// Guarda la instancia con la cadena de conexión a la Base de Datos ORACLE.
        /// </summary>
        OracleConnection connection = new OracleConnection(@"Data Source=LOCALHOST;User ID=BD2;Password=oracle");
        /// <summary>
        /// Guarda el ultimo mensaje del metodo utilizado.
        /// </summary>
        public string MensajeMetodo { get; set; }
        /// <summary>
        /// Captura la exepcion producida por un error en procesos de la Base de Datos ORACLE.
        /// </summary>
        public string ExceptionOracle { get; set; }
        #endregion
        #region Métodos
        /// <summary>
        /// Método que se encarga de ejectuar el procedimiento almacenado que está en el paquete MOTO_PACKAGE de la base de datos BD2
        /// </summary>
        /// <returns>Todas las motos que están en la tabla MOTO</returns>
        public DataSet Consulta(string pColumna)
        {
            this.ExceptionOracle = "";
            try
            {
                //abrir la conexion
                connection.Open();
                OracleCommand ora_cmd = new OracleCommand("BD2.MOTO_PACKAGE.P_CONSULTARMOTOS", connection);
                ora_cmd.Parameters.Add("O_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                ora_cmd.Parameters.Add("V_COL", OracleDbType.Varchar2, pColumna, ParameterDirection.Input);
                ora_cmd.CommandType = CommandType.StoredProcedure;
                //un OracleDataAdapter permite llenar un dataset
                OracleDataAdapter da = new OracleDataAdapter(ora_cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "MOTOS");
                this.MensajeMetodo = "\nConsulta realizada con éxito";
                return ds;
            }
            catch (Exception e)
            {
                this.ExepcionOracle(e.Message);
                this.MensajeMetodo = "\nERROR: " + this.ExceptionOracle;
                return null;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
        }
        /// <summary>
        /// Método que se encarga de ejectuar el procedimiento almacenado que está en el 
        /// paquete MOTO_PACKAGE de la Base de Datos "BD2 para realizar consultas condicionales."
        /// </summary>
        /// <param name="pColumna">Columnas que se utilizaran para realizar la consulta.</param>
        /// <param name="pCondicion">Condicion que se utilizara para realizar la consulta.</param>
        /// <returns>Retorna una cache de datos en memoria.</returns>
        public DataSet ConsultaWhere(string pColumna, string pCondicion)
        {
            this.ExceptionOracle = "";
            try
            {
                //abrir la conexion
                connection.Open();
                OracleCommand ora_cmd = new OracleCommand("BD2.MOTO_PACKAGE.P_CONSULTARMOTOSWHERE", connection);
                ora_cmd.Parameters.Add("O_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                ora_cmd.Parameters.Add("V_COL", OracleDbType.Varchar2, pColumna, ParameterDirection.Input);
                ora_cmd.Parameters.Add("V_COND", OracleDbType.Varchar2, pCondicion, ParameterDirection.Input);
                ora_cmd.CommandType = CommandType.StoredProcedure;
                //un OracleDataAdapter permite llenar un dataset
                OracleDataAdapter da = new OracleDataAdapter(ora_cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "MOTOS");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    this.MensajeMetodo = "\nError: NO DATA FOUND";
                }
                else
                {
                    this.MensajeMetodo = "\nConsulta realizada con éxito";
                }
                return ds;
            }
            catch (Exception e)
            {
                this.ExepcionOracle(e.Message);
                this.MensajeMetodo = "\nERROR: " + this.ExceptionOracle;
                return null;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
        }
        /// <summary>
        /// Método que se encarga de ejectuar el procedimiento almacenado que está en el 
        /// paquete MOTO_PACKAGE de la Base de Datos "BD2 para realizar consultas en el Diccionario de Datos."
        /// </summary>
        /// <param name="pColumna">Columnas que se utilizaran para realizar la consulta.</param>
        /// <returns>Retorna una cache de datos en memoria.</returns>
        public DataSet ConsultaDiccionario(string pColumnas)
        {
            this.ExceptionOracle = "";
            try
            {
                //abrir la conexion
                connection.Open();
                OracleCommand ora_cmd = new OracleCommand("BD2.MOTO_PACKAGE.P_CONSULTADICCIONARIO", connection);
                ora_cmd.Parameters.Add("O_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                ora_cmd.Parameters.Add("V_COL", OracleDbType.Varchar2, pColumnas, ParameterDirection.Input);
                ora_cmd.CommandType = CommandType.StoredProcedure;
                //un OracleDataAdapter permite llenar un dataset
                OracleDataAdapter da = new OracleDataAdapter(ora_cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "MOTOS");
                this.MensajeMetodo = "\nConsulta realizada con éxito";
                return ds;
            }
            catch (Exception e)
            {
                this.ExepcionOracle(e.Message);
                this.MensajeMetodo = "\nERROR: " + this.ExceptionOracle;
                return null;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// Método que se encarga de ejectuar el procedimiento almacenado que está en el 
        /// paquete MOTO_PACKAGE de la Base de Datos "BD2 para consultar el numero de tuplas que tiene una 
        /// tabla."
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla para realizar el conteo.</param>
        /// <returns>Retorna un cadena donde "0" es que no hay datos, 
        /// mayor que "0" es que hay datos y "-1" que ocurrio un problema en la consulta.</returns>
        public string ObtenerCantidadFilas(string pNombreTabla)
        {
            this.ExceptionOracle = "";
            String cantidadFilas;
            try
            {
                connection.Open();
                OracleCommand command = new OracleCommand("BD2.MOTO_PACKAGE.P_CANTIDADFILAS", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("V_NOMBRETABLA", OracleDbType.Varchar2, pNombreTabla, ParameterDirection.Input);
                command.Parameters.Add("V_CANTIDAD", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                cantidadFilas = command.Parameters["V_CANTIDAD"].Value.ToString();
                return cantidadFilas;
            }
            catch (Exception e)
            {
                this.ExepcionOracle(e.Message);
                this.MensajeMetodo = "\nERROR: " + this.ExceptionOracle;
                return "-1";
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// Método que se encarga de ejectuar el procedimiento almacenado que está en el 
        /// paquete MOTO_PACKAGE de la Base de Datos "BD2 para consultar la existencia de una tabla."
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla a consultar.</param>
        /// <returns>Retorna un cadena donde "TRUE" es que existe, "FALSE" de lo contrario.</returns>
        public string existeTabla(string pNombreTabla)
        {
            this.ExceptionOracle = "";
            String existe;
            try
            {
                connection.Open();
                OracleCommand command = new OracleCommand("BD2.MOTO_PACKAGE.P_BUSCARTABLA", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("PNOMBRETABLA", OracleDbType.Varchar2, pNombreTabla, ParameterDirection.Input);
                command.Parameters.Add("V_EXISTE", OracleDbType.Varchar2, 256).Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                existe = command.Parameters["V_EXISTE"].Value.ToString();
                return existe;
            }
            catch (Exception e)
            {
                this.ExepcionOracle(e.Message);
                this.MensajeMetodo = "\nERROR: " + this.ExceptionOracle;
                return "NO";
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// Método encargado de buscar y asignar una excepción
        /// </summary>
        /// <param name="pExcepcion"></param>
        private void ExepcionOracle(string pExcepcion)
        {
            switch (pExcepcion.Substring(0, 9))
            {
                case "ORA-00904":
                    this.ExceptionOracle = "Identificador invalido";
                    break;
                case "ORA-01722":
                    this.ExceptionOracle = "Tipo de dato no compatible";
                    break;
                case "ORA-00920":
                    this.ExceptionOracle = "Error de semantica";
                    break;
            }
        }
        #endregion
    }
}
