using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour
{
	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 20.0f, 600.0f, 20.0f), "SelectedBlock: " + (BlockManager.Instance.GetSelectedBlock() != null ? BlockManager.Instance.GetSelectedBlock().name : "None"));
		GUI.Label(new Rect(0.0f, 40.0f, 600.0f, 20.0f), "Currency: " + CurrencyManager.Instance.GetCurrency());
		GUI.Label(new Rect(0.0f, 60.0f, 600.0f, 20.0f), "Power: " + PowerManager.Instance.GetPowerOutput() + "/" + PowerManager.Instance.GetPowerCapacity());
	}
}
