using System;
using UnityEngine;

public class PhotonTransformViewScaleControl
{
	public PhotonTransformViewScaleControl(PhotonTransformViewScaleModel model)
	{
		this.m_Model = model;
	}

	public Vector3 GetScale(Vector3 currentScale)
	{
		switch (this.m_Model.InterpolateOption)
		{
		default:
			return this.m_NetworkScale;
		case PhotonTransformViewScaleModel.InterpolateOptions.MoveTowards:
			return Vector3.MoveTowards(currentScale, this.m_NetworkScale, this.m_Model.InterpolateMoveTowardsSpeed * Time.deltaTime);
		case PhotonTransformViewScaleModel.InterpolateOptions.Lerp:
			return Vector3.Lerp(currentScale, this.m_NetworkScale, this.m_Model.InterpolateLerpSpeed * Time.deltaTime);
		}
	}

	public void OnPhotonSerializeView(Vector3 currentScale, PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.m_Model.SynchronizeEnabled)
		{
			return;
		}
		if (stream.isWriting)
		{
			stream.SendNext(currentScale);
			this.m_NetworkScale = currentScale;
		}
		else
		{
			this.m_NetworkScale = (Vector3)stream.ReceiveNext();
		}
	}

	private PhotonTransformViewScaleModel m_Model;

	private Vector3 m_NetworkScale = Vector3.one;
}
