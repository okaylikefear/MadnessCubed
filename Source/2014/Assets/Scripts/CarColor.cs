using System;
using UnityEngine;

public class CarColor : MonoBehaviour
{
	private void Start()
	{
		this.car_renderer = base.gameObject.GetComponent<Renderer>();
		string text = this.car_main_color + " ";
		for (int i = 0; i < this.car_renderer.materials.Length; i++)
		{
			string text2 = this.car_renderer.transform.renderer.materials[i] + " ";
			bool flag = true;
			for (int j = 0; j < 23; j++)
			{
				if (text[j] != text2[j])
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.mat_index = i;
			}
		}
	}

	private void Update()
	{
		this.car_renderer.transform.renderer.materials[this.mat_index].color = this.car_color;
	}

	public Color car_color;

	public Material car_main_color;

	private Renderer car_renderer;

	private int mat_index;
}
