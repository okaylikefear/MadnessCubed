using System;
using MoPhoGames.USpeak.Codec;

public class RawCodec : ICodec
{
	public byte[] Encode(short[] data)
	{
		byte[] array = new byte[data.Length * 2];
		Buffer.BlockCopy(data, 0, array, 0, array.Length);
		return array;
	}

	public short[] Decode(byte[] data)
	{
		short[] array = new short[data.Length / 2];
		Buffer.BlockCopy(data, 0, array, 0, data.Length);
		return array;
	}
}
