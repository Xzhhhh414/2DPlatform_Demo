namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// ScrollRectPage with name.
	/// </summary>
	public class ScrollRectPageWithName : ScrollRectPage
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Names.
		/// </summary>
		[SerializeField]
		public List<string> Names;

		/// <inheritdoc/>
		public override void SetPage(int page)
		{
			base.SetPage(page);

			if (Name != null)
			{
				Name.text = page < Names.Count ? Names[page] : "Unnamed: " + page.ToString();
			}
		}
	}
}