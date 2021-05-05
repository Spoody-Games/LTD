using UnityEngine;
using UnityEditor;
//using CodeStage.AntiCheat.ObscuredTypes;

public class ClearPrefs
{
	[MenuItem("Tools/Clear PlayerPrefs")]
	private static void NewMenuOption()
	{
		//ObscuredPrefs.DeleteAll();
		PlayerPrefs.DeleteAll();
	}
}