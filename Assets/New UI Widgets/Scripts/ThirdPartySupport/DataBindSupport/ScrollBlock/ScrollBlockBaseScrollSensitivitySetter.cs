#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ScrollSensitivity of a ScrollBlockBase depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase ScrollSensitivity Setter")]
	public class ScrollBlockBaseScrollSensitivitySetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, System.Single value)
		{
			target.ScrollSensitivity = value;
		}
	}
}
#endif