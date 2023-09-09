using System;

namespace kube.data
{
	public struct EpisodeDesc
	{
		public EpisodeDesc(string title, int id)
		{
			this.title = title;
			this.id = id;
		}

		public string title;

		public int id;
	}
}
