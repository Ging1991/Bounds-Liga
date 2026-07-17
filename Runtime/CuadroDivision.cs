using UnityEngine;
using UnityEngine.UI;

namespace Bounds.Liga {

	public class CuadroDivision : MonoBehaviour {

		public Image imagen;

		public void SetDivision(string division, string mensaje) {

			GetComponentInChildren<Text>().text = mensaje;

			switch (division) {
				case "ORO": imagen.color = new Color(1f, 0.84f, 0f, 0.7f); break;
				case "PLATA": imagen.color = new Color(0.75f, 0.75f, 0.75f, 0.7f); break;
				case "HIERRO": imagen.color = new Color(0.75f, 0.75f, 0.75f, 0.7f); break;
				case "BRONCE": imagen.color = new Color(0.8f, 0.5f, 0.2f, 0.7f); break;
				case "MADERA": imagen.color = new Color(0.8f, 0.7f, 0.5f, 0.7f); break;
				case "DIAMANTE": imagen.color = new Color(1f, 0.75f, 0.8f, 0.7f); break;
				case "MÍTICO": imagen.color = new Color(1f, 0.4f, 0.4f, 0.7f); break;
				case "BARRO": imagen.color = new Color(0.6f, 0.4f, 0.2f, 0.7f); break;
				default: Debug.LogWarning("División no válida: " + division); break;
			}

		}

	}

}