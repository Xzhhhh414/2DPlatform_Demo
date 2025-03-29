namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// TreeGraph component.
	/// </summary>
	/// <typeparam name="T">Node type.</typeparam>
	[RequireComponent(typeof(MultipleConnector))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/treegraph.html")]
	public abstract class TreeGraphComponent<T> : MonoBehaviourInitiable, IStylable
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with 'foregrounds'.")]
		protected Graphic[] graphicsForeground = Compatibility.EmptyArray<Graphic>();

		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("graphicsForeground")]
		protected List<Graphic> foregrounds = new List<Graphic>();

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		[Obsolete("Replaced with 'Foregrounds'.")]
		public virtual Graphic[] GraphicsForeground
		{
			get
			{
				GraphicsForegroundInit();

				return graphicsForeground;
			}
		}

		/// <summary>
		/// Gets foreground graphics for coloring.
		/// </summary>
		public virtual List<Graphic> Foregrounds
		{
			get
			{
				GraphicsForegroundInit();

				return foregrounds;
			}
		}

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with 'backgrounds'.")]
		protected Graphic[] graphicsBackground = Compatibility.EmptyArray<Graphic>();

		/// <summary>
		/// Background graphics for coloring.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("graphicsForeground")]
		protected List<Graphic> backgrounds = new List<Graphic>();

		/// <summary>
		/// Get background graphics for coloring.
		/// </summary>
		[Obsolete("Replaced with 'Backgrounds'.")]
		public virtual Graphic[] GraphicsBackground
		{
			get
			{
				GraphicsBackgroundInit();

				return graphicsBackground;
			}
		}

		/// <summary>
		/// Get background graphics for coloring.
		/// </summary>
		public virtual List<Graphic> Backgrounds
		{
			get
			{
				GraphicsBackgroundInit();

				return backgrounds;
			}
		}

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Node.
		/// </summary>
		protected TreeNode<T> Node;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (Background == null)
			{
				var bg = transform.Find("Background");
				if (bg != null)
				{
					bg.TryGetComponent(out Background);
				}
			}
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public abstract void SetData(TreeNode<T> node);

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public virtual void MovedToCache()
		{
		}

		/// <summary>
		/// Toggle node visibility.
		/// </summary>
		public virtual void ToggleVisibility()
		{
			Node.IsVisible = !Node.IsVisible;
		}

		/// <summary>
		/// Toggle expanded.
		/// </summary>
		public virtual void ToggleExpanded()
		{
			Node.IsExpanded = !Node.IsExpanded;
		}

		/// <summary>
		/// Graphics background version.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected byte GraphicsBackgroundVersion = 0;

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected virtual void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsBackground = new Graphic[] { Background };
				#pragma warning restore
				GraphicsBackgroundVersion = 1;
			}

			if (GraphicsBackgroundVersion == 1)
			{
				if (backgrounds.Count == 0)
				{
					#pragma warning disable 0618
					backgrounds.AddRange(graphicsBackground);
					#pragma warning restore
				}

				GraphicsBackgroundVersion = 2;
			}
		}

		/// <summary>
		/// Graphics foreground version.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected byte GraphicsForegroundVersion = 0;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected virtual void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				if (foregrounds.Count == 0)
				{
					#pragma warning disable 0618
					foregrounds.AddRange(graphicsForeground);
					#pragma warning restore
				}

				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Set theme properties owner.
		/// </summary>
		/// <param name="owner">Owner.</param>
		public virtual void SetThemePropertyOwner(Component owner)
		{
			SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Color), Foregrounds, nameof(Graphic.color), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Backgrounds, nameof(Graphic.color), owner);
		}

		/// <summary>
		/// Set only images properties owner.
		/// </summary>
		/// <param name="owner">Owner.</param>
		public virtual void SetThemeImagesPropertiesOwner(Component owner)
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			GraphicsBackgroundInit();
			GraphicsForegroundInit();

			Compatibility.MarkDirty(this);
		}
#endif

		#region IStylable implementation

		/// <summary>
		/// Set the style.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.ApplyTo(Background);

			foreach (var gf in Foregrounds)
			{
				if (gf != null)
				{
					styleText.ApplyTo(gf.gameObject);
				}
			}

			if (TryGetComponent<MultipleConnector>(out var connector))
			{
				connector.SetStyle(style);
			}
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);

			return true;
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleBackground">Style for the background.</param>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			styleBackground.GetFrom(Background);

			foreach (var gf in Foregrounds)
			{
				if (gf != null)
				{
					styleText.GetFrom(gf.gameObject);
				}
			}

			if (TryGetComponent<MultipleConnector>(out var connector))
			{
				connector.GetStyle(style);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public virtual bool GetStyle(Style style)
		{
			GetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);

			return true;
		}
		#endregion
	}
}