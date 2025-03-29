#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a SpinnerVector3 depending on the UnityEngine.Vector3 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SpinnerVector3 Value Setter")]
	public class SpinnerVector3ValueSetter : ComponentSingleSetter<UIWidgets.SpinnerVector3, UnityEngine.Vector3>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SpinnerVector3 target, UnityEngine.Vector3 value)
		{
			target.Value = value;
		}
	}
}
#endif