namespace UIWidgets
{
	using System.Collections.Generic;
	using UIThemes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Highlight accordion.
	/// </summary>
	[RequireComponent(typeof(Accordion))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/containers/accordion.html#accordionhighlight")]
	public class AccordionHighlight : MonoBehaviourInitiable, IStylable, ITargetOwner
	{
		[SerializeField]
		StyleImage defaultToggleBackground;

		/// <summary>
		/// Default background.
		/// </summary>
		public StyleImage DefaultToggleBackground
		{
			get => defaultToggleBackground;

			set
			{
				defaultToggleBackground = value;
				UpdateHighlights();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("defaultText")]
		StyleText defaultToggleText;

		/// <summary>
		/// Default text.
		/// </summary>
		public StyleText DefaultToggleText
		{
			get => defaultToggleText;

			set
			{
				defaultToggleText = value;
				UpdateHighlights();
			}
		}

		[SerializeField]
		StyleImage activeToggleBackground;

		/// <summary>
		/// Active background.
		/// </summary>
		public StyleImage ActiveToggleBackground
		{
			get => activeToggleBackground;

			set
			{
				activeToggleBackground = value;
				UpdateHighlights();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("activeText")]
		StyleText activeToggleText;

		/// <summary>
		/// Active text.
		/// </summary>
		public StyleText ActiveToggleText
		{
			get => activeToggleText;

			set
			{
				activeToggleText = value;
				UpdateHighlights();
			}
		}

		Accordion accordion;

		/// <summary>
		/// Accordion.
		/// </summary>
		protected Accordion Accordion
		{
			get
			{
				if (accordion == null)
				{
					TryGetComponent(out accordion);
				}

				return accordion;
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			SetTargetOwner();

			Accordion.OnStartToggleAnimation.AddListener(OnToggle);
			Accordion.OnToggleItem.AddListener(OnToggle);
			Accordion.OnDataSourceChanged.AddListener(UpdateHighlights);

			UpdateHighlights();
		}

		/// <summary>
		/// Set theme target owner.
		/// </summary>
		public virtual void SetTargetOwner()
		{
			foreach (var item in Accordion)
			{
				if (item.ToggleObject.TryGetComponent<Graphic>(out var object_graphic))
				{
					UIThemes.Utilities.SetTargetOwner(typeof(Color), object_graphic, nameof(Graphic.color), this);
				}

				if ((item.ToggleLabel != null) && item.ToggleLabel.TryGetComponent<Graphic>(out var label_graphic))
				{
					UIThemes.Utilities.SetTargetOwner(typeof(Color), label_graphic, nameof(Graphic.color), this);
				}
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (accordion != null)
			{
				accordion.OnStartToggleAnimation.RemoveListener(OnToggle);
				Accordion.OnToggleItem.RemoveListener(OnToggle);
				accordion.OnDataSourceChanged.RemoveListener(UpdateHighlights);
			}
		}

		/// <summary>
		/// Process the toggle event.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void OnToggle(AccordionItem item)
		{
			UpdateHighlight(item);
		}

		/// <summary>
		/// Update item highlight.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void UpdateHighlight(AccordionItem item)
		{
			if (item.Open)
			{
				ActiveToggleBackground.ApplyTo(item.ToggleObject);
				if (item.ToggleLabel != null)
				{
					ActiveToggleText.ApplyTo(item.ToggleLabel.gameObject);
				}
			}
			else
			{
				DefaultToggleBackground.ApplyTo(item.ToggleObject);
				if (item.ToggleLabel != null)
				{
					DefaultToggleText.ApplyTo(item.ToggleLabel.gameObject);
				}
			}
		}

		/// <summary>
		/// Update highlights of all opened items.
		/// </summary>
		public virtual void UpdateHighlights()
		{
			foreach (var item in Accordion)
			{
				UpdateHighlight(item);
			}
		}

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			defaultToggleBackground = style.Accordion.ToggleDefaultBackground;
			activeToggleBackground = style.Accordion.ToggleActiveBackground;
			defaultToggleText = style.Accordion.ToggleDefaultText;
			activeToggleText = style.Accordion.ToggleActiveText;

			UpdateHighlights();

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Accordion.ToggleDefaultBackground = defaultToggleBackground;
			style.Accordion.ToggleActiveBackground = activeToggleBackground;
			style.Accordion.ToggleDefaultText = defaultToggleText;
			style.Accordion.ToggleActiveText = activeToggleText;

			return true;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			SetTargetOwner();
		}
		#endif
	}
}