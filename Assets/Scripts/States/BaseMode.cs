using UnityEngine;
using System.Collections;

public abstract class BaseMode : MonoBehaviour
{
	enum State
	{
		Running,
		Shutdown,
	}

	State m_eState = State.Running;

	public delegate void OnShutdownCompleteCallback();

	public event OnShutdownCompleteCallback OnShutdownComplete;

	// This MUST be called to signal that state is ready to change.
	public void InvokeOnShutdownComplete()
	{
		if (OnShutdownComplete != null)
		{
			OnShutdownComplete();
		}
	}

	public virtual void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		this.OnShutdownComplete += OnShutdownComplete;
	}

	protected BlockInfo GetSelectedBlock()
	{
		return BlockManager.Instance.GetSelectedBlock();
	}

	protected void SetSelectedBlock(BlockInfo cBlockInfo)
	{
		BlockManager.Instance.SetSelectedBlock(cBlockInfo);
	}

	protected GameObject GetBlockBuildType()
	{
		return BlockManager.Instance.BlockBuildType;
	}
}
