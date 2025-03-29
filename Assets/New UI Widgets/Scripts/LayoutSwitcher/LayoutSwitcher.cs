namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;

	/// <summary>
	/// Layout switcher.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/Layout/Layout Switcher")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/layout/layout-switcher.html")]
	public class LayoutSwitcher : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// The tracked objects.
		/// </summary>
		[SerializeField]
		protected List<RectTransform> Objects = new List<RectTransform>();

		/// <summary>
		/// The layouts.
		/// </summary>
		[SerializeField]
		public List<UILayout> Layouts = new List<UILayout>();

		/// <summary>
		/// Layout changed event.
		/// </summary>
		[SerializeField]
		public LayoutSwitcherEvent LayoutChanged = new LayoutSwitcherEvent();

		/// <summary>
		/// The default display size.
		/// </summary>
		[SerializeField]
		[Tooltip("Display size used when actual display size cannot be detected.")]
		public float DefaultDisplaySize;

		/// <summary>
		/// Window width.
		/// </summary>
		protected int WindowWidth;

		/// <summary>
		/// Window height.
		/// </summary>
		protected int WindowHeight;

		/// <summary>
		/// Function to select layout.
		/// </summary>
		protected Func<List<UILayout>, float, float, UILayout> layoutSelector = DefaultLayoutSelector;

		/// <summary>
		/// Function to select layout.
		/// </summary>
		public virtual Func<List<UILayout>, float, float, UILayout> LayoutSelector
		{
			get => layoutSelector;

			set
			{
				layoutSelector = value;

				ResolutionChanged();
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				Updater.Add(this);
			}
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (Application.isPlaying)
			{
				Updater.Remove(this);
			}
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (WindowWidth != Screen.width || WindowHeight != Screen.height)
			{
				WindowWidth = Screen.width;
				WindowHeight = Screen.height;
				ResolutionChanged();
			}
		}

		/// <summary>
		/// Saves the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public virtual void SaveLayout(UILayout layout) => layout.Save(Objects);

		/// <summary>
		/// Load layout when resolution changed.
		/// </summary>
		public virtual void ResolutionChanged()
		{
			var currentLayout = GetCurrentLayout();
			if (currentLayout == null)
			{
				return;
			}

			currentLayout.Load();
			LayoutChanged.Invoke(currentLayout);
		}

		/// <summary>
		/// Gets the current layout.
		/// </summary>
		/// <returns>The current layout.</returns>
		public virtual UILayout GetCurrentLayout()
		{
			if (Layouts.Count == 0)
			{
				return null;
			}

			return LayoutSelector(Layouts, DisplaySize(), AspectRatio());
		}

		/// <summary>
		/// Default layout select.
		/// </summary>
		/// <param name="layouts">Available layouts.</param>
		/// <param name="displaySize">Display size in inches.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Layout to use.</returns>
		public static UILayout DefaultLayoutSelector(List<UILayout> layouts, float displaySize, float aspectRatio)
		{
			using var _ = ListPool<UILayout>.Get(out var temp);
			FilterLayouts(layouts, displaySize, aspectRatio, temp);

			if (temp.Count == 0)
			{
				return null;
			}

			for (int i = 0; i < temp.Count; i++)
			{
				if (displaySize < temp[i].MaxDisplaySize)
				{
					return temp[i];
				}
			}

			return temp[0];
		}

		/// <summary>
		/// Filter layouts by aspect ratio and display size.
		/// </summary>
		/// <param name="layouts">Layouts.</param>
		/// <param name="displaySize">Display size.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <param name="output">Filtered layouts.</param>
		protected static void FilterLayouts(List<UILayout> layouts, float displaySize, float aspectRatio, List<UILayout> output)
		{
			for (int i = 0; i < layouts.Count; i++)
			{
				var layout = layouts[i];

				var diff = Mathf.Abs(aspectRatio - (layout.AspectRatio.x / layout.AspectRatio.y));

				if (diff < 0.05f)
				{
					output.Add(layout);
				}
			}

			output.Sort(new DisplaySizeComparer(displaySize).Compare);
		}

		/// <summary>
		/// Display size comparer.
		/// </summary>
		protected class DisplaySizeComparer : IEquatable<DisplaySizeComparer>
		{
			readonly float DisplaySize;

			/// <summary>
			/// Initializes a new instance of the <see cref="DisplaySizeComparer"/> class.
			/// </summary>
			/// <param name="displaySize">Display size.</param>
			public DisplaySizeComparer(float displaySize) => DisplaySize = displaySize;

			/// <summary>
			/// Compare layouts by display size.
			/// </summary>
			/// <param name="x">First layout.</param>
			/// <param name="y">Second layout.</param>
			/// <returns>Result of the comparison.</returns>
			public int Compare(UILayout x, UILayout y)
			{
				var x_ds = Mathf.Abs(x.MaxDisplaySize - DisplaySize);
				var y_ds = Mathf.Abs(y.MaxDisplaySize - DisplaySize);

				return x_ds.CompareTo(y_ds);
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is DisplaySizeComparer comparer) && Equals(comparer);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(DisplaySizeComparer other)
			{
				if (other is null)
				{
					return false;
				}

				return DisplaySize == other.DisplaySize;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => DisplaySize.GetHashCode();

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(DisplaySizeComparer left, DisplaySizeComparer right)
			{
				if (left is null)
				{
					return right is null;
				}

				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(DisplaySizeComparer left, DisplaySizeComparer right) => !(left == right);
		}

		/// <summary>
		/// Current aspect ratio.
		/// </summary>
		/// <returns>The ratio.</returns>
		public virtual float AspectRatio() => (float)WindowWidth / (float)WindowHeight;

		/// <summary>
		/// Current display size.
		/// </summary>
		/// <returns>The size.</returns>
		public virtual float DisplaySize()
		{
			if (Screen.dpi == 0)
			{
				return DefaultDisplaySize;
			}

			return Mathf.Sqrt(WindowWidth ^ 2 + WindowHeight ^ 2) / Screen.dpi;
		}
	}
}