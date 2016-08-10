using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBlockInfo : MonoBehaviour
{
	[SerializeField]
	Text BlockName;

	public void SetBlockSetEntryUI(BlockSetEntry cBlockSetEntry)
	{
		BlockName.text = cBlockSetEntry.BlockInfo.Name;
	}
}
