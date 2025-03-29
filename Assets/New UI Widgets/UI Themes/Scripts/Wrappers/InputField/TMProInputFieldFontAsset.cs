#if UIWIDGETS_TMPRO_SUPPORT
namespace UIThemes.Wrappers
{
	using TMPro;
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Theme property for the font asset of TMP_InputField.
	/// </summary>
	public class TMProInputFieldFontAsset : Wrapper<FontAsset, TMP_InputField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TMProInputFieldFontAsset"/> class.
		/// </summary>
		public TMProInputFieldFontAsset()
		{
			Name = nameof(TMP_InputField.fontAsset);
		}

		/// <inheritdoc/>
		protected override FontAsset Get(TMP_InputField widget)
		{
			return widget.fontAsset;
		}

		/// <inheritdoc/>
		protected override void Set(TMP_InputField widget, FontAsset value)
		{
			widget.fontAsset = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(TMP_InputField widget)
		{
			return widget.fontAsset != null;
		}
	}
}
#endif