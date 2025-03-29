#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// HexLayoutBuilder editor.
	/// </summary>
	[CustomEditor(typeof(HexLayoutBuilder), true)]
	public class HexLayoutBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (Application.isPlaying && (targets.Length == 1))
			{
				var script = (HexLayoutBuilder)target;

				if (GUILayout.Button("Update Grid"))
				{
					script.CreateGrid();
				}
			}
		}
	}
}
#endif