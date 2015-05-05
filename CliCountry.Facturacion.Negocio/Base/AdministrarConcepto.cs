using CliCountry.SAHI.Dominio.Entidades;
using CliCountry.SAHI.Dominio.Entidades.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliCountry.Facturacion.Negocio.Base
{
    public class AdministrarConcepto
    {
        public static AtencionCliente AplicarConceptosAtencion(AtencionCliente atencion)
        {
            var resultadoConceptos = ObtenerConceptos(
                new Atencion()
                {
                    IdAtencion = atencion.IdAtencion
                });

            if (resultadoConceptos.Ejecuto)
            {
                if (atencion.Deposito != null)
                {
                    atencion.Deposito.Concepto = resultadoConceptos.Objeto.ToList();
                }
            }
            else
            {
                throw new InvalidOperationException (resultadoConceptos.Mensaje);
            }

            return atencion;
        }

        /// <summary>
        /// Permite Consultar Conceptos Asociados a una Atenci n.
        /// </summary>
        /// <param name="atencion">The atencion.</param>
        /// <returns>Lista de conceptos.</returns>
        /// <remarks>
        /// Autor: Jorge Arturo Cortes - INTERGRUPO\jcortesm.
        /// FechaDeCreacion: 04/06/2013.
        /// UltimaModificacionPor: (Nombre del Autor de la modificacion - Usuario del dominio).
        /// FechaDeUltimaModificacion: (dd/MM/yyyy).
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio).
        /// Descripcion: (Descripcion detallada del metodo, procure especificar todo el metodo aqui).
        /// </remarks>
        private static Resultado<List<ConceptoCobro>> ObtenerConceptos(Atencion atencion)
        {
            Resultado<List<ConceptoCobro>> resultado = new Resultado<List<ConceptoCobro>>();

            try
            {
                Negocio.Base.Facturacion facturacion = new Negocio.Base.Facturacion();
                resultado.Ejecuto = true;
                resultado.Objeto = facturacion.ConsultarConceptos(atencion);
            }
            catch (Exception exception)
            {
                resultado.Ejecuto = false;
                resultado.Mensaje = exception.Message;
                resultado.Objeto = null;
            }

            return resultado;
        }
    }
}
