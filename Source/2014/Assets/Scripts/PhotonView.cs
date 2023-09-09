using System;
using System.Reflection;
using Photon;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public int prefix
	{
		get
		{
			if (this.prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				this.prefixBackup = (int)PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return this.prefixBackup;
		}
		set
		{
			this.prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!this.didAwake)
			{
				this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
			}
			return this.instantiationDataField;
		}
		set
		{
			this.instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return this.ownerId * PhotonNetwork.MAX_VIEW_IDS + this.subId;
		}
		set
		{
			bool flag = this.didAwake && this.subId == 0;
			this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			this.subId = value % PhotonNetwork.MAX_VIEW_IDS;
			if (flag)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView
	{
		get
		{
			return this.ownerId == 0;
		}
	}

	public PhotonPlayer owner
	{
		get
		{
			return PhotonPlayer.Find(this.ownerId);
		}
	}

	public int OwnerActorNr
	{
		get
		{
			return this.ownerId;
		}
	}

	public bool isMine
	{
		get
		{
			return this.ownerId == PhotonNetwork.player.ID || (this.isSceneView && PhotonNetwork.isMasterClient);
		}
	}

	public void Awake()
	{
		PhotonNetwork.networkingPeer.RegisterPhotonView(this);
		this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
		this.didAwake = true;
	}

	public void OnApplicationQuit()
	{
		this.destroyedByPhotonNetworkOrQuit = true;
	}

	public void OnDestroy()
	{
		if (!this.destroyedByPhotonNetworkOrQuit)
		{
			PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
		}
		if (!this.destroyedByPhotonNetworkOrQuit && !Application.isLoadingLevel)
		{
			if (this.instantiationId > 0)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"OnDestroy() seems to be called without PhotonNetwork.Destroy()?! GameObject: ",
					base.gameObject,
					" Application.isLoadingLevel: ",
					Application.isLoadingLevel
				}));
			}
			else if (this.viewID <= 0)
			{
				UnityEngine.Debug.LogWarning(string.Format("OnDestroy manually allocated PhotonView {0}. The viewID is 0. Was it ever (manually) set?", this));
			}
			else if (this.isMine && !PhotonNetwork.manuallyAllocatedViewIds.Contains(this.viewID))
			{
				UnityEngine.Debug.LogWarning(string.Format("OnDestroy manually allocated PhotonView {0}. The viewID is local (isMine) but not in manuallyAllocatedViewIds list. Use UnAllocateViewID() after you destroyed the PV.", this));
			}
		}
		if (PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(this.instantiationId))
		{
			GameObject x = PhotonNetwork.networkingPeer.instantiatedObjects[this.instantiationId];
			bool flag = x == base.gameObject;
			if (flag)
			{
				UnityEngine.Debug.LogWarning(string.Format("OnDestroy for PhotonView {0} but GO is still in instantiatedObjects. instantiationId: {1}. Use PhotonNetwork.Destroy(). {2} Identical with this: {3} PN.Destroyed called for this PV: {4}", new object[]
				{
					this,
					this.instantiationId,
					(!Application.isLoadingLevel) ? string.Empty : "Loading new scene caused this.",
					flag,
					this.destroyedByPhotonNetworkOrQuit
				}));
			}
		}
	}

	protected internal void ExecuteOnSerialize(PhotonStream pStream, PhotonMessageInfo info)
	{
		if (this.failedToFindOnSerialize)
		{
			return;
		}
		if (this.OnSerializeMethodInfo == null && !NetworkingPeer.GetMethod(this.observed as UnityEngine.MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out this.OnSerializeMethodInfo))
		{
			UnityEngine.Debug.LogError("The observed monobehaviour (" + this.observed.name + ") of this PhotonView does not implement OnPhotonSerialize()!");
			this.failedToFindOnSerialize = true;
			return;
		}
		this.OnSerializeMethodInfo.Invoke(this.observed, new object[]
		{
			pStream,
			info
		});
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		if (PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
		{
			PhotonNetwork.RPC(this, methodName, PhotonNetwork.masterClient, parameters);
		}
		else
		{
			PhotonNetwork.RPC(this, methodName, target, parameters);
		}
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", new object[]
		{
			this.viewID,
			base.gameObject.name,
			(!this.isSceneView) ? string.Empty : "(scene)",
			this.prefix
		});
	}

	public int subId;

	public int ownerId;

	public int group;

	protected internal bool mixedModeIsReliable;

	public int prefixBackup = -1;

	private object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public Component observed;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public int instantiationId;

	private bool didAwake;

	protected internal bool destroyedByPhotonNetworkOrQuit;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;
}
