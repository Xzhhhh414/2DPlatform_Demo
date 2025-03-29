namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Adjust size for the ILayoutElement of the specified type.
	/// </summary>
	/// <typeparam name="T">Type of the component.</typeparam>
	public abstract class LayoutElementAdjustSize<T> : UIBehaviourInitiable, ILayoutElement
		where T : UnityEngine.Object, ILayoutElement
	{
		T source;

		/// <summary>
		/// Source.
		/// </summary>
		protected T Source
		{
			get
			{
				Init();

				return source;
			}
		}

		[SerializeField]
		Vector2 sizeDelta = Vector2.zero;

		/// <summary>
		/// Size delta.
		/// </summary>
		public Vector2 SizeDelta
		{
			get => sizeDelta;

			set
			{
				if (sizeDelta != value)
				{
					sizeDelta = value;
					SetDirty();
				}
			}
		}

		[SerializeField]
		int _layoutPriority = 1;

		/// <summary>
		/// Layout priority.
		/// </summary>
		public int layoutPriority
		{
			get => _layoutPriority;

			set
			{
				if (_layoutPriority != value)
				{
					_layoutPriority = value;
					SetDirty();
				}
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (Utilities.IsNull(source))
			{
				TryGetComponent(out source);
			}
		}

		/// <summary>
		/// Min width.
		/// </summary>
		public float minWidth => Source.minWidth + SizeDelta.x;

		/// <summary>
		/// Preferred width.
		/// </summary>
		public float preferredWidth => Source.preferredWidth + SizeDelta.x;

		/// <summary>
		/// Flexible width.
		/// </summary>
		public float flexibleWidth => -1;

		/// <summary>
		/// Min height.
		/// </summary>
		public float minHeight => Source.minHeight + SizeDelta.y;

		/// <summary>
		/// Preferred height.
		/// </summary>
		public float preferredHeight => Source.preferredHeight + SizeDelta.y;

		/// <summary>
		/// Flexible height.
		/// </summary>
		public float flexibleHeight => -1;

		/// <summary>
		/// Calculate layout input horizontal.
		/// </summary>
		public void CalculateLayoutInputHorizontal()
		{
		}

		/// <summary>
		/// Calculate layout input vertical.
		/// </summary>
		public void CalculateLayoutInputVertical()
		{
		}

		/// <summary>
		/// Mark this instance as dirty.
		/// </summary>
		protected virtual void SetDirty()
		{
			LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		}

#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			SetDirty();
		}
#endif
	}
}