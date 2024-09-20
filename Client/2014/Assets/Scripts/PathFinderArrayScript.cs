using System;
using UnityEngine;

public class PathFinderArrayScript : MonoBehaviour
{
	public void ClearArray()
	{
		this.openedArrayNum = (this.closedArrayNum = 0);
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.openedArray = new AStarElement[this.arraySize];
		this.closedArray = new AStarElement[this.arraySize];
	}

	private void Update()
	{
	}

	public int arraySize;

	public int openedArrayNum;

	public int closedArrayNum;

	public AStarElement[] openedArray;

	public AStarElement[] closedArray;
}
