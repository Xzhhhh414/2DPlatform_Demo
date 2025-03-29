#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Step of a SnapGrid depending on the UnityEngine.Vector2 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapGrid Step Setter")]
	public class SnapGridStepSetter : ComponentSingleSetter<UIWidgets.SnapGrid, UnityEngine.Vector2>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapGrid target, UnityEngine.Vector2 value)
		{
			target.Step = value;
		}
	}
}
#endif