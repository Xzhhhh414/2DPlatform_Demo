#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SnapToSpacing of a SnapGrid depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapGrid SnapToSpacing Setter")]
	public class SnapGridSnapToSpacingSetter : ComponentSingleSetter<UIWidgets.SnapGrid, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapGrid target, System.Boolean value)
		{
			target.SnapToSpacing = value;
		}
	}
}
#endif