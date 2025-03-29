#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Increase of a ScrollBlockBase depending on the System.Action data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase Increase Setter")]
	public class ScrollBlockBaseIncreaseSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Action>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Action value)
		{
			target.Increase = value;
		}
	}
}
#endif