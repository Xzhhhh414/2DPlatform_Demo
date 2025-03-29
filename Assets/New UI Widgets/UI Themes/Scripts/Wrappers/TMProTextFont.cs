#if UIWIDGETS_TMPRO_SUPPORT
namespace UIThemes.Wrappers
{
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Theme property for the font of TMPro Text.
	/// </summary>
	public class TMProTextFont : Wrapper<FontAsset, TMPro.TMP_Text>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TMProTextFont"/> class.
		/// </summary>
		public TMProTextFont()
		{
			Name = nameof(TMPro.TMP_Text.font);
		}

		/// <inheritdoc/>
		protected override FontAsset Get(TMPro.TMP_Text widget)
		{
			return widget.font;
		}

		/// <inheritdoc/>
		protected override void Set(TMPro.TMP_Text widget, FontAsset value)
		{
			widget.font = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(TMPro.TMP_Text widget)
		{
			return widget.font != null;
		}
	}
}
#endif