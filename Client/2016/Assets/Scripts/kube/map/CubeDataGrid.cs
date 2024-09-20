using System;

namespace kube.map
{
	public class CubeDataGrid
	{
		public CubeDataGrid(CubeGrid grid)
		{
			this._grid = grid;
		}

		public byte this[int x, int y, int z]
		{
			get
			{
				return this._grid.getdata(x, y, z);
			}
			set
			{
				this._grid.setdata(x, y, z, (int)value);
			}
		}

		public CubeGrid _grid;
	}
}
