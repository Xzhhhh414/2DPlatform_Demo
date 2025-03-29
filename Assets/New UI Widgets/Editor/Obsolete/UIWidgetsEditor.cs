#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using UnityEditor;

	/// <summary>
	/// Base editor for the UIWidgets components.
	/// </summary>
	[CustomEditor(typeof(UIWidgetsBehaviour), true)]
	[Obsolete("Unused.")]
	public class UIWidgetsEditor : UIWidgetsMonoEditor
	{
	}
}
#endif