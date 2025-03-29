namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Helper for ScrollRectPaginator to scale centered slide.
	/// </summary>
	public class ScrollRectPaginatorScale : BasePaginatorScale<RectTransform>
	{
		/// <inheritdoc/>
		protected override RectTransform GetInstance(int index)
		{
			var content = Paginator.ScrollRect.content;
			if (index < 0)
			{
				return null;
			}

			if (index >= content.childCount)
			{
				return null;
			}

			return content.GetChild(index) as RectTransform;
		}
	}
}