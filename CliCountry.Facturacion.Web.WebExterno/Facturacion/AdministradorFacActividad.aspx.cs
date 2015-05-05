// --------------------------------
// <copyright file="AdministradorFacActividad.aspx.cs" company="InterGrupo S.A.">
//     COPYRIGHT(C) 2013, Intergrupo S.A
// </copyright>
// ---------------------------------

namespace CliCountry.Facturacion.Web.WebExterno.Facturacion
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;
    using System.Xml.Serialization;
    using CliCountry.Facturacion.Dominio.Entidades;
    using CliCountry.Facturacion.Web.WebExterno.Comun.Clases;
    using CliCountry.Facturacion.Web.WebExterno.Comun.Paginas;
    using CliCountry.Facturacion.Web.WebExterno.Properties;
    using CliCountry.Facturacion.Web.WebExterno.Utilidades;
    using CliCountry.SAHI.Comun.Utilidades;
    using CliCountry.SAHI.Dominio.Entidades;
    using CliCountry.SAHI.Dominio.Entidades.Facturacion;
    using CliCountry.SAHI.Dominio.Entidades.Facturacion.Ventas;
    using CliCountry.Facturacion.Negocio.Base;
    using CliCountry.Facturacion.Dominio.Excepciones;

    /// <summary>
    /// Clase CliCountry.Facturacion.Web.WebExterno.Facturacion.AdministradorFacActividad
    /// </summary>
    public partial class AdministradorFacActividad : WebPage
    {
        #region�Declaraciones�Locales

        #region�Constantes

        /// <summary>
        /// The ATENCIONCLIENTES
        /// </summary>
        private const string ATENCIONCLIENTES = "AtencionClientes";

        /// <summary>
        /// The CHECKACTIVO
        /// </summary>
        private const string CHECKACTIVO = "chkActivo";

        /// <summary>
        /// The CHECKFACTURAR
        /// </summary>
        private const string CHECKFACTURAR = "chkFacturar";

        /// <summary>
        /// The CHECKGENERAR
        /// </summary>
        private const string CHECKGENERAR = "chkGenerar";

        /// <summary>
        /// The LABELIDCONTRATO
        /// </summary>
        private const string LABELIDCONTRATO = "lblIdContrato";

        /// <summary>
        /// The LABELIDENTIDAD
        /// </summary>
        private const string LABELIDENTIDAD = "lblIdEntidad";

        /// <summary>
        /// The TXTOBSERVACION
        /// </summary>
        private const string TXTOBSERVACION = "txtObservacion";

        /// <summary>
        /// The VALIDACONCEPTO
        /// </summary>
        private const string VALIDACONCEPTO = "?cc=1";

        /// <summary>
        /// The VENTAS
        /// </summary>
        private const string VENTAS = "Ventas";

        /// <summary>
        /// The VINCULACIONES
        /// </summary>
        private const string VINCULACIONES = "Vinculaciones";

        #endregion�Constantes

        #endregion�Declaraciones�Locales

        #region�Propiedades

        #region�Propiedades�Protegidas

        /// <summary>
        /// Obtiene o establece lista venta
        /// </summary>
        protected List<Venta> Ventas
        {
            get
            {
                if (ViewState[VENTAS] == null)
                {
                    ViewState[VENTAS] = new List<Venta>();
                }

                return ViewState[VENTAS] as List<Venta>;
            }

            set
            {
                ViewState[VENTAS] = value;
            }
        }

        #endregion�Propiedades�Protegidas
        #region�Propiedades�Privadas

        /// <summary>
        /// Obtiene o establece la atenci�n relacionada con la factura.
        /// </summary>
        private AtencionCliente Atencion
        {
            get
            {
                if (ViewState[ATENCIONCLIENTES] == null)
                {
                    ViewState[ATENCIONCLIENTES] = new AtencionCliente();
                }

                return ViewState[ATENCIONCLIENTES] as AtencionCliente;
            }

            set
            {
                ViewState[ATENCIONCLIENTES] = value;
            }
        }

        /// <summary>
        /// Obtiene o establece vinculaciones
        /// </summary>
        private List<Vinculacion> Vinculaciones
        {
            get
            {
                if (ViewState[VINCULACIONES] == null)
                {
                    ViewState[VINCULACIONES] = new List<Vinculacion>();
                }

                return ViewState[VINCULACIONES] as List<Vinculacion>;
            }

            set
            {
                ViewState[VINCULACIONES] = value;
            }
        }

        #endregion�Propiedades�Privadas

        #endregion�Propiedades

        #region�Metodos

        #region�Metodos�Publicos�Estaticos

        /// <summary>
        /// Desbloquear Atencion.
        /// </summary>
        /// <param name="idatencion">The idatencion.</param>
        /// <param name="usuario">The usuario.</param>
        /// <returns>Retorna string.</returns>
        [WebMethod]
        public static string DesbloquearAtencion(int idatencion, string usuario)
        {
            if (idatencion != 0)
            {
                Negocio.Controlador.ControlFacturacion neg = new Negocio.Controlador.ControlFacturacion();
                var bloqueo = neg.ActualizarBloquearAtencion(idatencion, usuario);
                HttpContext.Current.Session["idAtencion"] = null;
            }

            return string.Empty;
        }

        /// <summary>
        /// M�todo para consulta de xml contratos plan.
        /// </summary>
        /// <param name="numeroPagina">Index of the page.</param>
        /// <param name="contrato">The contrato.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="entidad">The entidad.</param>
        /// <returns>Lista de contratos plan.</returns>
        [WebMethod]
        public static string ObtenerXMLContratoPlan(int numeroPagina, string contrato, string plan, string entidad)
        {
            Negocio.Controlador.ControlFacturacion controladorFacturacion = new Negocio.Controlador.ControlFacturacion();

            Paginacion<ContratoPlan> consultaContratoPlan = CrearObjetoConsultaContratoPlan(numeroPagina - 1, contrato, plan, entidad);
            Resultado<Paginacion<List<ContratoPlan>>> resultadoContratoPlan = controladorFacturacion.ConsultarContratoPlan(consultaContratoPlan);

            XmlSerializer serializadorXML = new XmlSerializer(typeof(List<ContratoPlan>));
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter writerXML = XmlWriter.Create(stringBuilder);

            serializadorXML.Serialize(writerXML, resultadoContratoPlan.Objeto.Item.ToList());

            string xml = "<NewDataSet>";

            for (int i = 0; i < resultadoContratoPlan.Objeto.Item.Count; i++)
            {
                xml += "<Data>";
                xml += "<NombreContrato>" + System.Web.HttpUtility.HtmlEncode(resultadoContratoPlan.Objeto.Item[i].Contrato.Nombre) + "</NombreContrato>";
                xml += "<NombreTercero>" + System.Web.HttpUtility.HtmlEncode(resultadoContratoPlan.Objeto.Item[i].Tercero.Nombre) + "</NombreTercero>";
                xml += "<IdPlan>" + resultadoContratoPlan.Objeto.Item[i].Plan.Id + "</IdPlan>";
                xml += "<NombrePlan>" + System.Web.HttpUtility.HtmlEncode(resultadoContratoPlan.Objeto.Item[i].Plan.Nombre) + "</NombrePlan>";
                xml += "</Data>";
            }

            xml += "<Pager>";
            xml += "<PageIndex>" + numeroPagina.ToString() + "</PageIndex>";
            xml += "<PageSize>" + resultadoContratoPlan.Objeto.LongitudPagina.ToString() + "</PageSize>";
            xml += "<RecordCount>" + resultadoContratoPlan.Objeto.TotalRegistros + "</RecordCount>";
            xml += "</Pager>";

            xml += "</NewDataSet>";
            return xml;
        }

        #endregion�Metodos�Publicos�Estaticos
        #region�Metodos�Publicos

        /// <summary>
        /// Consultar Atencion.
        /// </summary>
        /// <remarks>
        /// Autor: Jos� Alexander Murcia Salamanca - INTERGRUPO\jmurcias 
        /// FechaDeCreacion: 05/09/2014
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: (Descripci�n detallada del m�todo, procure especificar todo el m�todo aqui).
        /// </remarks>
        public void ConsultarAtencion()
        {
            ImgGuardarMovimiento.Enabled = false;

            if (!string.IsNullOrEmpty(txtAtencion.Text.Trim()) && Page.IsValid && CargarDatosCliente())
            {
                CargarVentasVinculaciones();
            }
            else if (string.IsNullOrEmpty(txtAtencion.Text.Trim()))
            {
                LimpiarDatosBasicos();
            }

            txtAtencion.Focus();
            ucBuscarClienteTercero.LimpiarCampos(true);
            ucBuscarClienteTercero.LimpiarCampos(false);
        }

        private void CargarVentasVinculaciones()
        {
            var identificadorAtencion = Convert.ToInt32(txtAtencion.Text);
            CargarVentas(identificadorAtencion, null);
            CargarVinculaciones(InfoVinculacion(identificadorAtencion, 1));

            if (Session["Atencion"] != null)
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "ReporteClose", "ecm.close();", true);
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "ReporteNoFacturable", "vrnf.close();", true);
            }
        }

        #endregion�Metodos�Publicos
        #region�Metodos�Protegidos

        /// <summary>
        /// Confirma el desbloqueo de una atenci�n.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Luis Fernando Quintero - INTERGRUPO\lquinterom 
        /// FechaDeCreacion: 08/05/2014
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: (Descripci�n detallada del metodo, procure especificar todo el metodo aqui).
        /// </remarks>
        protected void Btn_confirm_Click(object sender, EventArgs e)
        {
            if (Session["idAtencionDesbloquear"] != null)
            {
                string identificadorAtencionTmp = Session["idAtencionDesbloquear"].ToString();

                string validar = WebService.Facturacion.ValidarAtencionBloqueada(Convert.ToInt32(identificadorAtencionTmp));

                if (!string.IsNullOrEmpty(validar) && validar != Context.User.Identity.Name)
                {
                    var bloqueo = WebService.Facturacion.ActualizarBloquearAtencion(Convert.ToInt32(identificadorAtencionTmp), Context.User.Identity.Name);

                    if (!bloqueo.Ejecuto)
                    {
                        MostrarMensaje("No se pudo desbloquear atenci�n anterior. Consulte administrador para desbloquear.", TipoMensaje.Error);
                    }
                    else
                    {
                        ConsultarAtencion();
                    }
                }
            }
        }

        /// <summary>
        /// Cierra el popup.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 05/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void BtnCerrarCorte_Click(object sender, EventArgs e)
        {
            var listaCorte = ucCortesFacturacion.ObtenerListaCorteActivo();

            if (listaCorte.Count() > 0)
            {
                var ventasFacturar = from
                                        venta in Ventas
                                     from
                                        item in listaCorte
                                     where
                                        venta.FechaVenta >= item.FechaInicial &&
                                        venta.FechaVenta <= item.FechaFinal &&
                                        item.Activo
                                     select
                                        new Venta()
                                        {
                                            Facturar = venta.Facturar,
                                            NombreTransaccion = venta.NombreTransaccion,
                                            NumeroVenta = venta.NumeroVenta,
                                            FechaVenta = venta.FechaVenta,
                                            NombreUbicacionConsumo = venta.NombreUbicacionConsumo,
                                            NombreUbicacionEntrega = venta.NombreUbicacionEntrega,
                                            IdAtencion = venta.IdAtencion
                                        };

                CargaObjetos.OrdenamientoGrilla(grvVentas, ventasFacturar);
            }
            else
            {
                grvVentas.DataSource = Ventas;
                grvVentas.DataBind();
            }

            mpeCortes.Hide();
        }

        /// <summary>
        /// Metodo de Cerrar Popup Exclusiones.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 08/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void BtnCerrarPopupExclusiones_Click(object sender, EventArgs e)
        {
            List<NoMarcada> exclusionesNoMarcadas = new List<NoMarcada>();
            exclusionesNoMarcadas = ucExclusiones.ObtenerExclusionesDesmarcadas();
            VinculacionSeleccionada.NoMarcadas = exclusionesNoMarcadas;
            ObtenerSeleccionada().NoMarcadas = exclusionesNoMarcadas;
        }

        /// <summary>
        /// Metodo de Definir Condiciones de Operacion Ejecutada.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tipoOperacion">The tipo operacion.</param>
        /// Autor: Alex David Mattos R. - INTERGRUPO\amattos
        /// <remarks>
        /// Autor: (Nombre del Autor y Usuario del dominio)
        /// FechaDeCreacion: (dd/MM/yyyy)
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void DefinirCondiciones_OperacionEjecutada(object sender, Global.TipoOperacion tipoOperacion)
        {
            mltvFacAct.SetActiveView(vFacAct);
        }

        /// <summary>
        /// Obtiene las exclusiones configuradas y marcadas.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tipoOperacion">The tipo operacion.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 16/01/2014
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void Exclusiones_OperacionEjecutada(object sender, Global.TipoOperacion tipoOperacion)
        {
            List<NoMarcada> exclusionesNoMarcadas = new List<NoMarcada>();
            exclusionesNoMarcadas = VinculacionSeleccionada.NoMarcadas;
            ObtenerSeleccionada().NoMarcadas = exclusionesNoMarcadas;
        }

        /// <summary>
        /// Carga el Popup de exclusiones de contratos y manuales con la grilla segun entidad y contrato.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GridViewCommandEventArgs" /> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 08/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void GrvEntidades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int indiceFila = Convert.ToInt32(e.CommandArgument);
            var identificadorEntidad = (Label)grvEntidades.Rows[indiceFila].FindControl("lblIdEntidad");

            switch (e.CommandName)
            {
                case CliCountry.SAHI.Comun.Utilidades.Global.SELECCIONAR:
                    VinculacionSeleccionada = AdministrarVinculacion.BuscarVinculacion(Convert.ToInt32(identificadorEntidad.Text), indiceFila + 1, Vinculaciones);
                    ActivarRegistroGridViewVinculaciones(indiceFila);
                    break;
            }
        }

        /// <summary>
        /// Actualiza la informaci�n de toda la grilla de vinculaciones.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 11/06/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgActualizarVinculaciones_Click(object sender, ImageClickEventArgs e)
        {
            Resultado<int> resultado = new Resultado<int>();

            foreach (GridViewRow registro in grvEntidades.Rows)
            {
                var activo = (CheckBox)registro.FindControl("chkActivo");
                var valorOrden = (TextBox)registro.FindControl("txtOrden");
                var identificadorTipoAfiliacion = (Label)registro.FindControl("lblIdTipoAfiliacion");
                var montoEjecutado = (Label)registro.FindControl("lblMontoEjecutado");
                var identificadorContrato = (Label)registro.FindControl("lblIdContrato");
                var identificadorPlan = Convert.ToInt32(registro.Cells[7].Text);
                var observacion = (TextBox)registro.FindControl("txtObservacion");

                var vinculacion = new Vinculacion()
                {
                    CodigoEntidad = Settings.Default.General_CodigoEntidad,
                    IdAtencion = Convert.ToInt32(txtAtencion.Text),
                    Contrato = new Contrato()
                    {
                        Id = Convert.ToInt32(identificadorContrato.Text)
                    },
                    Plan = new Plan()
                    {
                        Id = identificadorPlan
                    },
                    IdTipoAfiliacion = Convert.ToInt32(identificadorTipoAfiliacion.Text),
                    IdEstado = Convert.ToInt32(0),
                    NumeroAfiliacion = txtNoDocumento.Text,
                    IndHabilitado = activo.Checked ? Convert.ToInt16(1) : Convert.ToInt16(0),
                    Orden = Convert.ToInt32(valorOrden.Text),
                    MontoEjecutado = Convert.ToDecimal(montoEjecutado.Text),
                    Observacion = observacion.Text
                };

                resultado = WebService.Facturacion.ActualizarVinculacion(vinculacion);
            }

            if (resultado.Ejecuto && string.IsNullOrEmpty(resultado.Mensaje))
            {
                ucBuscarClienteTercero.LimpiarCampos(true);
                ucBuscarClienteTercero.LimpiarCampos(false);
                ImgConsultaAtencion_Click(sender, e);
                MostrarMensaje(string.Format(Resources.VincularEntidad.VinculaEntidad_MsjActualizacion, txtAtencion.Text), TipoMensaje.Ok);
            }
            else
            {
                MostrarMensaje(string.Format(Resources.VincularEntidad.VincularEntidad_MsjError, txtAtencion.Text), TipoMensaje.Error);
            }
        }

        /// <summary>
        /// Cargar la vinculacion Entidad.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: (Nombre del Autor y Usuario del dominio)
        /// FechaDeCreacion: (dd/MM/yyyy)
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgbtnVincularEntidad_Click(object sender, ImageClickEventArgs e)
        {
            ucVincularEntidad.lblIdAtencion.Text = Convert.ToString(txtAtencion.Text);
            ucVincularEntidad.LimpiarCampos();
            ucVincularEntidad.lblTitulo.Text = Resources.VincularEntidad.VincularEntidad_Titulo;
            ucVincularEntidad.LblMensaje.Text = string.Empty;
            ucVincularEntidad.CargarCombos();
            ucVincularEntidad.txtNumeroAfiliacion.Text = txtNoDocumento.Text;
            ucVincularEntidad.txtOrden.Text = grvEntidades.Rows.Count > 0 ? Convert.ToString(grvEntidades.Rows.Count + 1) : Convert.ToString(1);
            mpeVincularentidad.Show();
        }

        /// <summary>
        /// Se ejecuta cuando se hace click en el evento.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 02/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgConsultaAtencion_Click(object sender, ImageClickEventArgs e)
        {
            ConsultarAtencion();
        }

        /// <summary>
        /// Permite cargar el popup de cortes facturaci�n
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 05/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgCortesFacturacion_Click(object sender, ImageClickEventArgs e)
        {
            mpeCortes.Show();

            TextBox txtFechaFinal = (TextBox)ucCortesFacturacion.FindControl("txtFechaInicial");
            txtFechaFinal.Text = Session["fechaMinimaVenta"].ToString();
            ucCortesFacturacion.FechaMinima = Convert.ToDateTime(txtFechaFinal.Text);
            TextBox txtFechaInicial = (TextBox)ucCortesFacturacion.FindControl("txtFechaFinal");
            txtFechaInicial.Text = DateTime.Now.ToShortDateString();

            var gridVentas = grvVentas.Rows.Cast<GridViewRow>();

            foreach (GridViewRow filaGrid in gridVentas)
            {
                foreach (Venta filaVenta in Ventas)
                {
                    if (filaVenta.NumeroVenta == Convert.ToInt32(filaGrid.Cells[2].Text))
                    {
                        filaVenta.Facturar = (filaGrid.FindControl(CHECKFACTURAR) as CheckBox).Checked;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Metodo de Definir condiciones.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: (Nombre del Autor y Usuario del dominio)
        /// FechaDeCreacion: (dd/MM/yyyy)
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgDefinirCondiciones_Click(object sender, ImageClickEventArgs e)
        {
            mltvFacAct.SetActiveView(vDefinirCondiciones);
            ucDefinirCondiciones.CargarDatosPaginaDefinirCondiciones(TipoFacturacion.FacturacionActividad);
        }

        /// <summary>
        /// Metodo para cargar el modal de exclusiones.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: David Mauricio Guti�rrez Ruiz - INTERGRUPO\dgutierrez
        /// FechaDeCreacion: 12/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgDefinirExclusiones_Click(object sender, ImageClickEventArgs e)
        {
            var exclusion = new ExclusionFacturaActividades()
            {
                CodigoEntidad = Settings.Default.General_CodigoEntidad,
                IdTercero = VinculacionSeleccionada.Tercero.Id,
                IdContrato = VinculacionSeleccionada.Contrato.Id,
                IdPlan = VinculacionSeleccionada.Plan.Id
            };

            ucExclusiones.ExclusionesNoMarcadas = VinculacionSeleccionada.NoMarcadas;
            ucExclusiones.CargarGrillaExclusiones(exclusion);
            mpeExclusiones.Show();
        }

        /// <summary>
        /// Metodo para generar el estado de Cuenta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        protected void ImgGenerarEstadoCuenta_Click(object sender, ImageClickEventArgs e)
        {
            this.CerrarVentanas();
            this.GenerarEstadoCuenta();
        }

        /// <summary>
        /// Cierra las ventanas que se abrieron en otros procesos de generaci�n de estado de cuenta.
        /// </summary>
        private void CerrarVentanas()
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "ReporteCloseEstado", "if(typeof ecm != 'undefined'){ecm.close();}", true);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "ReporteNoFacturable", "if(typeof vrnf != 'undefined'){vrnf.close();}", true);
        }

        /// <summary>
        /// Evento Para realizar el almacenamiento de los movimientos.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: David Mauricio Guti�rrez Ruiz - INTERGRUPO\dgutierrez
        /// FechaDeCreacion: 25/06/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgGuardarMovimiento_Click(object sender, ImageClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAtencion.Text))
            {
                var atencionCliente = new AtencionCliente()
                {
                    IdAtencion = Convert.ToInt32(txtAtencion.Text),
                    IndMovimiento = ChkMovimiento.Checked
                };

                var resultado = WebService.Facturacion.ActualizarAtencion(atencionCliente);

                if (resultado.Ejecuto)
                {
                    if (resultado.Objeto)
                    {
                        MostrarMensaje(Resources.AdministradorFacActividades.FacActividad_MsjActualizoMovimiento, TipoMensaje.Ok);
                    }
                    else
                    {
                        MostrarMensaje(Resources.AdministradorFacActividades.FacActividad_MsjNoActualizoMovimiento, TipoMensaje.Error);
                    }
                }
                else
                {
                    MostrarMensaje(resultado.Mensaje, TipoMensaje.Error);
                }
            }
        }

        /// <summary>
        /// Metodo para vincular ventas.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageClickEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: David Mauricio Guti�rrez Ruiz - INTERGRUPO\dgutierrez
        /// FechaDeCreacion: 12/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void ImgVincularVentas_Click(object sender, ImageClickEventArgs e)
        {
            ucVinculacionVenta.LimpiarCampos();
            ucVinculacionVenta.AtencionSeleccionada = new Atencion()
            {
                IdAtencion = VinculacionSeleccionada.IdAtencion
            };
            mpeVentas.Show();
        }

        /// <summary>
        /// Metodo de Evento de Inicializacion.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: David Mauricio Guti�rrez Ruiz - INTERGRUPO\dgutierrez
        /// FechaDeCreacion: 23/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected override void OnInit(EventArgs e)
        {
            ucVinculacionVenta.OperacionEjecutada += VinculacionVenta_OperacionEjecutada;
            ucDefinirCondiciones.OperacionEjecutada += DefinirCondiciones_OperacionEjecutada;
            ucVincularEntidad.OperacionEjecutada += VincularEntidad_OperacionEjecutada;
            ucExclusiones.OperacionEjecutada += Exclusiones_OperacionEjecutada;
            base.OnInit(e);
        }

        /// <summary>
        /// Se ejecuta cuando se carga la p�gina.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        protected void Page_Load(object sender, EventArgs e)
        {
            ucBuscarClienteTercero.Condicion = true;

            if (!Page.IsPostBack)
            {
                CargarListas();
            }
        }

        #endregion�Metodos�Protegidos
        #region�Metodos�Privados�Estaticos

        /// <summary>
        /// Metodo para Crear Objeto de Consulta.
        /// </summary>
        /// <param name="numeroPagina">The numero pagina.</param>
        /// <param name="contrato">The contrato.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="entidad">The entidad.</param>
        /// <returns>
        /// Retorna Tercero.
        /// </returns>
        private static Paginacion<ContratoPlan> CrearObjetoConsultaContratoPlan(int numeroPagina, string contrato, string plan, string entidad)
        {
            Paginacion<ContratoPlan> consulta = new Paginacion<ContratoPlan>()
            {
                PaginaActual = numeroPagina,
                LongitudPagina = Properties.Settings.Default.Paginacion_CantidadRegistrosPagina,
                Item = new ContratoPlan()
                {
                    Contrato = new Contrato()
                    {
                        Nombre = contrato
                    },
                    IndHabilitado = true,
                    Plan = new Plan()
                    {
                        Nombre = plan
                    },
                    Tercero = new Tercero()
                    {
                        Nombre = entidad
                    }
                }
            };

            return consulta;
        }

        #endregion�Metodos�Privados�Estaticos
        #region�Metodos�Privados

        /// <summary>
        /// M�todo para activar un registro de la grilla de Vinculaciones.
        /// </summary>
        /// <param name="indiceFila">The indice fila.</param>
        private void ActivarRegistroGridViewVinculaciones(int indiceFila)
        {
            foreach (GridViewRow fila in grvEntidades.Rows)
            {
                if (fila.FindControl("imbSelect") != null)
                {
                    if (fila.RowIndex == indiceFila)
                    {
                        ImageButton btnselect = (ImageButton)fila.FindControl("imbSelect");
                        btnselect.ImageUrl = "~/App_Themes/SAHI/images/pagSiguiente.png";
                    }
                    else
                    {
                        ImageButton btnselect = (ImageButton)fila.FindControl("imbSelect");
                        btnselect.ImageUrl = "~/App_Themes/SAHI/images/seleccionar.png";
                    }
                }
            }
        }

        /// <summary>
        /// M�todo para adicionar el item por defecto.
        /// </summary>
        /// <param name="combo">The combo.</param>
        /// <param name="valorSeleccionado">if set to <c>true</c> [valor seleccionado].</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void AdicionarItemPorDefecto(DropDownList combo, bool valorSeleccionado)
        {
            combo.Items.Insert(0, new System.Web.UI.WebControls.ListItem(Resources.GlobalWeb.General_ComboItemTexto, Resources.GlobalWeb.General_ComboItemValor));

            if (!valorSeleccionado)
            {
                combo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Metodo para cargar conceptos asociados a una factura.
        /// </summary>
        /// <remarks>
        /// Autor: Jorge Cortes - INTERGRUPO\jcortesm
        /// FechaDeCreacion: 25/06/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarConceptosFactura()
        {
            var resultadoDepositos = WebService.Facturacion.ConsultarDepositos(
                new Atencion()
                {
                    IdAtencion = Atencion.IdAtencion
                });

            if (resultadoDepositos.Ejecuto)
            {
                Atencion.Deposito = resultadoDepositos.Objeto.FirstOrDefault();

                if (Atencion.Deposito == null)
                {
                    Atencion.Deposito = new Deposito()
                    {
                        TotalDeposito = 0
                    };
                }

                var resultadoConceptos = WebService.Facturacion.ConsultarConceptos(
                new Atencion()
                {
                    IdAtencion = Atencion.IdAtencion
                });

                if (resultadoConceptos.Ejecuto)
                {
                    if (Atencion.Deposito != null)
                    {
                        Atencion.Deposito.Concepto = resultadoConceptos.Objeto.ToList();
                    }
                }
                else
                {
                    MostrarMensaje(resultadoConceptos.Mensaje, TipoMensaje.Error);
                }
            }
            else
            {
                MostrarMensaje(resultadoDepositos.Mensaje, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// M�todo para mostrar datos en los campos.
        /// </summary>
        /// <returns>Indica Si Registros Datos de Cliente</returns>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 02/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private bool CargarDatosCliente()
        {
            bool cargarInformacion = false;

            Atencion = WebService.Facturacion.ConsultarAtencionCliente(Convert.ToInt32(txtAtencion.Text));

            if (Atencion != null)
            {
                if (Atencion.VentasPendientes == true)
                {
                    txtPaciente.Text = string.Concat(Atencion.NombreCliente, " ", Atencion.ApellidoCliente);
                    txtNoDocumento.Text = Atencion.NumeroDocumento;
                    txtTipoAtencion.Text = Atencion.NombreAtencionTipo;
                    ChkMovimiento.Checked = Atencion.IndMovimiento;
                    ChkMovimiento.Enabled = true;
                    ImgGuardarMovimiento.Enabled = true;
                    cargarInformacion = true;
                    CargarConceptosFactura();
                }
                else
                {
                    LimpiarDatosBasicos();
                    cargarInformacion = false;
                    MostrarMensaje(Resources.GlobalWeb.General_SinVentas, TipoMensaje.Error);
                }
            }
            else
            {
                LimpiarDatosBasicos();
                cargarInformacion = false;
                MostrarMensaje(Resources.GlobalWeb.General_GrillaSinDatos, TipoMensaje.Error);
            }

            return cargarInformacion;
        }

        /// <summary>
        /// Carga el combo de los formatos.
        /// </summary>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano
        /// FechaDeCreacion: (30/01/2013)
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Carga el combo de los formatos.
        /// </remarks>
        private void CargarListaFormatos()
        {
            var resultado = WebService.Integracion.ConsultarFormatos();

            if (resultado.Ejecuto)
            {
                DdlFormato.DataSource = resultado.Objeto;
                DdlFormato.DataValueField = Resources.GlobalWeb.General_ComboPropiedadValor;
                DdlFormato.DataTextField = Resources.GlobalWeb.General_ComboPropiedadTexto;
                DdlFormato.DataBind();
                DdlFormato.SelectedIndex = 0;
            }
            else
            {
                MostrarMensaje(resultado.Mensaje, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// M�todo de cargue inicial de los listados.
        /// </summary>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarListas()
        {
            CargarListaTiposMovimiento();
            CargarListaSecciones();
            CargarListaFormatos();
        }

        /// <summary>
        /// Carga el combo de las secciones.
        /// </summary>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarListaSecciones()
        {
            var seccion = new Seccion()
            {
                CodigoEntidad = Settings.Default.General_CodigoEntidad,
                CodEsor = Settings.Default.Seccion_CodigoEstadoOrgan,
                CodSede = Settings.Default.Seccion_CodigoSede
            };

            var resultado = WebService.Integracion.ConsultarSecciones(seccion);

            if (resultado.Ejecuto)
            {
                ddlSeccion.DataSource = resultado.Objeto;
                ddlSeccion.DataValueField = Resources.GlobalWeb.General_ComboPropiedadCodEsor;
                ddlSeccion.DataTextField = Resources.GlobalWeb.General_ComboPropiedadTexto;
                ddlSeccion.DataBind();
                AdicionarItemPorDefecto(ddlSeccion, false);
                PreSeleccionarSeccion();
            }
            else
            {
                MostrarMensaje(resultado.Mensaje, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// Carga el combo de los tipos de movimiento.
        /// </summary>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarListaTiposMovimiento()
        {
            var tipoMovimiento = new TipoMovimiento()
            {
                TimModIde = Settings.Default.TipoMovimiento_IdentificadorMovimientoNumerado,
                Codigo = Settings.Default.General_CodigoEntidad,
                PanCod = Settings.Default.TipoMovimiento_CodigoPantalla,
                TimIndAct = Settings.Default.TipoMovimiento_IndicadorPantallaActivo
            };

            var resultado = WebService.Integracion.ConsultarTiposMovimiento(tipoMovimiento);

            if (resultado.Ejecuto)
            {
                ddlTipoMovimiento.DataSource = resultado.Objeto;
                ddlTipoMovimiento.DataValueField = Resources.GlobalWeb.General_ComboPropiedadValor;
                ddlTipoMovimiento.DataTextField = Resources.GlobalWeb.General_ComboPropiedadTexto;
                ddlTipoMovimiento.DataBind();
                AdicionarItemPorDefecto(ddlTipoMovimiento, false);
                PreSeleccionarTipoMovimiento();
            }
            else
            {
                MostrarMensaje(resultado.Mensaje, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// M�todo para cargar paginador.
        /// </summary>
        /// <param name="resultado">The resultado.</param>
        /// <remarks>
        /// Autor: Silvia Lorena L�pez Camacho - INTERGRUPO\slopez
        /// FechaDeCreacion: 01/10/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarPaginador(Paginacion<ObservableCollection<Venta>> resultado)
        {
            pagControl.ObjetoPaginador = new Paginador()
            {
                CantidadPaginas = resultado.CantidadPaginas,
                LongitudPagina = resultado.LongitudPagina,
                MaximoPaginas = Properties.Settings.Default.Paginacion_CantidadBotones,
                PaginaActual = resultado.PaginaActual,
                TotalRegistros = resultado.TotalRegistros
            };
        }

        /// <summary>
        /// M�todo para el llenado de la grilla de ventas.
        /// </summary>
        /// <param name="identificadorAtencion">The id atencion.</param>
        /// <param name="ventasMarcadas">The ventas marcadas.</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 03/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarVentas(int identificadorAtencion, List<int> ventasMarcadas)
        {
            var parametrosConsulta = ConsultarParametros(0, identificadorAtencion);
            parametrosConsulta.LongitudPagina = 0;
            var respuesta = WebService.Facturacion.ConsultarVentasAtencion(parametrosConsulta);
            bool cancelMsg = false;

            if (!respuesta.Ejecuto)
            {
                MostrarMensaje(respuesta.Mensaje, TipoMensaje.Error);
                return;
            }

            Ventas = respuesta.Objeto.Item;

            if (ventasMarcadas != null)
            {
                foreach (var item in Ventas)
                {
                    var resultado = ventasMarcadas.Where(sp => sp == item.NumeroVenta);
                    item.Facturar = resultado.Count() > 0 ? true : false;
                }
            }

            bool consultar = false;
            string idatencion = (Session["idAtencion"] != null) ? Session["idAtencion"].ToString().Trim() : string.Empty;

            if (!string.IsNullOrEmpty(idatencion) && idatencion != txtAtencion.Text.Trim())
            {
                string identificadorAtencionTmp = Session["idAtencion"].ToString();
                var bloqueo = WebService.Facturacion.ActualizarBloquearAtencion(VinculacionSeleccionada.IdAtencion,
                    Context.User.Identity.Name);

                if (!bloqueo.Ejecuto)
                {
                    MostrarMensaje("No se pudo desbloquear atenci�n anterior. Consulte administrador para desbloquear.", TipoMensaje.Error);
                }
            }

            if (Ventas == null)
            {
                LimpiarDatosBasicos();
                MostrarMensaje(Resources.GlobalWeb.General_SinVentas, TipoMensaje.Error);
                return;
            }

            string validar;
            bool autorizado;
            consultar = AdministrarBloqueoAtencion(identificadorAtencion, consultar, out validar, out autorizado);

            cancelMsg = CargarDatosBloqueoAtencion(identificadorAtencion, cancelMsg, consultar, validar, autorizado);
        }

        private bool CargarDatosBloqueoAtencion(int identificadorAtencion, bool cancelMsg, bool consultar, string validar, bool autorizado)
        {
            if (autorizado)
            {
                cancelMsg = true;
                Session["idAtencionDesbloquear"] = identificadorAtencion;
                lblConfirmacion.Text = "El n�mero de atenci�n: " + identificadorAtencion +
                " esta bloqueada por: " + validar + ".";
                mpeConfirmar.Show();
            }

            if (consultar)
            {
                CargaObjetos.OrdenamientoGrilla(this.Page, grvVentas, Ventas);

                // CargarPaginador(respuesta.Objeto);
                Session["fechaMinimaVenta"] = Ventas.Min(p => p.FechaVenta).ToShortDateString();
                ucCortesFacturacion.FechaMinima = Convert.ToDateTime(Ventas.Min(p => p.FechaVenta).ToShortDateString());
                ucCortesFacturacion.LimpiarListaCorteFacturacion();
                divTabsMarco.Visible = true;
            }
            else if (!cancelMsg)
            {
                MostrarMensaje("El n�mero de atenci�n: " + identificadorAtencion + " esta bloqueada por: " + validar, TipoMensaje.Error);
                LimpiarDatosBasicos();
            }
            return cancelMsg;
        }

        private bool AdministrarBloqueoAtencion(int identificadorAtencion, bool consultar, out string validar, out bool autorizado)
        {
            validar = WebService.Facturacion.ValidarAtencionBloqueada(identificadorAtencion);
            autorizado = false;

            if (string.IsNullOrEmpty(validar))
            {
                consultar = ValidarBloqueoAtencion(identificadorAtencion, consultar);
            }
            else if (!string.IsNullOrEmpty(validar) && validar != Context.User.Identity.Name)
            {
                autorizado = ValidarRolUsuarioDesbloquearAtencion(ref consultar);
            }
            else if (!string.IsNullOrEmpty(validar) && validar == Context.User.Identity.Name)
            {
                consultar = true;
                Session["idAtencion"] = identificadorAtencion;
            }
            return consultar;
        }

        private bool ValidarRolUsuarioDesbloquearAtencion(ref bool consultar)
        {
            consultar = false;

            string[] roles = Settings.Default.UsuarioRoles_DesbloquearAtencion.ToString().Split(',');

            string usuario = Context.User.Identity.Name;

            bool autorizado = false;

            foreach (string rol in roles)
            {
                if (Convert.ToBoolean(WebService.Facturacion.ValidarRolUsuario(usuario, rol).Objeto))
                {
                    autorizado = true;
                    break;
                }
            }
            return autorizado;
        }

        private bool ValidarBloqueoAtencion(int identificadorAtencion, bool consultar)
        {
            var bloqueo = WebService.Facturacion.ActualizarBloquearAtencion(identificadorAtencion, Context.User.Identity.Name);

            if (bloqueo.Ejecuto && bloqueo.Objeto != 0)
            {
                Session["idAtencion"] = identificadorAtencion;
                consultar = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(bloqueo.Mensaje))
                {
                    MostrarMensaje(Resources.GlobalWeb.FacturacionActividades_ErrorAtencionBloqueada, TipoMensaje.Error);
                }
                else
                {
                    MostrarMensaje(bloqueo.Mensaje, TipoMensaje.Error);
                }
            }
            return consultar;
        }

        /// <summary>
        /// Metodo que llena la Grilla de Vinculacion.
        /// </summary>
        /// <param name="atencion">The atencion.</param>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void CargarVinculaciones(Vinculacion atencion)
        {
            var resultado = WebService.Facturacion.ConsultarVinculaciones(atencion);

            if (resultado.Ejecuto)
            {
                Vinculaciones = resultado.Objeto.ToList();

                if (Vinculaciones != null)
                {
                    CargaObjetos.OrdenamientoGrilla(grvEntidades, Vinculaciones);

                    if (Vinculaciones.Count > 0)
                    {
                        GrvEntidades_RowCommand(this, new GridViewCommandEventArgs(grvEntidades.Rows[0], new CommandEventArgs(CliCountry.SAHI.Comun.Utilidades.Global.SELECCIONAR, 0)));
                        grvEntidades.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                MostrarMensaje(resultado.Mensaje, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// Devuelve encabezado para la factura.
        /// </summary>
        /// <returns>
        /// Retorna Encabezado.
        /// </returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 14/05/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private ProcesoFactura ConstruirEncabezadoProcesoFactura()
        {
            var procesoFactura = new ProcesoFactura()
            {
                CodigoEntidad = Settings.Default.General_CodigoEntidad,
                CodigoSeccion = ddlSeccion.SelectedValue,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                IdContrato = Convert.ToInt32(VinculacionSeleccionada.Contrato.Id),
                IdEstado = Settings.Default.ProcesoFactura_EstadoPendiente,
                IdFormato = Convert.ToByte(DdlFormato.SelectedValue),
                IdUsuarioFirma = Settings.Default.UsuarioFirma_Facturacion,
                IdTercero = Convert.ToInt32(VinculacionSeleccionada.Tercero.Id),
                IdTipoMovimiento = Convert.ToInt32(ddlTipoMovimiento.SelectedValue),
                IndHabilitado = Convert.ToByte(ValorDefecto.IndHabilidado),
                TipoRelacion = Settings.Default.FacturaActividades_TipoRelacion,
                TipoFactura = TipoFacturacion.FacturacionActividad,
                VentasNoMarcadas = ObtenerVentasNoMarcadas()
            };

            return procesoFactura;
        }

        /// <summary>
        /// Obtiene la informaci�n de los campos filtro.
        /// </summary>
        /// <param name="numeroPagina">The numero pagina.</param>
        /// <param name="identificadorAtencion">The id atencion.</param>
        /// <returns>
        /// Resultado operacion.
        /// </returns>
        /// <remarks>
        /// Autor: Silvia Lorena L�pez Camacho - INTERGRUPO\slopez
        /// FechaDeCreacion: 01/10/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private Paginacion<Venta> ConsultarParametros(int numeroPagina, int identificadorAtencion)
        {
            Paginacion<Venta> consulta = new Paginacion<Venta>()
            {
                PaginaActual = numeroPagina,
                LongitudPagina = Properties.Settings.Default.Paginacion_CantidadRegistrosPagina,

                Item = new Venta()
                {
                    IdAtencion = identificadorAtencion
                }
            };

            return consulta;
        }

        /// <summary>
        /// Retorna lista de proceso de factura detalle.
        /// </summary>
        /// <param name="detalles">The detalles.</param>
        /// <returns>Lista de datos.</returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 14/05/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private List<ProcesoFacturaDetalle> ExtraerInformacionDetalle(List<ProcesoFacturaDetalle> detalles)
        {
            var items = from
                            detalle in detalles
                        select new ProcesoFacturaDetalle()
                        {
                            Cruzar = true,
                            IdAtencion = Convert.ToInt32(txtAtencion.Text)
                        };

            return items.ToList();
        }

        /// <summary>
        /// Genera el estado de cuenta.
        /// </summary>
        private void GenerarEstadoCuenta()
        {
            try
            {
                    if (Page.IsValid)
                    {
                        int indiceFilaGridViewVinculaciones = 0;
                        var identificadorEntidad = (Label)grvEntidades.Rows[indiceFilaGridViewVinculaciones].FindControl("lblIdEntidad");  
              
                        VinculacionSeleccionada = AdministrarVinculacion.BuscarVinculacion(Convert.ToInt32(identificadorEntidad.Text), indiceFilaGridViewVinculaciones + 1, Vinculaciones);
                
                        grvEntidades.SelectedIndex = 0;
                        ActivarRegistroGridViewVinculaciones(indiceFilaGridViewVinculaciones);

                        List<int> ventasSeleccionadas = ObtenerVentasMarcadas();

                        if (ventasSeleccionadas.Count() > 0)
                        {
                            Session["ProcesoFactura"] = ConstruirProcesoFactura(ventasSeleccionadas);
                            ActivarConceptosCobro();
                    
                            try
                            {
                                AdministrarConcepto.AplicarConceptosAtencion(Atencion);
                        
                                Session["Atencion"] = Atencion;
                                Session.Add(Resources.GlobalWeb.Session_TipoFacturacion, TipoFacturacion.FacturacionActividad);

                                AbrirVentana(this, this.GetType(), Resources.AdministradorFacActividades.FacActividad_ReporteEstadoCuenta, "ecm");                      
                            }
                            catch (InvalidOperationException error)
                            {
                                MostrarMensaje(error.Message, TipoMensaje.Error);
                            }
                        }
                        else
                        {
                            MostrarMensaje(Resources.GlobalWeb.General_DebeSeleccionarVentas, TipoMensaje.Error);
                        }
                    }
                    else
                    {
                        MostrarMensaje(null, TipoMensaje.Error);
                    }
            }
            catch(ArgumentNullException excepcion)
            {
                MostrarMensaje(excepcion.Message, TipoMensaje.Error);
            }
        }

        /// <summary>
        /// Construye el proceso de la factura.
        /// </summary>
        /// <param name="ventasSeleccionadas">Lista de identificadores de ventas seleccionadas.</param>
        /// <returns>Retorna el objeto generado para proceso factura.</returns>
        private ProcesoFactura ConstruirProcesoFactura(List<int> ventasSeleccionadas)
        {
            ProcesoFactura procesoFactura = ConstruirEncabezadoProcesoFactura();
            procesoFactura.Detalles = new List<ProcesoFacturaDetalle>();
            procesoFactura.Detalles.Add(new ProcesoFacturaDetalle()
            {
                IdAtencion = VinculacionSeleccionada.IdAtencion,
                IdPlan = VinculacionSeleccionada.Plan.Id,
                Cruzar = true
            });

            procesoFactura.ExclusionesNoMarcadas = VinculacionSeleccionada.NoMarcadas;
            procesoFactura.Responsable = ObtenerDatosResponsableIngresado();
            procesoFactura.Vinculaciones = ObtenerVinculacionesSeleccionadas();
            procesoFactura.VentasSeleccionadas = ventasSeleccionadas;

            return procesoFactura;
        }

        /// <summary>
        /// Obtiene la informaci�n de parametros del objeto de Vinculaci�n.
        /// </summary>
        /// <param name="identificadorAtencion">The id atencion.</param>
        /// <param name="indActivo">The ind act.</param>
        /// <returns>
        /// Objeto Vinculacion.
        /// </returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private Vinculacion InfoVinculacion(int identificadorAtencion, short indActivo)
        {
            var vinculacion = new Vinculacion()
            {
                IdAtencion = identificadorAtencion,
                IndHabilitado = indActivo
            };

            return vinculacion;
        }

        /// <summary>
        /// Limpiar los campos de datos basicos.
        /// </summary>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 03/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void LimpiarDatosBasicos()
        {
            Atencion = null;
            Ventas = null;
            Vinculaciones = null;
            VinculacionSeleccionada = null;
            txtPaciente.Text = string.Empty;
            txtNoDocumento.Text = string.Empty;
            txtTipoAtencion.Text = string.Empty;
            ChkMovimiento.Checked = false;
            ChkMovimiento.Enabled = false;
            ImgGuardarMovimiento.Enabled = false;
            divTabsMarco.Visible = false;
        }

        /// <summary>
        /// Metodo para activar los conceptos asociados a un deposito.
        /// </summary>
        private void ActivarConceptosCobro()
        {
            if (Atencion.Deposito != null)
            {
                foreach (ConceptoCobro concepto in Atencion.Deposito.Concepto)
                {
                    var listaConceptos = (from fila in grvEntidades.Rows.Cast<GridViewRow>()
                                          where (fila.FindControl(CHECKACTIVO) as CheckBox).Checked
                                          && (fila.FindControl(CHECKGENERAR) as CheckBox).Checked
                                          && concepto.IdContrato == Convert.ToInt32((fila.FindControl(LABELIDCONTRATO) as Label).Text)
                                          select concepto).ToList();

                    concepto.Activo = listaConceptos.Count() > 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// Obtiene la informaci�n del tercero responsable para el estado de cuenta.
        /// </summary>
        /// <returns>Retorna Responsable.</returns>
        private Responsable ObtenerDatosResponsableIngresado()
        {
            var responsable = new Responsable()
            {
                CodigoEntidad = Settings.Default.General_CodigoEntidad,
                Consecutivo = Settings.Default.General_CodigoEntidad,
                NumeroFactura = 0,
                IdTipoMovimiento = 0,
                CodigoMovimientoSecuencial = string.Empty,
                IdVenta = 0,
                NumeroVenta = 0,
                IdProducto = 0,
                LoteDetalleVenta = 0,
                Componente = Resources.GlobalWeb.General_ValorNA,
                IdCliente = string.IsNullOrEmpty(ucBuscarClienteTercero.TxtCliente.Text) ? 0 : Convert.ToInt32(ucBuscarClienteTercero.TxtCliente.Text),
                IdTercero = string.IsNullOrEmpty(ucBuscarClienteTercero.TxtTercero.Text) ? 0 : Convert.ToInt32(ucBuscarClienteTercero.TxtTercero.Text),
                PorcentajeDescuentoDetalle = 0,
                PorcentajeValorDetalle = 0,
                IdComponente = 0
            };

            return responsable == null ? new Responsable() : responsable;
        }

        /// <summary>
        /// Metodo para Obtener la Vinculacion Seleccionada.
        /// </summary>
        /// <returns>Retorna Vinculacion.</returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 17/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private Vinculacion ObtenerSeleccionada()
        {
            var vinculacion = from
                                  item in Vinculaciones
                              where
                                  item.Tercero.Id == VinculacionSeleccionada.Tercero.Id
                                  && item.Contrato.Id == VinculacionSeleccionada.Contrato.Id
                              select
                                  item;

            return vinculacion.FirstOrDefault();
        }

        /// <summary>
        /// Metodo para obtener las ventas Seleccionadas.
        /// </summary>
        /// <returns>Lista de Ventas Seleccionadas.</returns>
        private List<int> ObtenerVentasMarcadas()
        {
            try
            {
                var ventasMarcadas = from
                                     ventas in grvVentas.Rows.Cast<GridViewRow>()
                                     where
                                         (ventas.FindControl("chkFacturar") as CheckBox).Checked
                                     select
                                         Convert.ToInt32(ventas.Cells[2].Text);

                return ventasMarcadas.ToList();
            }
            catch (ArgumentNullException error)
            {
                throw new FacturacionException(error, "ObtenerVentasMarcadas()");
            }
            catch (FormatException error)
            {
                throw new FacturacionException(error, "ObtenerVentasMarcadas()");
            }
            catch (OverflowException error)
            {
                throw new FacturacionException(error, "ObtenerVentasMarcadas()");
            }
            
        }

        /// <summary>
        /// Metodo para controlar las ventas Seleccionadas.
        /// </summary>
        /// <returns>Lista de Ventas No Seleccionadas.</returns>
        /// <remarks>
        /// Autor: Dario Fernando Preciado Barboza - INTERGRUPO\Dpreciado
        /// FechaDeCreacion: 03/07/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private List<int> ObtenerVentasNoMarcadas()
        {
            var ventasMarcadas = from
                                     ventas in grvVentas.Rows.Cast<GridViewRow>()
                                 where
                                     (ventas.FindControl("chkFacturar") as CheckBox).Checked == false
                                 select
                                     Convert.ToInt32(ventas.Cells[2].Text);

            return ventasMarcadas.ToList();
        }

        /// <summary>
        /// Retorna la lista de vinculaciones seleccionadas.
        /// </summary>
        /// <returns>Listado de vinculaciones seleccionadas.</returns>
        private List<Vinculacion> ObtenerVinculacionesSeleccionadas()
        {
            List<Vinculacion> lstVinculaciones = new List<Vinculacion>();
            foreach (Vinculacion vinculacion in Vinculaciones)
            {
                var listaVinculacionesSeleccionadas = (from fila in grvEntidades.Rows.Cast<GridViewRow>()
                                                       where (fila.FindControl(CHECKGENERAR) as CheckBox).Checked
                                                       && (fila.FindControl(CHECKACTIVO) as CheckBox).Checked
                                                       && vinculacion.Contrato.Id == Convert.ToInt32((fila.FindControl(LABELIDCONTRATO) as Label).Text)
                                                       && vinculacion.Tercero.Id == Convert.ToInt32((fila.FindControl(LABELIDENTIDAD) as Label).Text)
                                                       select vinculacion).ToList();

                var listaObservaciones = (from fila in grvEntidades.Rows.Cast<GridViewRow>()
                                          where vinculacion.Contrato.Id == Convert.ToInt32((fila.FindControl(LABELIDCONTRATO) as Label).Text)
                                          && vinculacion.Tercero.Id == Convert.ToInt32((fila.FindControl(LABELIDENTIDAD) as Label).Text)
                                          select fila).ToList();

                if (listaVinculacionesSeleccionadas.Count > 0)
                {
                    vinculacion.IndGenerar = 1;
                    lstVinculaciones.Add(vinculacion);
                }

                var listaVinculacionesOmitidas = (from fila in grvEntidades.Rows.Cast<GridViewRow>()
                                                  where (fila.FindControl(CHECKGENERAR) as CheckBox).Checked == false
                                                  && (fila.FindControl(CHECKACTIVO) as CheckBox).Checked
                                                  && vinculacion.Contrato.Id == Convert.ToInt32((fila.FindControl(LABELIDCONTRATO) as Label).Text)
                                                  && vinculacion.Tercero.Id == Convert.ToInt32((fila.FindControl(LABELIDENTIDAD) as Label).Text)
                                                  select vinculacion).ToList();
                if (listaVinculacionesOmitidas.Count > 0)
                {
                    vinculacion.IndGenerar = 0;
                    lstVinculaciones.Add(vinculacion);
                }

                foreach (GridViewRow fila in listaObservaciones)
                {
                    if (Convert.ToInt32((fila.FindControl(LABELIDCONTRATO) as Label).Text) == vinculacion.Contrato.Id
                        && Convert.ToInt32((fila.FindControl(LABELIDENTIDAD) as Label).Text) == vinculacion.Tercero.Id)
                    {
                        vinculacion.Observacion = (fila.FindControl(TXTOBSERVACION) as TextBox).Text;
                    }
                }
            }

            return lstVinculaciones;
        }

        /// <summary>
        /// Carga en el viewstate la vinculaci�n seleccionada para modificaci�n.
        /// </summary>
        /// <param name="identificadorEntidad">The id entidad.</param>
        /// <param name="identificadorContrato">The id contrato.</param>
        /// <param name="identificadorPlan">The id plan.</param>
        /// <returns>
        /// Retorna la vinculaci�n seleccionada.
        /// </returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 11/06/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private Vinculacion ObtenerVinculacionSeleccionada(int identificadorEntidad, int identificadorContrato, int identificadorPlan)
        {
            var vinculacion = from
                                  item in Vinculaciones
                              where
                                  item.IdAtencion == Convert.ToInt32(txtAtencion.Text)
                                  && item.Tercero.Id == identificadorEntidad
                                  && item.Contrato.Id == identificadorContrato
                                  && item.Plan.Id == identificadorPlan
                              select
                                  item;

            return vinculacion.FirstOrDefault();
        }

        /// <summary>
        /// M�todo para preseleccionar la secci�n por Defecto (Clinica Country).
        /// </summary>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void PreSeleccionarSeccion()
        {
            var secciones = ddlSeccion.DataSource as List<Seccion>;

            if (secciones != null && secciones.Count > 0)
            {
                var itemDefecto = (from
                                       item in secciones
                                   where
                                       item.CodEsor == Resources.ValoresPreSeleccion.FacRelacion_CodEsor
                                   select
                                       item).SingleOrDefault();

                if (itemDefecto != null)
                {
                    ddlSeccion.SelectedIndex = secciones.IndexOf(itemDefecto) + 1;
                }
            }
        }

        /// <summary>
        /// M�todo para preseleccionar la secci�n por Defecto (FACTURA DE VENTA).
        /// </summary>
        /// <remarks>
        /// Autor: Jhon Alberto Falcon Arellano - INTERGRUPO\Jfalcon
        /// FechaDeCreacion: 01/04/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void PreSeleccionarTipoMovimiento()
        {
            var movimientos = ddlTipoMovimiento.DataSource as List<TipoMovimiento>;

            if (movimientos != null && movimientos.Count > 0)
            {
                var itemDefecto = (from
                                       item in movimientos
                                   where
                                       item.Id == int.Parse(Resources.ValoresPreSeleccion.FacRelacion_TimIde)
                                   select
                                       item).SingleOrDefault();

                if (itemDefecto != null)
                {
                    ddlTipoMovimiento.SelectedIndex = movimientos.IndexOf(itemDefecto) + 1;
                }
            }
        }

        /// <summary>
        /// Evalua la grilla de vinculaciones si tiene un numero de orden repetido.
        /// </summary>
        /// <param name="orden">The orden.</param>
        /// <returns>Indica si tiene o no ordenes repetidas para la atenci�n.</returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 22/05/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private bool ValidarGrillaVinculacionesCampoOrden(int orden)
        {
            var vinculaciones = (from item in grvEntidades.Rows.Cast<GridViewRow>()
                                 select new Vinculacion()
                                 {
                                     IdAtencion = Convert.ToInt32(txtAtencion.Text),
                                     Orden = Convert.ToInt32((item.Cells[5].FindControl("txtOrden") as TextBox).Text)
                                 }).ToList();

            if (vinculaciones.Where(c => c.Orden == orden).Count() > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Valida informaci�n del proceso.
        /// </summary>
        /// <param name="estadoCuenta">The estado cuenta.</param>
        /// <returns>Validaci�n del proceso.</returns>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 14/05/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private bool ValidarInformacionProceso(EstadoCuentaEncabezado estadoCuenta)
        {
            var itemsProcesar = from
                                    atencion in estadoCuenta.FacturaAtencion
                                from
                                    detalle in atencion.Detalle
                                where
                                    detalle.Exclusion == null
                                    && detalle.ExclusionManual == null
                                select
                                    detalle;

            return itemsProcesar.Count() > 0 ? true : false;
        }

        /// <summary>
        /// Metodo para Controlar la Ejecucion de las Ventas.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tipoOperacion">The tipo operacion.</param>
        /// <remarks>
        /// Autor: David Mauricio Guti�rrez Ruiz - INTERGRUPO\dgutierrez
        /// FechaDeCreacion: 11/06/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void VinculacionVenta_OperacionEjecutada(object sender, Global.TipoOperacion tipoOperacion)
        {
            var ventas = ObtenerVentasMarcadas();
            CargarVentas(Convert.ToInt32(txtAtencion.Text), ventas);
        }

        /// <summary>
        /// Metodo de Vincular Entidad.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tipoOperacion">The tipo operacion.</param>
        /// <remarks>
        /// Autor: Iv�n Jos� Pimienta Serrano - INTERGRUPO\Ipimienta
        /// FechaDeCreacion: 22/05/2013
        /// UltimaModificacionPor: (Nombre del Autor de la modificaci�n - Usuario del dominio)
        /// FechaDeUltimaModificacion: (dd/MM/yyyy)
        /// EncargadoSoporte: (Nombre del Autor - Usuario del dominio)
        /// Descripci�n: Descripci�n detallada del metodo, procure especificar todo el metodo aqui.
        /// </remarks>
        private void VincularEntidad_OperacionEjecutada(object sender, Global.TipoOperacion tipoOperacion)
        {
            CargarVinculaciones(InfoVinculacion(Convert.ToInt32(txtAtencion.Text), 1));
        }

        #endregion�Metodos�Privados

        #endregion�Metodos
    }
}