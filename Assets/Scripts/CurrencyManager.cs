using UnityEngine;
using System.Collections;

public class CurrencyManager : Singleton<CurrencyManager>
{
	int m_nCurrency = 0;

	public void SetCurrency(int nCurrency)
	{
		m_nCurrency = nCurrency;
	}

	public void SpendCurrency(int nSpentCurrency)
	{
		m_nCurrency -= nSpentCurrency;
	}

	public void CollectCurrency(int nCollectedCurrency)
	{
		m_nCurrency += nCollectedCurrency;
	}

	public bool CurrencyAvailable(int nCurrency)
	{
		return m_nCurrency >= nCurrency;
	}
}
