namespace UIThemes
{
	using System;
	using System.Collections.Generic;
	using UIThemes.Pool;
	using UIThemes.Wrappers;
	using UnityEngine;
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

	/// <summary>
	/// Base class for the ThemeTarget.
	/// </summary>
	public abstract partial class ThemeTargetBase : MonoBehaviour, IThemeTarget
	{
		static ThemeTargetBase() => Utilities.InvokeStaticMethods<PropertiesRegistryAttribute>();

		/// <summary>
		/// Static init.
		/// Empty method to ensure static constructor was called.
		/// </summary>
		public static void StaticInit()
		{
		}

		/// <summary>
		/// Type of the Theme.
		/// </summary>
		public abstract Type ThemeType
		{
			get;
		}

		/// <summary>
		/// Colors.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.Colors))]
		protected List<Target> colors = new List<Target>();

		/// <summary>
		/// Colors.
		/// </summary>
		public IReadOnlyList<Target> Colors => colors;

		/// <summary>
		/// Color multipliers.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.ColorMultipliers))]
		protected List<Target> colorMultipliers = new List<Target>();

		/// <summary>
		/// Colors multipliers.
		/// </summary>
		public IReadOnlyList<Target> ColorMultipliers => colorMultipliers;

		/// <summary>
		/// Sprites.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.Sprites))]
		protected List<Target> sprites = new List<Target>();

		/// <summary>
		/// Sprites.
		/// </summary>
		public IReadOnlyList<Target> Sprites => sprites;

		/// <summary>
		/// Textures.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.Textures))]
		protected List<Target> textures = new List<Target>();

		/// <summary>
		/// Textures.
		/// </summary>
		public IReadOnlyList<Target> Textures => textures;

		/// <summary>
		/// Fonts.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.Fonts))]
		protected List<Target> fonts = new List<Target>();

		/// <summary>
		/// Fonts.
		/// </summary>
		public IReadOnlyList<Target> Fonts => fonts;

		/// <summary>
		/// TMPro fonts.
		/// </summary>
		[SerializeField]
		#if UIWIDGETS_TMPRO_SUPPORT
		[ThemeProperty(nameof(Theme.FontsTMPro))]
		#endif
		protected List<Target> fontsTMPro = new List<Target>();

		/// <summary>
		/// TMPro fonts.
		/// </summary>
		public IReadOnlyList<Target> FontsTMPro => fontsTMPro;

		/// <summary>
		/// Font sizes.
		/// </summary>
		[SerializeField]
		[ThemeProperty(nameof(Theme.FontSizes))]
		protected List<Target> fontSizes = new List<Target>();

		/// <summary>
		/// Font sizes.
		/// </summary>
		public IReadOnlyList<Target> FontSizes => fontSizes;

		bool isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			InitOnce();
		}

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected abstract void InitOnce();

		/// <summary>
		/// Get theme.
		/// </summary>
		/// <returns>Theme.</returns>
		public abstract Theme GetTheme();

		/// <summary>
		/// Set theme.
		/// </summary>
		/// <param name="theme">Theme.</param>
		public abstract void SetTheme(Theme theme);

		/// <summary>
		/// Refresh.
		/// </summary>
		public virtual void Refresh()
		{
			var theme = GetTheme();
			if (theme == null)
			{
				return;
			}

			ThemeChanged(theme.ActiveVariationId);
		}

		/// <summary>
		/// Is theme valid and has specified variation ID.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <returns>true if theme valid and has specified variation ID; otherwise false.</returns>
		protected virtual bool IsValidTheme(VariationId variationId)
		{
			var theme = GetTheme();
			if (theme == null)
			{
				return false;
			}

			if (!theme.HasVariation(variationId))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Process theme changes.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		protected virtual void ThemeChanged(VariationId variationId)
		{
			if (!IsValidTheme(variationId))
			{
				return;
			}

			var theme = GetTheme();
			SetValue(variationId, theme.Colors, colors);
			SetValue(variationId, theme.ColorMultipliers, colorMultipliers);
			SetValue(variationId, theme.Sprites, sprites);
			SetValue(variationId, theme.Textures, textures);
			SetValue(variationId, theme.Fonts, fonts);

			#if UIWIDGETS_TMPRO_SUPPORT
			SetValue(variationId, theme.FontsTMPro, fontsTMPro);
			#endif

			SetValue(variationId, theme.FontSizes, fontSizes);
		}

		/// <summary>
		/// Set values.
		/// </summary>
		/// <typeparam name="TValue">Type of value.</typeparam>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="values">Values wrapper.</param>
		/// <param name="targets">Targets.</param>
		protected virtual void SetValue<TValue>(VariationId variationId, Theme.ValuesWrapper<TValue> values, List<Target> targets)
		{
			foreach (var target in targets)
			{
				if (!target.Active)
				{
					continue;
				}

				var wrapper = PropertyWrappers<TValue>.Get(target);
				if (wrapper == null)
				{
					continue;
				}

				if (!wrapper.Active(target.Component))
				{
					continue;
				}

				if (!values.HasOption(target.OptionId))
				{
					continue;
				}

				if ((wrapper is IWrapperValidation wa) && !wa.IsValidType(target.Component))
				{
					var message = string.Format(
						"[ThemeTarget] Type of property/field \"{0}.{1}\" was changed, and it cannot be controlled by Theme.",
						target.Component.GetType(),
						target.Property);
					Debug.LogError(message);
					continue;
				}

				var value = values.Get(variationId, target.OptionId);
				var changed = wrapper.Set(target.Component, value, values.Comparer);
				#if UNITY_EDITOR
				if (changed && !Application.isPlaying)
				{
					UtilitiesEditor.MarkDirty(target.Component);
				}
				#endif
			}
		}

		/// <summary>
		/// Is target invalid?
		/// </summary>
		/// <typeparam name="TValue">Type of value.</typeparam>
		/// <param name="target">Target.</param>
		/// <returns>true if target is invalid; otherwise false.</returns>
		protected virtual bool InvalidTarget<TValue>(Target target)
		{
			if (ReferenceEquals(target.Component, null))
			{
				return true;
			}

			if (target.Component == null)
			{
				return false;
			}

			return !PropertyWrappers<TValue>.Has(target.Component.GetType(), target.Property);
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Find targets.
		/// </summary>
		public void FindTargets()
		{
			using var _ = ListPool<Component>.Get(out var temp);

			gameObject.GetComponents(temp);
			FindTargets(temp);
		}

		/// <summary>
		/// Find targets.
		/// </summary>
		/// <param name="components">Components.</param>
		public virtual void FindTargets(List<Component> components)
		{
			var excluded_properties = ExclusionList.Get(components);
			FindTargets(components, excluded_properties);
			excluded_properties.Return();
		}

		/// <summary>
		/// Is property active?
		/// </summary>
		/// <param name="name">Property name.</param>
		/// <returns>true if property active; otherwise false.</returns>
		protected virtual bool IsActiveProperty(string name)
		{
			var theme = GetTheme();
			if (theme == null)
			{
				return true;
			}

			return theme.IsActiveProperty(name);
		}

		/// <summary>
		/// Find targets.
		/// </summary>
		/// <param name="components">Components.</param>
		/// <param name="excludedProperties">Excluded properties.</param>
		protected virtual void FindTargets(List<Component> components, ExclusionList excludedProperties)
		{
			if (IsActiveProperty(nameof(Theme.Colors)))
			{
				FindTargets<Color>(components, colors, excludedProperties);
			}

			if (IsActiveProperty(nameof(Theme.ColorMultipliers)))
			{
				FindTargets<ColorMultiplierValue>(components, colorMultipliers, excludedProperties);
			}

			if (IsActiveProperty(nameof(Theme.Sprites)))
			{
				FindTargets<Sprite>(components, sprites, excludedProperties);
			}

			if (IsActiveProperty(nameof(Theme.Textures)))
			{
				FindTargets<Texture>(components, textures, excludedProperties);
			}

			if (IsActiveProperty(nameof(Theme.Fonts)))
			{
				FindTargets<Font>(components, fonts, excludedProperties);
			}

			#if UIWIDGETS_TMPRO_SUPPORT
			if (IsActiveProperty(nameof(Theme.FontsTMPro)))
			{
				FindTargets<FontAsset>(components, fontsTMPro, excludedProperties);
			}
			#endif

			if (IsActiveProperty(nameof(Theme.FontSizes)))
			{
				FindTargets<FontSizeValue>(components, fontSizes, excludedProperties);
			}
		}

		/// <summary>
		/// Find targets.
		/// </summary>
		/// <typeparam name="TValue">Type of value.</typeparam>
		/// <param name="components">Components.</param>
		/// <param name="targets">Targets.</param>
		/// <param name="excludedProperties">Excluded properties.</param>
		/// <returns>Targets count.</returns>
		protected virtual int FindTargets<TValue>(List<Component> components, List<Target> targets, ExclusionList excludedProperties)
		{
			targets.RemoveAll(InvalidTarget<TValue>);

			var wrapper = new TargetsWrapper(targets, excludedProperties);
			foreach (var component in components)
			{
				PropertyWrappers<TValue>.Find(component, wrapper);
			}

			return targets.Count;
		}

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			// FindTargets();
			// Refresh();
		}
		#endif

		/// <summary>
		/// Set owner of the property.
		/// </summary>
		/// <typeparam name="TComponent">Type of component.</typeparam>
		/// <param name="propertyType">Type of property.</param>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <param name="owner">Owner.</param>
		public virtual void SetPropertyOwner<TComponent>(Type propertyType, TComponent component, string property, Component owner)
			where TComponent : Component
		{
			if (propertyType == typeof(Color))
			{
				SetPropertyOwner(Colors, component, property, owner);
			}
			else if (propertyType == typeof(Sprite))
			{
				SetPropertyOwner(Sprites, component, property, owner);
			}
			else if (propertyType == typeof(Texture))
			{
				SetPropertyOwner(Textures, component, property, owner);
			}
			else if (propertyType == typeof(Font))
			{
				SetPropertyOwner(Fonts, component, property, owner);
			}
			#if UIWIDGETS_TMPRO_SUPPORT
			else if (propertyType == typeof(FontAsset))
			{
				SetPropertyOwner(FontsTMPro, component, property, owner);
			}
			#endif
		}

		/// <summary>
		/// Set owner of the property.
		/// </summary>
		/// <typeparam name="TComponent">Type of component.</typeparam>
		/// <param name="options">Options.</param>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <param name="owner">Owner.</param>
		protected virtual void SetPropertyOwner<TComponent>(IReadOnlyList<Target> options, TComponent component, string property, Component owner)
			where TComponent : Component
		{
			// WARNING: will not work correctly if component has two fields with the same name (one declared as "new")
			foreach (var option in options)
			{
				if (ReferenceEquals(option.Component, component) && (option.Property == property) && !UnityObjectComparer<Component>.Instance.Equals(option.Owner, owner))
				{
					option.Owner = owner;
					#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
					#endif
				}
			}
		}
	}
}