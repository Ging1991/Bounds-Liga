using Bounds.Infraestructura;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Bounds.Salesforce;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces.Salida;
using Ging1991.Musica;
using Ging1991.Persistencia.Direcciones;
using Ging1991.Persistencia.Lectores;
using Ging1991.Persistencia.Proveedores;
using Ging1991.Salesforce;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bounds.Liga {

	public class ClasificatoriasControl : MonoBehaviour {

		public GameObject indicadorVictorias;
		public GameObject indicadorDerrotas;
		public GameObject oponente1;
		public GameObject oponente2;
		public GameObject oponente3;
		public ParametrosControl parametrosControl;

		private void InicializarMusica(string direccion) {
			MusicaAmbiental musicaAmbiental = MusicaAmbiental.Instancia;
			if (musicaAmbiental.actual != "GENERAL") {
				musicaAmbiental.Inicializar(new ProveedorAudios(new DireccionRecursos(direccion)));
				musicaAmbiental.Reproducir("GENERAL");
			}
		}

		public Text nombreOBJ;
		public Configuracion configuracion;
		public ControlUIBounds personalizarUI;
		public CuadroDivision cuadroDivision;
		private IProveedor<string, string> proveedorTexto;

		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			proveedorTexto = new ProveedorTexto(parametros.direcciones["SISTEMA"], TipoLector.RECURSOS);
			personalizarUI.Personalizar(parametros.direcciones["SISTEMA"], parametros.direcciones["COLORES"]);
			InicializarMusica(parametros.direcciones["MUSICA_AMBIENTAL"]);

			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			nombreOBJ.text = proveedorTexto.GetElemento("JUGADOR X").Replace("[JUGADOR]", configuracion.GetNombre());
			CargarDatos();
		}


		async void CargarDatos() {
			LectorCredenciales lector = new LectorCredenciales(new DireccionRecursos("Salesforce", "Credenciales").Generar());
			ServicioTraerDatosDeDivision servicio = new(lector.Leer());

			if (await servicio.AutorizarAsincronico()) {
				ServicioTraerDatosDeDivision.Puntuacion puntuacion = await servicio.LlamarAsincronica(configuracion.GetNombre());

				cuadroDivision.SetDivision(puntuacion.division, proveedorTexto.GetElemento("DIVISION X").Replace("[DIVISION]", puntuacion.division));

				indicadorVictorias.GetComponent<Indicador>().SetValor(Color.green, puntuacion.victorias, 5, 5);
				indicadorDerrotas.GetComponent<Indicador>().SetValor(Color.red, puntuacion.derrotas, 5, 5);

				if (puntuacion.oponentes.Count > 0)
					oponente1.GetComponentInChildren<Text>().text = puntuacion.oponentes[0];
				if (puntuacion.oponentes.Count > 1)
					oponente2.GetComponentInChildren<Text>().text = puntuacion.oponentes[1];
				if (puntuacion.oponentes.Count > 2)
					oponente3.GetComponentInChildren<Text>().text = puntuacion.oponentes[2];
			}
		}


		public void Volver() {
			SceneManager.LoadScene(parametrosControl.parametros.escenaPadre);
		}



		public void BotonJugarPartida() {
			ControlEscena.GetInstancia().CambiarEscena("CLASIFICATORIAS CARGA");
		}


	}

}