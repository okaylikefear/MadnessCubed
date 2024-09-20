using System;

namespace MoPhoGames.USpeak.Core
{
	public struct USpeakFrameContainer
	{
		public void LoadFrom(byte[] source)
		{
			int num = BitConverter.ToInt32(source, 0);
			this.Samples = BitConverter.ToUInt16(source, 4);
			this.encodedData = new byte[num];
			Array.Copy(source, 6, this.encodedData, 0, num);
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[6 + this.encodedData.Length];
			byte[] bytes = BitConverter.GetBytes(this.encodedData.Length);
			bytes.CopyTo(array, 0);
			byte[] bytes2 = BitConverter.GetBytes(this.Samples);
			Array.Copy(bytes2, 0, array, 4, 2);
			for (int i = 0; i < this.encodedData.Length; i++)
			{
				array[i + 6] = this.encodedData[i];
			}
			return array;
		}

		public ushort Samples;

		public byte[] encodedData;
	}
}
