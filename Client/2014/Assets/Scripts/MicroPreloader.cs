using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class MicroPreloader : MonoBehaviour
{
	public virtual IEnumerator Start()
	{
		return new MicroPreloader._0024Start_002438(this).GetEnumerator();
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}

	[CompilerGenerated]
	[Serializable]
	internal sealed class _0024Start_002438 : GenericGenerator<object>
	{
		public _0024Start_002438(MicroPreloader self_)
		{
			this._0024self__002446 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new MicroPreloader._0024Start_002438._0024(this._0024self__002446);
		}

		internal MicroPreloader _0024self__002446;
	}
}
