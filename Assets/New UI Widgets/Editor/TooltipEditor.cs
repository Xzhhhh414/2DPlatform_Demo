#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Custom editor for the Tooltip.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Tooltip), true)]
	[Obsolete]
	public class TooltipEditor : Editor
	{
		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var tooltip = target as Tooltip;
			if ((tooltip == null) || (tooltip.TooltipObject != null))
			{
				return;
			}

			if (GUILayout.Button("Create Tooltip Object"))
			{
				var go = Widgets.CreateFromPrefab(PrefabsMenu.Instance.Tooltip);
				go.transform.SetParent(tooltip.transform);

				var tooltipRectTransform = go.transform as RectTransform;
				tooltipRectTransform.anchorMin = new Vector2(1, 1);
				tooltipRectTransform.anchorMax = new Vector2(1, 1);
				tooltipRectTransform.pivot = new Vector2(1, 0);
				tooltipRectTransform.anchoredPosition = new Vector2(0, 0);

				Undo.RecordObject(tooltip, "Create Tooltip Object");

				tooltip.TooltipObject = go;

				EditorUtility.SetDirty(tooltip);
			}
		}
	}
}
#endif