namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviourInteractable,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable, UIThemes.ITargetOwner
	{
		/// <summary>
		/// Scroll data for the ListViewBase.
		/// </summary>
		protected abstract class ListViewBaseScrollData
		{
			/// <summary>
			/// Restore scroll position after items was changed.
			/// </summary>
			public bool Restore = false;

			/// <summary>
			/// Get ScrollRect ContentStartPosition.
			/// </summary>
			[DomainReloadExclude]
			protected static readonly Func<ScrollRect, Vector2> GetContentStartPosition;

			/// <summary>
			/// Set ScrollRect ContentStartPosition.
			/// </summary>
			[DomainReloadExclude]
			protected static readonly Action<ScrollRect, Vector2> SetContentStartPosition;

			/// <summary>
			/// Displayed indices.
			/// </summary>
			protected readonly List<int> DisplayedIndices = new List<int>();

			/// <summary>
			/// Scroll delta.
			/// </summary>
			protected readonly List<float?> ItemsPositions = new List<float?>();

			/// <summary>
			/// Scroll position.
			/// </summary>
			protected float ScrollPosition;

			/// <summary>
			/// Retain scroll position.
			/// </summary>
			protected abstract bool RetainScrollPosition
			{
				get;
			}

			/// <summary>
			/// ListView.
			/// </summary>
			protected abstract ListViewBase ListView
			{
				get;
			}

			/// <summary>
			/// Margin.
			/// </summary>
			protected abstract float Margin
			{
				get;
			}

			/// <summary>
			/// ScrollRect ContentStartPosition.
			/// </summary>
			protected Vector2 ContentStartPosition
			{
				get => GetContentStartPosition(ListView.GetScrollRect());

				set => SetContentStartPosition(ListView.GetScrollRect(), value);
			}

			static ListViewBaseScrollData()
			{
				var type = typeof(ScrollRect);
				var content_start = type.GetField("m_ContentStartPosition", BindingFlags.NonPublic | BindingFlags.Instance);
				GetContentStartPosition = sr => (Vector2)content_start.GetValue(sr);
				SetContentStartPosition = (sr, value) => content_start.SetValue(sr, value);
			}

			/// <summary>
			/// Update scroll rect to prevent ScrollPosition jump during drag.
			/// </summary>
			/// <param name="delta">Delta.</param>
			public void UpdateScrollRect(float delta)
			{
				var c = ContentStartPosition;

				if (ListView.IsHorizontal())
				{
					c.x -= delta;
				}
				else
				{
					c.y += delta;
				}

				ContentStartPosition = c;
			}

			/// <summary>
			/// Update scroll position.
			/// </summary>
			public void UpdateScrollPosition()
			{
				ScrollPosition = ListView.GetScrollPosition();
			}

			/// <summary>
			/// Update.
			/// </summary>
			/// <param name="indices">Indices.</param>
			public abstract void Update(List<int> indices);
		}
	}
}