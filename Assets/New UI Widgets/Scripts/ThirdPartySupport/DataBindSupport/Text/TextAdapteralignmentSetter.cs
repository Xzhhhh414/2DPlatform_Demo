#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the alignment of a TextAdapter depending on the UnityEngine.TextAnchor data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TextAdapter alignment Setter")]
	public class TextAdapteralignmentSetter : ComponentSingleSetter<UIWidgets.TextAdapter, UnityEngine.TextAnchor>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TextAdapter target, UnityEngine.TextAnchor value)
		{
			target.alignment = value;
		}
	}
}
#endif