using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Bounds.Infraestructura;
using Bounds.Salesforce;
using Ging1991.Salesforce;
using Ging1991.Persistencia.Direcciones;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Bounds.Musica;
using Bounds.Mazos;
using Ging1991.Core.Interfaces;
using Ging1991.Persistencia.Proveedores;
using Ging1991.Persistencia.Lectores;

namespace Bounds.Liga {

	public class LigaControl : MonoBehaviour {

		private ServicioEncontrarOponente.Oponente oponente;
		public ParametrosControl parametrosControl;
		public Configuracion configuracion;
		public MusicaDeFondo musicaDeFondo;
		public ControlUIBounds personalizarUI;
		private IProveedor<string, string> proveedorTexto;

		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			proveedorTexto = new ProveedorTexto(parametros.direcciones["SISTEMA"], TipoLector.RECURSOS);
			personalizarUI.Personalizar(parametros.direcciones["SISTEMA"], parametros.direcciones["COLORES"]);
			musicaDeFondo.Inicializar(parametros.direcciones["MUSICA_TIENDA"]);

			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			CargarOponente();
		}


		private async void CargarOponente() {
			LectorCredenciales lector = new LectorCredenciales(new DireccionRecursos("Salesforce", "Credenciales").Generar());
			ServicioEncontrarOponente api = new(lector.Leer());

			if (await api.AutorizarAsincronico()) {
				oponente = await api.LlamarAsincronica(configuracion.GetNombre());
				Text cartelTexto = GameObject.Find("Mensaje1").GetComponentInChildren<Text>();
				cartelTexto.text = proveedorTexto.GetElemento("OPONENTE X").Replace("[OPONENTE]", oponente.nombre);
				Text cartelTexto2 = GameObject.Find("Mensaje2").GetComponentInChildren<Text>();
				cartelTexto2.text = proveedorTexto.GetElemento("MAZO X").Replace("[MAZO]", oponente.nombreMazo);

			}
			else {
				Debug.Log("Fallo");
			}
		}


		public void PresionarBotonDuelo() {
			GlobalDuelo parametros = GlobalDuelo.GetInstancia();
			parametros.modo = "LIGA";
			parametros.finalizarDuelo = new FinLiga();

			parametros.jugadorLP1 = 4000;
			parametros.jugadorLP2 = 4000;

			parametros.jugadorNombre1 = configuracion.GetNombre();
			parametros.jugadorNombre2 = oponente.nombre;

			parametros.jugadorMiniatura1 = "LAUNIX";
			parametros.jugadorMiniatura2 = oponente.avatar;

			Mazo mazo1 = new MazoJugador(MazoJugador.GetPredeterminado());

			MazoBD mazoBD = new MazoBD();
			mazoBD.cartas = oponente.cartas;
			mazoBD.principalVacio = $"{oponente.vacio}_A_N_1";

			Mazo mazo2 = new MazoSF(mazoBD);

			parametros.mazo1 = mazo1.cartas;
			parametros.mazo2 = mazo2.cartas;

			parametros.mazoVacio1 = mazo1.principalVacio;
			parametros.mazoVacio2 = mazo2.principalVacio;

			SceneManager.LoadScene("DUELO");
		}


		public class FinLiga : IFinalizarDuelo {
			public void FinalizarDuelo(int jugadorGanador) {
				ProcesarModoLiga(GlobalDuelo.GetInstancia(), jugadorGanador == 1);
				ControlEscena escena = ControlEscena.GetInstancia();
				escena.CambiarEscena("MENU");
			}

		}


		private static async void ProcesarModoLiga(GlobalDuelo parametros, bool haGanado) {
			List<int> cartasID1 = new List<int>();
			foreach (var carta in parametros.mazo1) {
				cartasID1.Add(carta.cartaID);
			}

			List<int> cartasID2 = new List<int>();
			foreach (var carta in parametros.mazo2) {
				cartasID2.Add(carta.cartaID);
			}
			Configuracion configuracion = new(new DireccionDinamica("CONFIGURACION", "CONFIGURACION.json").Generar());
			string jugador1 = configuracion.GetNombre();
			string jugador2 = parametros.jugadorNombre2;

			LectorCredenciales lector = new LectorCredenciales(new DireccionRecursos("Salesforce", "Credenciales").Generar());
			ServicioGuardarResultado servicio = new ServicioGuardarResultado(lector.Leer());

			if (await servicio.AutorizarAsincronico()) {
				if (haGanado) {
					servicio.LlamarAsincronica(jugador1, jugador2, cartasID1, cartasID2);
				}
				else {
					servicio.LlamarAsincronica(jugador2, jugador1, cartasID2, cartasID1);
				}
			}
		}


		public void Volver() {
			SceneManager.LoadScene(parametrosControl.parametros.escenaPadre);
		}


	}

}