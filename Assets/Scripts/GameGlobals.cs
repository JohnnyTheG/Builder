using UnityEngine;
using System.Collections;

public class GameGlobals : Singleton<GameGlobals>
{
	public GameObject MouseHighlight;

	public BuildMenuController BuildMenuController;

	public Material GhostMaterial;
	public Color CanBuildColor;
	public Color CannotBuildColor;
}
