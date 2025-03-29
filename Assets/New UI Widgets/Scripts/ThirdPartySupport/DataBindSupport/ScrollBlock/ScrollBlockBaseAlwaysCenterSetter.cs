#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the AlwaysCenter of a ScrollBlockBase depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase AlwaysCenter Setter")]
	public class ScrollBlockBaseAlwaysCenterSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Boolean value)
		{
			target.AlwaysCenter = value;
		}
	}
}
#endif