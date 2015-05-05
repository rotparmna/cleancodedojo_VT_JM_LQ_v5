namespace CliCountry.Facturacion.Dominio.Excepciones
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public class FacturacionException : Exception, ISerializable
    {
        /// <summary>
        /// Excpecion generada.
        /// </summary>
        public Exception ExcepcionGenerada { get; set; }

        /// <summary>
        /// Origen excepcion.
        /// </summary>
        public string OrigenExcepcion { get; set; }

        /// <summary>
        /// Constructur de factura exception.
        /// </summary>
        public FacturacionException()
        {

        }

        /// <summary>
        /// Constructor excepcion generica.
        /// </summary>
        /// <param name="ExcepcionGenerada"></param>
        public FacturacionException(Exception excepcionGenerada, string origenExcepcion)
        {
            this.ExcepcionGenerada = ExcepcionGenerada;
            this.OrigenExcepcion = origenExcepcion;
        }
    }
}
