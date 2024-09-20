using System;

namespace kube.map
{
	public class CubeTypesGrid
	{
		public CubeTypesGrid(CubeGrid grid)
		{
			this._grid = grid;
		}

		public int this[int x, int y, int z]
		{
			get
			{
				return this._grid.get(x, y, z);
			}
			set
			{
				this._grid.set(x, y, z, value);
			}
		}

		public CubeGrid _grid;
	}
}
