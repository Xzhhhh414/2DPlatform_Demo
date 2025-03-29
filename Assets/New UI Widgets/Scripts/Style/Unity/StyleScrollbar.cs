namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the scrollbar.
	/// </summary>
	[Serializable]
	public class StyleScrollbar : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the main background.
		/// </summary>
		[SerializeField]
		public StyleImage MainBackground;

		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public StyleImage Handle;

		/// <summary>
		/// Apply style for the specified scrollbar.
		/// </summary>
		/// <param name="component">Scrollbar.</param>
		public virtual void ApplyTo(Scrollbar component)
		{
			if (component == null)
			{
				return;
			}

			if (component.TryGetComponent<StyleSupportScrollbar>(out var style_support))
			{
				style_support.SetStyle(this);
			}
			else
			{
				if (component.TryGetComponent<Image>(out var bg))
				{
					MainBackground.ApplyTo(bg);
				}

				Background.ApplyTo(component.transform.Find("Background"));

				Handle.ApplyTo(component.targetGraphic as Image);
			}
		}

		/// <summary>
		/// Set style options from the specified scrollbar.
		/// </summary>
		/// <param name="component">Scrollbar.</param>
		public virtual void GetFrom(Scrollbar component)
		{
			if (component == null)
			{
				return;
			}

			if (component.TryGetComponent<StyleSupportScrollbar>(out var style_support))
			{
				style_support.GetStyle(this);
			}
			else
			{
				if (component.TryGetComponent<Image>(out var bg))
				{
					MainBackground.GetFrom(bg);
				}

				Background.GetFrom(component.transform.Find("Background"));

				Handle.GetFrom(component.targetGraphic as Image);
			}
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			MainBackground.SetDefaultValues();
			Background.SetDefaultValues();
			Handle.SetDefaultValues();
		}
#endif
	}
}