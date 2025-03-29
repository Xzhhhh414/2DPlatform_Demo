#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the UnscaledTime of a ScrollBlockBase depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase UnscaledTime Setter")]
	public class ScrollBlockBaseUnscaledTimeSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Boolean value)
		{
			target.UnscaledTime = value;
		}
	}
}
#endif