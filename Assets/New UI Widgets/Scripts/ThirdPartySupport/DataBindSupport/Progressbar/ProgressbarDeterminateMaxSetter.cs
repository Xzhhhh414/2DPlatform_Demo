#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Max of a ProgressbarDeterminate depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ProgressbarDeterminate Max Setter")]
	public class ProgressbarDeterminateMaxSetter : ComponentSingleSetter<UIWidgets.ProgressbarDeterminate, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ProgressbarDeterminate target, System.Int32 value)
		{
			target.Max = value;
		}
	}
}
#endif