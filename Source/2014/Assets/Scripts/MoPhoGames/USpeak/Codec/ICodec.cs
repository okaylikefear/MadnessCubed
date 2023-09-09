using System;

namespace MoPhoGames.USpeak.Codec
{
	public interface ICodec
	{
		byte[] Encode(short[] data);

		short[] Decode(byte[] data);
	}
}
