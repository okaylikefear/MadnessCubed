using System;

namespace MoPhoGames.USpeak.Core
{
	public class USpeakSettingsData
	{
		public USpeakSettingsData()
		{
			this.bandMode = BandMode.Narrow;
			this.Is3D = false;
		}

		public USpeakSettingsData(byte src)
		{
			if ((src & 1) == 1)
			{
				this.Is3D = true;
			}
			else
			{
				this.Is3D = false;
			}
			if ((src & 4) == 4)
			{
				this.bandMode = BandMode.Narrow;
			}
			else
			{
				this.bandMode = BandMode.Wide;
			}
		}

		public byte ToByte()
		{
			byte b = 0;
			if (this.Is3D)
			{
				b |= 1;
			}
			else if (this.bandMode == BandMode.Narrow)
			{
				b |= 4;
			}
			return b;
		}

		public BandMode bandMode;

		public bool Is3D;
	}
}
