using UnityEngine;
using UnityEngine.UI;

namespace Bounds.Liga {

	public class CuadroDivision : MonoBehaviour {

		public void SetDivision(string division) {

			GetComponentInChildren<Text>().text = $"División: {division}";
			Image image = GetComponent<Image>();

			switch (division) {
				case "ORO": image.color = new Color(1f, 0.84f, 0f, 0.7f); break;
				case "PLATA": image.color = new Color(0.75f, 0.75f, 0.75f, 0.7f); break;
				case "HIERRO": image.color = new Color(0.75f, 0.75f, 0.75f, 0.7f); break;
				case "BRONCE": image.color = new Color(0.8f, 0.5f, 0.2f, 0.7f); break;
				case "MADERA": image.color = new Color(0.8f, 0.7f, 0.5f, 0.7f); break;
				case "DIAMANTE": image.color = new Color(1f, 0.75f, 0.8f, 0.7f); break;
				case "MÍTICO": image.color = new Color(1f, 0.4f, 0.4f, 0.7f); break;
				case "BARRO": image.color = new Color(0.6f, 0.4f, 0.2f, 0.7f); break;
				default: Debug.LogWarning("División no válida: " + division); break;
			}

		}

	}

}