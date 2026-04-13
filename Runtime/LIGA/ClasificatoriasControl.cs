using Bounds.Entrenamiento;
using Bounds.Infraestructura;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Bounds.Salesforce;
using Ging1991.Interfaces.Salida;
using Ging1991.Persistencia.Direcciones;
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
		public MusicaDeFondo musicaDeFondo;

		public Text nombreOBJ;
		public Configuracion configuracion;
		public PersonalizarUI personalizarUI;

		void Start() {
			personalizarUI.Personalizar();
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			musicaDeFondo.Inicializar(parametros.direcciones["MUSICA_TIENDA"]);
			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			nombreOBJ.text = $"Nombre: {configuracion.GetNombre()}";
			CargarDatos();
		}


		async void CargarDatos() {
			LectorCredenciales lector = new LectorCredenciales(new DireccionRecursos("Salesforce", "Credenciales").Generar());
			ServicioTraerDatosDeDivision servicio = new(lector.Leer());

			if (await servicio.AutorizarAsincronico()) {
				ServicioTraerDatosDeDivision.Puntuacion puntuacion = await servicio.LlamarAsincronica(configuracion.GetNombre());

				CuadroDivision cuadroDivision = GameObject.Find("CuadroDivision").GetComponent<CuadroDivision>();
				cuadroDivision.SetDivision(puntuacion.division);

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