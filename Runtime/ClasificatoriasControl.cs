using Bounds.Infraestructura;
using Bounds.Salesforce;
using Bounds.Sistema;
using Bounds.Sistema.Parametros;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces.Salida;
using Ging1991.Persistencia.Direcciones;
using Ging1991.Persistencia.Lectores;
using Ging1991.Persistencia.Proveedores;
using Ging1991.Salesforce;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bounds.Liga {

	public class ClasificatoriasControl : MonoBehaviour {

		public ControlBounds controlBounds;
		private ParametrosGlobales parametros;
		public GameObject indicadorVictorias;
		public GameObject indicadorDerrotas;
		public GameObject oponente1;
		public GameObject oponente2;
		public GameObject oponente3;
		public Text nombreOBJ;
		public CuadroDivision cuadroDivision;
		private IProveedor<string, string> proveedorTexto;

		void Start() {
			parametros = controlBounds.InicializarEscena("GENERAL");
			proveedorTexto = new ProveedorTexto(parametros.direccionesGeneradas["IDIOMA"], TipoLector.RECURSOS);
			nombreOBJ.text = proveedorTexto.GetElemento("JUGADOR X").Replace("[JUGADOR]", RegistroGlobal.Instancia.configuracion.GetNombre());
			CargarDatos();
		}


		async void CargarDatos() {
			LectorCredenciales lector = new LectorCredenciales(new DireccionRecursos("Salesforce", "Credenciales").Generar());
			ServicioTraerDatosDeDivision servicio = new(lector.Leer());

			if (await servicio.AutorizarAsincronico()) {
				ServicioTraerDatosDeDivision.Puntuacion puntuacion = await servicio.LlamarAsincronica(RegistroGlobal.Instancia.configuracion.GetNombre());

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
			SceneManager.LoadScene(parametros.escenaAnterior);
		}


		public void BotonJugarPartida() {
			ControlEscena.GetInstancia().CambiarEscena("CLASIFICATORIAS CARGA");
		}


	}

}