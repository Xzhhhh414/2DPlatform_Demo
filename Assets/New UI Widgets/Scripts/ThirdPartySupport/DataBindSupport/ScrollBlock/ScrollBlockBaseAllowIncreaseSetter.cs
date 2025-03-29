#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the AllowIncrease of a ScrollBlockBase depending on the System.Func<System.Boolean> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase AllowIncrease Setter")]
	public class ScrollBlockBaseAllowIncreaseSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Func<System.Boolean>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Func<System.Boolean> value)
		{
			target.AllowIncrease = value;
		}
	}
}
#endif