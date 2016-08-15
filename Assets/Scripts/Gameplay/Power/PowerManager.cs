using UnityEngine;
using System.Collections;

public class PowerManager : Singleton<PowerManager>
{
	int PowerCapacity = 0;
	int PowerOutput = 0;
	int PowerConsumption = 0;

	public void AddPowerCapacity(int nPowerCapacity)
	{
		PowerCapacity += nPowerCapacity;
	}

	public void RemovePowerCapacity(int nPowerCapacity)
	{
		PowerCapacity -= nPowerCapacity;
	}

	public void AddPowerOutput(int nPowerOutput)
	{
		PowerOutput += nPowerOutput;
	}

	public void RemovePowerOutput(int nPowerOutput)
	{
		PowerOutput -= nPowerOutput;
	}

	public void AddPowerConsumption(int nPowerConsumption)
	{
		PowerConsumption += nPowerConsumption;
	}

	public void RemovePowerConsumption(int nPowerConsumption)
	{
		PowerConsumption -= nPowerConsumption;
	}

	public int GetPowerOutput()
	{
		// Output should never be higher than capacity when returned or displayed.
		// Clamp the returned value between 0 and current capacity.
		// So, if you have 2 machines generating 1000 power, but only capacity to store 800,
		// it should never display or allow use of 1000. Adding another capacitor gives 1200
		// meaning the output would be 1000 at that point as it is within the capacity!
		return Mathf.Clamp(PowerOutput, 0, PowerCapacity);
	}

	public int GetPowerCapacity()
	{
		return PowerCapacity;
	}

	public int GetPowerConsumption()
	{
		return PowerConsumption;
	}
}
