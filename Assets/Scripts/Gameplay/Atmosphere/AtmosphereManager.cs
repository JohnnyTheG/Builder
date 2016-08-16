using UnityEngine;
using System.Collections;

public class AtmosphereManager : Singleton<AtmosphereManager>
{
	int AtmosphereGenerationRate = 0;

	// This may need to be a const?
	// Should capacity change?
	int AtmosphereCapacity = 0;
	
	// How full the atmosphere is.
	int AtmosphereFill = 0;

	void Start()
	{
		StartCoroutine(Tick());
	}

	float GetAtmospherePercentage()
	{
		return AtmosphereFill / AtmosphereCapacity;
	}

	IEnumerator Tick()
	{
		while (true)
		{
			AtmosphereFill = Mathf.Clamp(AtmosphereFill + AtmosphereGenerationRate, 0, AtmosphereCapacity);

			yield return new WaitForSeconds(1.0f);
		}
	}
}
