#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ValueMax of a LoadingAnimation depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] LoadingAnimation ValueMax Setter")]
	public class LoadingAnimationValueMaxSetter : ComponentSingleSetter<UIWidgets.LoadingAnimation, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.LoadingAnimation target, System.Int32 value)
		{
			target.ValueMax = value;
		}
	}
}
#endif