using System;

namespace MoPhoGames.USpeak.Codec
{
	public class MuLawCodec : ICodec
	{
		public byte[] Encode(short[] data)
		{
			return MuLawEncoder.MuLawEncode(data);
		}

		public short[] Decode(byte[] data)
		{
			return MuLawDecoder.MuLawDecode(data);
		}
	}
}
