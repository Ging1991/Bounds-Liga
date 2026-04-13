using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Bounds.Global.Mazos;
using Bounds.Persistencia.Datos;
using Bounds.Infraestructura;
using Bounds.Salesforce;
using Ging1991.Salesforce;
using Ging1991.Persistencia.Direcciones;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Bounds.Modulos.Persistencia;
using Bounds.Musica;
using Bounds.Entrenamiento;

namespace Bounds.Liga {

	public class LigaControl : MonoBehaviour {

		private ServicioEncontrarOponente.Oponente oponente;
		public ParametrosControl parametrosControl;
		public Configuracion configuracion;
		public MusicaDeFondo musicaDeFondo;
		public PersonalizarUI personalizarUI;

		void Start() {
			personalizarUI.Personalizar();
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
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
				cartelTexto.text = "Oponente: " + oponente.nombre;
				Text cartelTexto2 = GameObject.Find("Mensaje2").GetComponentInChildren<Text>();
				cartelTexto2.text = "Mazo: " + oponente.nombreMazo;

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

			Global.Mazo mazo1 = new MazoJugador(MazoJugador.GetPredeterminado());

			MazoBD mazoBD = new MazoBD();
			List<string> cartas = new List<string>();
			foreach (var carta in oponente.cartas)
				cartas.Add($"{carta}_A_N_1");
			mazoBD.cartas = cartas;
			mazoBD.principalVacio = $"{oponente.vacio}_A_N_1";

			Global.Mazo mazo2 = new MazoSF(mazoBD);

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