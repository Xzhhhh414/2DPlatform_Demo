#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Adjust size for the TMPro InputField.
	/// </summary>
	[RequireComponent(typeof(TMP_InputField))]
	public class TMProInputFieldAdjustSize : LayoutElementAdjustSize<TMP_InputField>
	{
	}
}
#endif