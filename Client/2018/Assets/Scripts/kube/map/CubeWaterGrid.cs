using System;

namespace kube.map
{
	public class CubeWaterGrid
	{
		public CubeWaterGrid(CubeGrid grid)
		{
			this._grid = grid;
		}

		public byte this[int x, int y, int z]
		{
			get
			{
				int num = 0;
				int num2 = 0;
				this._grid.get(x, y, z, ref num, ref num2);
				if (num == 128 || num == 0)
				{
					return (byte)num2;
				}
				return 0;
			}
			set
			{
				int num = this._grid.get(x, y, z);
				if (num == 128 || num == 0)
				{
					this._grid.setdata(x, y, z, (int)value);
				}
			}
		}

		public CubeGrid _grid;
	}
}
