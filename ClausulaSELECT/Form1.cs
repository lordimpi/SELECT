using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClausulaSELECT.Logica;
using System.Text.RegularExpressions;

namespace ClausulaSELECT
{
    /// <summary>
    /// Clase parcial donde se creara todos los controles para el funcionamiento del formulario
    /// </summary>
    public partial class Form1 : Form
    {
        #region Controles del formulario

        #region Formulario
        /// <summary>
        /// Iniciliza todos los componentes para el formulario.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.RtxtComandos.SelectionStart = this.RtxtComandos.Text.Length;
            this.RtxtComandos.TextChanged += (ob, ev) =>
            {
                Posicion = RtxtComandos.SelectionStart;
                ColorearPlabrasClaves();
            };
        }
        /// <summary>
        /// Lista de palabras clave usadas como referencia para colorear en el editos de texto.
        /// </summary>
        string[] Claves = { "SELECT", "FROM", "WHERE", "<", ">", ">=", "<=", "!=", "=" };
        /// <summary>
        /// Variable utilizada para poder ubicar palabras claves y colearlas.
        /// </summary>
        int Posicion = 0;
        Moto moto = new Moto();
        /// <summary>
        /// Método encargado de colorear palabras claves que son digitadas en el editor de texto.
        /// </summary>
        private void ColorearPlabrasClaves()
        {
            this.RtxtComandos.Select(0, RtxtComandos.Text.Length);
            this.RtxtComandos.SelectionColor = Color.Black;
            this.RtxtComandos.Select(Posicion, 0);

            string[] texto = RtxtComandos.Text.Trim().Split(' ');
            int inicio = 0;

            foreach (string PalabraT in texto)
            {
                foreach (string PalabraC in Claves)
                {
                    if (PalabraT.Length != 0)
                    {
                        if (PalabraT.ToUpper().Trim().Equals(PalabraC))
                        {
                            inicio = this.RtxtComandos.Text.IndexOf(PalabraT, inicio);
                            this.RtxtComandos.Select(inicio, PalabraT.Length);
                            RtxtComandos.SelectionColor = Color.DarkGreen;
                            this.RtxtComandos.Select(Posicion, 0);
                            inicio = inicio + 1;
                        }
                    }
                }
            }
        }
        #endregion
        #region Ventanas
        /// <summary>
        /// Restaura el formulario por completo a su forma original.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void RestoreWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            RestoreWindow.Visible = false;
            MaximizeWindow.Visible = true;
        }
        /// <summary>
        /// Minimiza la ventanda del formulario.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void MinimizeWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// Maximiza la ventana del formulario.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void MaximizeWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            MaximizeWindow.Visible = false;
            RestoreWindow.Visible = true;
        }
        /// <summary>
        /// Cierra el formulario por completo.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void CloseWindow_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
        #region Botones
        /// <summary>
        /// Se encarga de borrar los comandos por completo 
        /// en el editor de texto para un nuevo uso.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            this.RtxtComandos.Clear();
            this.dataGridResultado.Columns.Clear();
            this.lblErrores.Text = "";
        }
        /// <summary>
        /// Ejecuta un comando ingresado por el editor de texto.
        /// </summary>
        /// <param name="sender">Guarda una referencia al objeto que lanza un evento.</param>
        /// <param name="e">Contiene los eventos lanzados por el control.</param>
        private void BtnEjecutar_Click(object sender, EventArgs e)
        {
            lblErrores.Text = "";
            string select = RtxtComandos.Text;
            if (!select.Equals(""))
            {
                string respuesta = moto.ValidarConsulta(select);
                if (respuesta.Equals("asterisco") || respuesta.Equals("columnas") || respuesta.Equals("asteriscoWhere") || respuesta.Equals("columnasWhere"))
                {

                    DataSet dt = new DataSet();
                    select = select.ToUpper();
                    respuesta = moto.ValidarBaseDatos(select, respuesta);
                    string[] split = Regex.Split(respuesta, @",");
                    if (split[0].Equals("SI"))
                    {
                        dt = moto.Set;
                        dataGridResultado.DataSource = dt;
                        dataGridResultado.DataMember = "MOTOS";
                    }
                    lblErrores.Text += split[1];
                    lblErrores.Text += moto.MensajeMetodo;
                }
                else
                {
                    this.dataGridResultado.Columns.Clear();
                    lblErrores.Text = respuesta;
                }
            } else
            {
                lblErrores.Text = "Consulta en Blanco";
            }
        }
    }
    #endregion

        #endregion

}
