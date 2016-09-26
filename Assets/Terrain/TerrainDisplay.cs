using UnityEngine;
using System.Collections;

public class TerrainDisplay : Singleton<TerrainDisplay>
{
	[SerializeField]
	Renderer TextureRenderer;

    public void DrawTexture(Texture2D cTexture)
	{
		

		TextureRenderer.sharedMaterial.mainTexture = cTexture;
		TextureRenderer.transform.localScale = new Vector3(cTexture.width, 1.0f, cTexture.height);
	}
}
