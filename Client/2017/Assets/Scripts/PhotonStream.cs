using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonStream
{
	public PhotonStream(bool write, object[] incomingData)
	{
		this.write = write;
		if (incomingData == null)
		{
			this.writeData = new List<object>(10);
		}
		else
		{
			this.readData = incomingData;
		}
	}

	public bool isWriting
	{
		get
		{
			return this.write;
		}
	}

	public bool isReading
	{
		get
		{
			return !this.write;
		}
	}

	public int Count
	{
		get
		{
			return (!this.isWriting) ? this.readData.Length : this.writeData.Count;
		}
	}

	public object ReceiveNext()
	{
		if (this.write)
		{
			UnityEngine.Debug.LogError("Error: you cannot read this stream that you are writing!");
			return null;
		}
		object result = this.readData[(int)this.currentItem];
		this.currentItem += 1;
		return result;
	}

	public object PeekNext()
	{
		if (this.write)
		{
			UnityEngine.Debug.LogError("Error: you cannot read this stream that you are writing!");
			return null;
		}
		return this.readData[(int)this.currentItem];
	}

	public void SendNext(object obj)
	{
		if (!this.write)
		{
			UnityEngine.Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
			return;
		}
		this.writeData.Add(obj);
	}

	public object[] ToArray()
	{
		return (!this.isWriting) ? this.readData : this.writeData.ToArray();
	}

	public void Serialize(ref bool myBool)
	{
		if (this.write)
		{
			this.writeData.Add(myBool);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			myBool = (bool)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref int myInt)
	{
		if (this.write)
		{
			this.writeData.Add(myInt);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			myInt = (int)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref string value)
	{
		if (this.write)
		{
			this.writeData.Add(value);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			value = (string)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref char value)
	{
		if (this.write)
		{
			this.writeData.Add(value);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			value = (char)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref short value)
	{
		if (this.write)
		{
			this.writeData.Add(value);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			value = (short)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref float obj)
	{
		if (this.write)
		{
			this.writeData.Add(obj);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			obj = (float)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref PhotonPlayer obj)
	{
		if (this.write)
		{
			this.writeData.Add(obj);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			obj = (PhotonPlayer)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref Vector3 obj)
	{
		if (this.write)
		{
			this.writeData.Add(obj);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			obj = (Vector3)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref Vector2 obj)
	{
		if (this.write)
		{
			this.writeData.Add(obj);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			obj = (Vector2)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	public void Serialize(ref Quaternion obj)
	{
		if (this.write)
		{
			this.writeData.Add(obj);
		}
		else if (this.readData.Length > (int)this.currentItem)
		{
			obj = (Quaternion)this.readData[(int)this.currentItem];
			this.currentItem += 1;
		}
	}

	private bool write;

	private List<object> writeData;

	private object[] readData;

	internal byte currentItem;
}
