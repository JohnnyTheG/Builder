using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBlockInfo : MonoBehaviour
{
	[SerializeField]
	Text BlockName;

	public void SetBlockInfo(BlockInfo cBlockInfo)
	{
		BlockName.text = cBlockInfo.Name;
	}
}
