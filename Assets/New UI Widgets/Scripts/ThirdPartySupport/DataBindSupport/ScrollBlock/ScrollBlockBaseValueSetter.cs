#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a ScrollBlockBase depending on the System.Func<System.Int32,System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase Value Setter")]
	public class ScrollBlockBaseValueSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Func<System.Int32,System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Func<System.Int32,System.String> value)
		{
			target.Value = value;
		}
	}
}
#endif