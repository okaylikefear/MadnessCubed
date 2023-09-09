using System;

public class KubeStream
{
	public KubeStream(byte[] data = null)
	{
		if (data == null)
		{
			data = new byte[32];
		}
		this.data = data;
	}

	public int Length
	{
		get
		{
			return this.pos;
		}
	}

	public void WriteByte(byte b)
	{
		this.data[this.pos] = b;
		this.pos++;
	}

	public byte ReadByte()
	{
		byte result = this.data[this.pos];
		this.pos++;
		return result;
	}

	public void WriteShort(ushort s)
	{
		this.data[this.pos++] = (byte)(s & 255);
		this.data[this.pos++] = (byte)(s >> 8 & 255);
	}

	public ushort ReadShort()
	{
		ushort num = (ushort)this.data[this.pos++];
		return num | (ushort)(this.data[this.pos++] << 8);
	}

	public byte[] data;

	protected int pos;
}
