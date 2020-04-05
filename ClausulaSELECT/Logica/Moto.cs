using System.Data;
using ClausulaSELECT.datos;
using System.Text.RegularExpressions;
namespace ClausulaSELECT.Logica
{
    public class Moto
    {
        #region Argumentos
        Datos dt = new Datos();
        public string MensajeMetodo { get; set; }
        private DataSet set;
        public DataSet Set
        {
            get
            {
                return set;
            }
        }
        #endregion
        #region Métodos
        #region Consultas a la base de datos
        public bool ConsultarMotos(string str)
        {
            set = dt.Consulta(str);
            this.MensajeMetodo = dt.MensajeMetodo;
            return (set != null);
        }
        public bool ConsultarMotosWhere(string str, string condicion)
        {
            set = dt.ConsultaWhere(str, condicion);
            this.MensajeMetodo = dt.MensajeMetodo;
            return (set != null);
        }

        #region Diccionario datos y validaciones
        public string CantidadFilas(string tabla)
        {
            string respuesta = dt.ObtenerCantidadFilas(tabla);
            return respuesta;
        }
        #endregion
        #endregion
        #region Manejo de Errores
        public string ManejoErrores(string consulta)
        {
            string error = string.Format("Errores de Sintaxis:\n{0}", ErrorSintaxis(consulta));
            error = (ErrorSemantica(consulta)) ? string.Format("{0}\nError de Semantica", error) : error;
            return error;
        }
        private string ErrorSintaxis(string consulta)
        {
            return ReporteErroresSintaxis(consulta);
        }
        #region Reporte Errores Semantica
        private bool ErrorSemantica(string consulta)
        {
            return ReporteErrorSemantica(consulta);
        }
        private bool ReporteErrorSemantica(string consulta)
        {
            Regex regex = new Regex(@"^\s*SELECT\s+(\w+|\w+(\s*,\s*\w+)+|\*)\s+FROM\s+\w+(\s+WHERE\s+\w+(\s+IS NULL\s*|\s*(=|<|>|<=|>=|!=)\s*(\w+|\s*'\s*\w+\s*'\s*)))?$");
            if (regex.IsMatch(consulta))
                return false;
            return true;
        }

        #endregion
        private string ReporteErroresSintaxis(string consulta)
        {
            string[] array = DividirConsulta(consulta);
            string columnas = VerificarColumnas(array[1]);
            array[1] = columnas;
            string respuesta = "";
            if (array.Length > 5)
            {
                columnas = VerificarColumnas(array[5]);
                array[5] = columnas;
            }


            foreach (string str in array)
            {
                if (!str.Equals("SELECT") && !str.Equals("*") && !str.Equals("FROM") && !str.Equals("MOTO") && !str.Equals("WHERE") && !str.Equals("<") && !str.Equals(">") && !str.Equals("=") && !str.Equals("<=") && !str.Equals(">=") && !str.Equals("IS NULL") && !str.Equals(""))
                {
                    respuesta += string.Format("Error en la sintaxis de {0} o palabra es desconocida\n", str);
                }
            }
            return (respuesta.Equals("")) ? "Ninguno" : respuesta;
        }
        #endregion
        #region Formato Consultas
        private string FormatoConsulta(string consulta)
        {
            bool encontradoFormato = false;
            string formato = "ninguno";

            for (int i = 0; i < 5 && !encontradoFormato; i++)
            {
                switch (i)
                {
                    case 0:
                        encontradoFormato = FormatoAsterisco(consulta);
                        if (encontradoFormato)
                            formato = "asterisco";
                        break;
                    case 1:
                        encontradoFormato = FormatoColumnas(consulta);
                        if (encontradoFormato)
                            formato = "columnas";
                        break;
                    case 2:
                        encontradoFormato = FormatoAsteriscoWhere(consulta);
                        if (encontradoFormato)
                            formato = "asteriscoWhere";
                        break;
                    case 3:
                        encontradoFormato = FormatoColumnasWhere(consulta);
                        if (encontradoFormato)
                            formato = "columnasWhere";
                        break;
                }
            }
            return formato;
        }
        private bool FormatoAsterisco(string consulta)
        {
            Regex regex = new Regex(@"^\s*SELECT\s+\*\s+FROM\s+MOTO\s*$");
            if (regex.IsMatch(consulta))
                return true;
            return false;
        }
        private bool FormatoColumnas(string consulta)
        {
            Regex regex = new Regex(@"^\s*SELECT\s+\w+\s*(,\s*\w+)*\s+FROM\s+\w+\s*$");
            if (regex.IsMatch(consulta))
                return true;
            return false;
        }
        private bool FormatoColumnasWhere(string consulta)
        {
            Regex regex = new Regex(@"^\s*SELECT\s+\w+(\s*,\s*\w+)*\s+FROM\s+\w+\s+WHERE\s+\w+\s*(((=|<|>|>=|<=|!=)\s*(\w+|'(\s*\w*\s*)+'))|IS NULL)\s*$");
            if (regex.IsMatch(consulta))
                return true;
            return false;
        }
        private bool FormatoAsteriscoWhere(string consulta)
        {
            Regex regex = new Regex(@"^\s*SELECT\s+\*\s+FROM\s+\w+\s+WHERE\s+\w+\s*(((=|<|>|>=|<=|!=)\s*(\w+|'(\s*\w*\s*)+'))|IS NULL)\s*$");
            if (regex.IsMatch(consulta))
                return true;
            return false;
        }
        #endregion
        #region Logica de Validaciones
        /// <summary>
        /// Valida si la consulta esta correcta, si no es así devuelve los errores en la consulta
        /// </summary>
        /// <param name="consulta"></param>
        /// <returns></returns>
        public string ValidarConsulta(string consulta)
        {
            consulta = consulta.ToUpper();
            string formato = FormatoConsulta(consulta);
            if (formato.Equals("ninguno"))
            {
                return ManejoErrores(consulta);
            }
            return formato;
        }
        public string ValidarBaseDatos(string consulta, string formato)
        {
            string respuesta;
            string[] partes = DividirConsulta(consulta);
            string tabla = dt.existeTabla(partes[3]);
            respuesta = string.Format("La tabla {1} {0} existe.\n", tabla, partes[3]);
            string where = "";
            bool hecho = false;
            if (tabla.Equals("SI"))
            {
                string cantidad = CantidadFilas(partes[3]);
                if (!cantidad.Equals("-1") && !cantidad.Equals("0"))
                {
                    respuesta = string.Format("{0}La tabla {1} tiene {2} tuplas.\n", respuesta, partes[3], cantidad);
                    respuesta = string.Format("{0}{1}", respuesta, ConsultarDiccionario(partes[1]));
                    switch (formato)
                    {
                        case "asterisco":
                            hecho = ConsultarMotos("*");
                            break;
                        case "columnas":
                            hecho = ConsultarMotos(partes[1]);
                            break;
                        case "asteriscoWhere":
                            for (int i = 4; i < partes.Length; i++)
                            {
                                if (partes[i].Equals("'U'") || partes[i].Equals("'N'"))
                                    where = string.Format("{0} {1}", where, partes[i].ToLower());
                                else
                                    where = string.Format("{0} {1}", where, partes[i]);
                            }
                            hecho = ConsultarMotosWhere("*", where);
                            break;
                        case "columnasWhere":
                            for (int i = 4; i < partes.Length; i++)
                            {
                                if (partes[i].Equals("'U'") || partes[i].Equals("'N'"))
                                    where = string.Format("{0} {1}", where, partes[i].ToLower());
                                else
                                    where = string.Format("{0} {1}", where, partes[i]);
                            }
                            hecho = ConsultarMotosWhere(partes[1], where);
                            break;
                    }
                }
                else
                {
                    respuesta = string.Format("{0}La tabla {1} NO tiene tuplas.\n", respuesta, partes[3]);
                }
            }
            if (!hecho)
                EncontrarError(formato, consulta);
            respuesta = (hecho) ? string.Format("SI,{0}", respuesta) : string.Format("NO,{0}", respuesta);

            return respuesta;
        }
        private string[] DividirConsulta(string consulta)
        {
            consulta = Regex.Replace(consulta, "\\s+", " ");
            consulta = Regex.Replace(consulta, "\\s*,\\s*", ",");
            return Regex.Split(consulta, @"\s");
        }

        #endregion
        #region Consultar Diccionario
        public string ConsultarDiccionario(string allColumnas)
        {
            string str = "\nInformación sobre las columnas:\n";
            string[] columnas;
            if (allColumnas.Equals("*"))
            {
                columnas = new string[] { "MOTCODIGO", "MOTPLACA", "MOTFECHAELAB", "MOTESTADO" };
            }
            else
            {
                columnas = Regex.Split(allColumnas, @",");
            }
            var data = dt.ConsultaDiccionario("COLUMN_NAME, DATA_TYPE");
            if (data != null)
            {
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < columnas.Length; j++)
                    {
                        if (data.Tables[0].Rows[i].ItemArray[0].Equals(columnas[j]))
                            str = string.Format("{0}La columna: {1} es de tipo: {2}\n", str, data.Tables[0].Rows[i].ItemArray[0], data.Tables[0].Rows[i].ItemArray[1]);
                    }
                }
            }
            else
            {
                str += "NJ se pudo consultar la información de las columnas";
            }
            this.MensajeMetodo = dt.MensajeMetodo;
            return str;
        }
        public string VerificarColumnas(string allColumnas)
        {
            string[] columnas;
            if (allColumnas.Equals("*"))
            {
                columnas = new string[] { "MOTCODIGO", "MOTPLACA", "MOTFECHAELAB", "MOTESTADO" };
            }
            else
            {
                columnas = Regex.Split(allColumnas, @"\s*,\s*");
            }
            var data = dt.ConsultaDiccionario("COLUMN_NAME, DATA_TYPE");
            if (data != null)
            {
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < columnas.Length; j++)
                    {
                        if (data.Tables[0].Rows[i].ItemArray[0].Equals(columnas[j]))
                            allColumnas = Regex.Replace(allColumnas, columnas[j], "");
                    }
                }
                allColumnas = Regex.Replace(allColumnas, ",", "");
            }
            return allColumnas;
        }
        private void EncontrarError(string formato, string consulta)
        {
            string[] partes = DividirConsulta(consulta);
            string error = dt.ExceptionOracle;
            var data = dt.ConsultaDiccionario("COLUMN_NAME, DATA_TYPE");
            string cols = partes[1];
            string[] columnas = Regex.Split(cols, @"\s*,\s*");

            switch (formato)
            {
                case "columnas":
                    if (error.Equals("Identificador invalido"))
                    {
                        for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < columnas.Length; j++)
                            {
                                if (data.Tables[0].Rows[i].ItemArray[0].Equals(columnas[j]))
                                {
                                    cols = Regex.Replace(cols, columnas[j], "");
                                }
                            }
                        }
                        string[] c = Regex.Split(cols, ",");
                        this.MensajeMetodo += string.Format(":\n");
                        foreach (string l in c)
                        {
                            this.MensajeMetodo += string.Format("La columna {0} no existe\n", l);
                        }
                    }
                    break;
                case "columnasWhere":
                    if (error.Equals("Identificador invalido"))
                    {
                        for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < columnas.Length; j++)
                            {
                                if (data.Tables[0].Rows[i].ItemArray[0].Equals(columnas[j]))
                                {
                                    cols = Regex.Replace(cols, columnas[j], "");
                                }
                            }
                        }
                        string[] c = Regex.Split(cols, ",");
                        this.MensajeMetodo += string.Format(":\n");
                        foreach (string l in c)
                        {
                            if (!l.Equals(""))
                                this.MensajeMetodo += string.Format("La columna {0} no existe\n", l);
                        }
                    }
                    break;
            }

        }
        #endregion
        #endregion
    }
}
