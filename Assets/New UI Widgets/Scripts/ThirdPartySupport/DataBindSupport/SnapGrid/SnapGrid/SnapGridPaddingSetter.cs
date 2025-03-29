#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Padding of a SnapGrid depending on the UnityEngine.Vector2 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapGrid Padding Setter")]
	public class SnapGridPaddingSetter : ComponentSingleSetter<UIWidgets.SnapGrid, UnityEngine.Vector2>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapGrid target, UnityEngine.Vector2 value)
		{
			target.Padding = value;
		}
	}
}
#endif