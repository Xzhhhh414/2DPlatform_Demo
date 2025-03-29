namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout element data.
	/// Used as replacement for the LayoutUtility.Get[X]Width() and Get[X]Height().
	/// </summary>
	public readonly struct LayoutElementData
	{
		/// <summary>
		/// Min width.
		/// </summary>
		public readonly float MinWidth;

		/// <summary>
		/// Min height.
		/// </summary>
		public readonly float MinHeight;

		/// <summary>
		/// Min size.
		/// </summary>
		public readonly Vector2 MinSize => new Vector2(MinWidth, MinHeight);

		/// <summary>
		/// Preferred width.
		/// </summary>
		public readonly float PreferredWidth;

		/// <summary>
		/// Preferred height.
		/// </summary>
		public readonly float PreferredHeight;

		/// <summary>
		/// Preferred size.
		/// </summary>
		public readonly Vector2 PreferredSize => new Vector2(PreferredWidth, PreferredHeight);

		/// <summary>
		/// Flexible width.
		/// </summary>
		public readonly float FlexibleWidth;

		/// <summary>
		/// Flexible height.
		/// </summary>
		public readonly float FlexibleHeight;

		/// <summary>
		/// Flexible size.
		/// </summary>
		public readonly Vector2 FlexibleSize => new Vector2(FlexibleWidth, FlexibleHeight);

		[DomainReloadExclude]
		static readonly Comparison<ILayoutElement> Comparison = (a, b) => a.layoutPriority.CompareTo(b.layoutPriority);

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutElementData"/> struct.
		/// </summary>
		/// <param name="component">Component.</param>
		public LayoutElementData(Component component)
		{
			MinWidth = -1f;
			MinHeight = -1f;
			PreferredWidth = -1f;
			PreferredHeight = -1f;
			FlexibleWidth = -1f;
			FlexibleHeight = -1f;

			using var _ = ListPool<ILayoutElement>.Get(out var temp);

			component.GetComponents(temp);
			temp.Sort(Comparison);

			var priority = int.MinValue;
			List<Component> list = ListPool<Component>.Get();
			foreach (var le in temp)
			{
				if (le is ILayoutIgnorer ignore && ignore.ignoreLayout)
				{
					continue;
				}

				var higher_priority = le.layoutPriority > priority;
				if (le.minWidth >= 0f)
				{
					MinWidth = higher_priority ? le.minWidth : Mathf.Max(MinWidth, le.minWidth);
				}

				if (le.minHeight >= 0f)
				{
					MinHeight = higher_priority ? le.minHeight : Mathf.Max(MinHeight, le.minHeight);
				}

				if (le.preferredWidth >= 0f)
				{
					PreferredWidth = higher_priority ? le.preferredWidth : Mathf.Max(PreferredWidth, le.preferredWidth);
				}

				if (le.preferredHeight >= 0f)
				{
					PreferredHeight = higher_priority ? le.preferredHeight : Mathf.Max(PreferredHeight, le.preferredHeight);
				}

				if (le.flexibleWidth >= 0f)
				{
					FlexibleWidth = higher_priority ? le.flexibleWidth : Mathf.Max(FlexibleWidth, le.flexibleWidth);
				}

				if (le.flexibleHeight >= 0f)
				{
					FlexibleHeight = higher_priority ? le.flexibleHeight : Mathf.Max(FlexibleHeight, le.flexibleHeight);
				}

				priority = le.layoutPriority;
			}
		}

		/// <summary>
		/// Set properties for the specified LayoutElement.
		/// </summary>
		/// <param name="le">LayoutElement.</param>
		public readonly void SetTo(LayoutElement le)
		{
			le.minWidth = MinWidth;
			le.minHeight = MinHeight;
			le.preferredWidth = PreferredWidth;
			le.preferredHeight = PreferredHeight;
			le.flexibleWidth = FlexibleWidth;
			le.flexibleHeight = FlexibleHeight;
		}
	}
}