namespace UIThemes
{
	using UIThemes.Wrappers;
	using UnityEngine;
	using UnityEngine.Scripting;
	using UnityEngine.UI;
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Properties registry.
	/// </summary>
	public static class PropertyWrappersRegistry
	{
		/// <summary>
		/// Add properties.
		/// </summary>
		[PropertiesRegistry]
		[Preserve]
		public static void AddWrappers()
		{
			// base
			PropertyWrappers<Color>.Add(new GraphicColor());
			PropertyWrappers<Sprite>.Add(new ImageSprite());
			PropertyWrappers<Texture>.Add(new RawImageTexture());
			PropertyWrappers<Font>.Add(new TextFont());
			PropertyWrappers<Color>.Add(new ShadowEffectColor());
			PropertyWrappers<FontSizeValue>.Add(new TextFontSize());

			// Selectable
			PropertyWrappers<Color>.Add(new SelectableNormalColor());
			PropertyWrappers<Color>.Add(new SelectableHighlightedColor());
			PropertyWrappers<Color>.Add(new SelectablePressedColor());
			PropertyWrappers<Color>.Add(new SelectableSelectedColor());
			PropertyWrappers<Color>.Add(new SelectableDisabledColor());

			PropertyWrappers<Sprite>.Add(new SelectableHighlightedSprite());
			PropertyWrappers<Sprite>.Add(new SelectablePressedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableSelectedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableDisabledSprite());

			PropertyWrappers<ColorMultiplierValue>.Add(new SelectableColorMultiplier());

			// InputField
			PropertyWrappers<Color>.Add(new InputFieldCaretColor());
			PropertyWrappers<Color>.Add(new InputFieldSelectionColor());

			// Ignore
			PropertyWrappers<Sprite>.AddIgnore(typeof(Image), nameof(Image.overrideSprite));
			PropertyWrappers<Texture>.AddIgnore(typeof(Graphic), nameof(Graphic.mainTexture));

			// TMPro
			#if UIWIDGETS_TMPRO_SUPPORT
			PropertyWrappers<FontSizeValue>.Add(new TMProTextFontSize());

			PropertyWrappers<FontAsset>.Add(new TMProTextFont());

			PropertyWrappers<Color>.Add(new TMProInputFieldCaretColor());
			PropertyWrappers<Color>.Add(new TMProInputFieldSelectionColor());
			PropertyWrappers<FontAsset>.Add(new TMProInputFieldFontAsset());

			PropertyWrappers<FontAsset>.AddIgnore(typeof(TMPro.TMP_SubMeshUI), nameof(TMPro.TMP_SubMeshUI.fontAsset));
			#endif
		}
	}
}