// --------------------------------
// <copyright file="Reportes.cs" company="InterGrupo S.A.">
//     COPYRIGHT(C) 2013, Intergrupo S.A
// </copyright>
// ---------------------------------
namespace CliCountry.Facturacion.Web.WebExterno.Comun.Extensiones
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.Reporting.WebForms;

    /// <summary>
    /// Clase CliCountry.Facturacion.Web.WebExterno.Comun.Extensiones.Reportes
    /// </summary>
    public static class Reportes
    {
        #region�Metodos

        #region�Metodos�Publicos�Estaticos

        /// <summary>
        /// M�todo de extensi�n para deshabilitar las extensiones de exportaci�n de los reportes
        /// </summary>
        /// <param name="serverReport">El objeto ServerReport.</param>
        /// <param name="formatsToDisable">Listado de extensiones a deshabilitar.</param>
        /// <remarks>
        /// Autor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeCreacion: 28/02/2013
        /// UltimaModificacionPor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeUltimaModificacion: 28/02/2013
        /// EncargadoSoporte: William V�squez R. - INTRGRUPO\wvasquez
        /// Descripci�n:
        /// </remarks>
        public static void DisableUnwantedExportFormats(this ServerReport serverReport, params string[] formatsToDisable)
        {
            foreach (RenderingExtension extension in serverReport.ListRenderingExtensions())
            {
                if (formatsToDisable.Contains(extension.Name))
                {
                    ReflectivelySetVisibilityFalse(extension);
                }
            }
        }

        /// <summary>
        /// M�todo de extensi�n para deshabilitar las extensiones de exportaci�n de los reportes
        /// </summary>
        /// <param name="localReport">El objeto LocalReport.</param>
        /// <param name="formatsToDisable">Listado de extensiones a deshabilitar.</param>
        /// <remarks>
        /// Autor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeCreacion: 28/02/2013
        /// UltimaModificacionPor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeUltimaModificacion: 28/02/2013
        /// EncargadoSoporte: William V�squez R. - INTRGRUPO\wvasquez
        /// Descripci�n:
        /// </remarks>
        public static void DisableUnwantedExportFormats(this LocalReport localReport, params string[] formatsToDisable)
        {
            foreach (RenderingExtension extension in localReport.ListRenderingExtensions())
            {
                if (formatsToDisable.Contains(extension.Name))
                {
                    ReflectivelySetVisibilityFalse(extension);
                }
            }
        }

        #endregion�Metodos�Publicos�Estaticos

        #region�Metodos�Privados�Estaticos

        /// <summary>
        /// Metodo auxiliar que esconde la extensi�n que se desea deshabilitar
        /// </summary>
        /// <param name="extension">Nombre de la extensi�n a ocultar.</param>
        /// <remarks>
        /// Autor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeCreacion: 28/02/2013
        /// UltimaModificacionPor: William V�squez R. - INTRGRUPO\wvasquez
        /// FechaDeUltimaModificacion: 28/02/2013
        /// EncargadoSoporte: William V�squez R. - INTRGRUPO\wvasquez
        /// Descripci�n:
        /// </remarks>
        private static void ReflectivelySetVisibilityFalse(RenderingExtension extension)
        {
            FieldInfo info = extension.GetType().GetField("m_isVisible", BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null)
            {
                info.SetValue(extension, false);
            }
        }

        #endregion�Metodos�Privados�Estaticos

        #endregion�Metodos
    }
}