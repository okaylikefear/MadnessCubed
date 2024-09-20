using System;

namespace kube.data
{
	public class SubTaskDesc
	{
		public SubTaskDesc(int p, int p0, int p1)
		{
			this.type = p;
			this.kind = p0;
			this.target = p1;
		}

		public int type;

		public int kind;

		public int target;
	}
}
