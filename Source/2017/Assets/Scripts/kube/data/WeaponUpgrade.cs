using System;
using LitJson;
using UnityEngine;

namespace kube.data
{
	public class WeaponUpgrade
	{
		public static float[] getUpgradeValue(int weaponId)
		{
			WeaponUpgradeData upgradeData = WeaponUpgrade.getUpgradeData(weaponId);
			upgradeData.upgradeValue[1] = 77.7f * Mathf.Pow(1f / (upgradeData.upgradeValue[1] * 100f), 0.25f);
			upgradeData.upgradeValue[1] = 10f / upgradeData.upgradeValue[2] / 10f;
			return upgradeData.upgradeValue;
		}

		public static WeaponUpgradeData getUpgradeData(int weaponId)
		{
			WeaponParamsObj weaponParamsObj = Kube.IS.weaponParams[weaponId];
			int bulletsType = weaponParamsObj.BulletsType;
			InventoryScript.BulletParams bulletParams = Kube.IS.bulletParams[bulletsType];
			int[] array = new int[]
			{
				weaponParamsObj.currentDamageIndex,
				weaponParamsObj.currentAccuracyIndex,
				weaponParamsObj.currentDeltaShotIndex,
				weaponParamsObj.currentClipSizeIndex,
				bulletParams.initialAmountIndex
			};
			int[] array2 = new int[]
			{
				weaponParamsObj.Damage.Length,
				weaponParamsObj.Accuracy.Length,
				weaponParamsObj.DeltaShotArray.Length,
				weaponParamsObj.clipSize.Length,
				bulletParams.initialAmountArray.Length
			};
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = Math.Min(Kube.GPS.upgradePrice.GetLength(2) - 1, array2[i]);
			}
			float[] array3;
			try
			{
				array3 = new float[]
				{
					weaponParamsObj.Damage[array[0]],
					weaponParamsObj.Accuracy[array[1]],
					weaponParamsObj.DeltaShotArray[array[2]],
					(float)weaponParamsObj.clipSize[array[3]],
					(float)bulletParams.initialAmount
				};
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("bad weapon " + weaponId);
				array3 = new float[]
				{
					weaponParamsObj.Damage[0],
					weaponParamsObj.Accuracy[0],
					weaponParamsObj.DeltaShotArray[0],
					(float)weaponParamsObj.clipSize[0],
					(float)bulletParams.initialAmount
				};
			}
			array3[4] = (float)Kube.IS.bulletParams[Kube.IS.weaponParams[weaponId].BulletsType].initialAmount;
			return new WeaponUpgradeData
			{
				upgradeIndex = array,
				upgradeValue = array3,
				upgradeAvail = array2
			};
		}

		public static void Parse(JsonData wpu)
		{
			if (wpu == null || !wpu.IsObject)
			{
				return;
			}
			for (int i = 0; i < Kube.IS.weaponParams.Length; i++)
			{
				int[] array = new int[4];
				for (int j = 0; j < 4; j++)
				{
					string text = i.ToString() + "_" + j;
					if (wpu.Keys.Contains(text))
					{
						array[j] = (int)wpu[text];
						if ((float)Kube.GPS.inventarWeapons[i] < Time.time)
						{
							Kube.GPS.inventarWeapons[i] = (int)Time.time + 10000000;
						}
					}
				}
				WeaponParamsObj weaponParamsObj = Kube.IS.weaponParams[i];
				Kube.IS.weaponParams[i].currentDamageIndex = Math.Min(array[0], weaponParamsObj.Damage.Length - 1);
				Kube.IS.weaponParams[i].currentAccuracyIndex = Math.Min(array[1], weaponParamsObj.Accuracy.Length - 1);
				Kube.IS.weaponParams[i].currentDeltaShotIndex = Math.Min(array[2], weaponParamsObj.DeltaShotArray.Length - 1);
				Kube.IS.weaponParams[i].currentClipSizeIndex = Math.Min(array[3], weaponParamsObj.clipSize.Length - 1);
				Kube.IS.weaponParams[i].accuarcy = Kube.IS.weaponParams[i].Accuracy[Kube.IS.weaponParams[i].currentAccuracyIndex];
				Kube.IS.weaponParams[i].DeltaShot = Kube.IS.weaponParams[i].DeltaShotArray[Kube.IS.weaponParams[i].currentDeltaShotIndex];
			}
		}
	}
}
