namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListViewString.
	/// </summary>
	public class ListViewString : ListViewCustom<ListViewStringItemComponent, string>
	{
		/// <summary>
		/// Enable sort.
		/// </summary>
		[SerializeField]
		protected bool EnableSort = false;

		/// <summary>
		/// Items comparison.
		/// </summary>
		public Comparison<string> ItemsComparison = (x, y) => UtilitiesCompare.Compare(x, y);

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (EnableSort && DataSource.Comparison == null)
			{
				DataSource.Comparison = ItemsComparison;
			}
		}
	}
}