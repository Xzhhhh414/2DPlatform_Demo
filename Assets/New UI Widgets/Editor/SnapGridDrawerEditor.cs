#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Custom editor for the SnapGridDrawer.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SnapGridDrawer), true)]
	public class SnapGridDrawerEditor : Editor
	{
		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var drawer = target as SnapGridDrawer;
			if (drawer == null)
			{
				return;
			}

			if (!drawer.TryGetComponent<SnapGridBase>(out var _))
			{
				EditorGUILayout.HelpBox("Requires SnapGrid or SnapLines components.", MessageType.Error);

				if (GUILayout.Button("Add SnapGrid component"))
				{
					Undo.AddComponent<SnapGrid>(drawer.gameObject);
				}

				if (GUILayout.Button("Add SnapLines component"))
				{
					Undo.AddComponent<SnapLines>(drawer.gameObject);
				}
			}
		}
	}
}
#endif