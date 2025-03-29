#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the RotateSpeed of a LoadingAnimation depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] LoadingAnimation RotateSpeed Setter")]
	public class LoadingAnimationRotateSpeedSetter : ComponentSingleSetter<UIWidgets.LoadingAnimation, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.LoadingAnimation target, System.Single value)
		{
			target.RotateSpeed = value;
		}
	}
}
#endif