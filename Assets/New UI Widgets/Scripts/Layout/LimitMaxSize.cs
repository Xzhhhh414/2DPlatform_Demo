namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Limits RectTransform width or height if its size is relative to the parent.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/layout/limit-maxsize.html")]
	public class LimitMaxSize : UIBehaviourConditional
	{
		/// <summary>
		/// Limit width.
		/// </summary>
		[SerializeField]
		public bool LimitWidth;

		/// <summary>
		/// Maximum allowed width.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(LimitWidth))]
		public float MaxWidth;

		/// <summary>
		/// Limit height.
		/// </summary>
		[SerializeField]
		public bool LimitHeight;

		/// <summary>
		/// Maximum allowed height.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(LimitHeight))]
		public float MaxHeight;

		/// <summary>
		/// Own RectTransform.
		/// </summary>
		protected RectTransform OwnRectTransform;

		/// <summary>
		/// Process the dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			if (OwnRectTransform == null)
			{
				OwnRectTransform = transform as RectTransform;
			}

			var parent = OwnRectTransform.parent as RectTransform;
			if (parent == null)
			{
				return;
			}

			var parent_size = parent.rect.size;
			var own_size = OwnRectTransform.rect.size;
			if (LimitWidth)
			{
				var width = Mathf.Min(parent_size.x, MaxWidth);
				if (width != own_size.x)
				{
					OwnRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				}
			}

			if (LimitHeight)
			{
				var height = Mathf.Min(parent_size.y, MaxHeight);
				if (height != own_size.y)
				{
					OwnRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}

			base.OnRectTransformDimensionsChange();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			if (LimitWidth && (MaxWidth == 0))
			{
				MaxWidth = (transform as RectTransform).rect.width;
			}

			if (LimitHeight && (MaxHeight == 0))
			{
				MaxHeight = (transform as RectTransform).rect.height;
			}

			base.OnValidate();
		}
#endif
	}
}