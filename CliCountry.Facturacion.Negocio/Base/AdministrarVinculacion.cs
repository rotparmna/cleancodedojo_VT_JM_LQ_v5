using CliCountry.Facturacion.Dominio.Excepciones;
using CliCountry.SAHI.Dominio.Entidades.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliCountry.Facturacion.Negocio.Base
{
    public class AdministrarVinculacion
    {
        /// <summary>
        /// Busca información de una Vinculación a partir del identificador de la entidad y su número de orden.
        /// </summary>
        /// <param name="identificadorEntidad">Identificador de la entidad.</param>
        /// <param name="ordenVinculacion">Número de orden.</param>
        /// <returns>
        /// Retorna la Vinculacion.
        /// </returns>
        public static Vinculacion BuscarVinculacion(int identificadorEntidad, int ordenVinculacion, List<Vinculacion> vinculaciones)
        {
            try
            {
                var vinculacion = from
                                      item in vinculaciones
                                  where
                                      item.Tercero.Id == identificadorEntidad
                                      && item.Orden == ordenVinculacion
                                  select
                                      item;

                return vinculacion.FirstOrDefault();
            }
            catch (ArgumentException error)
            {
                throw new FacturacionException(error, "BuscarVinculacion(int identificadorEntidad, int ordenVinculacion, List<Vinculacion> vinculaciones)");
            }
            
        }
    }
}
