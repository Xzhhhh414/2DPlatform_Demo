namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UIWidgets.Attributes;

	/// <summary>
	/// ListViewUnderline sample.
	/// </summary>
	public class ListViewUnderlineSample : ListViewCustom<ListViewUnderlineSampleComponent, ListViewUnderlineSampleItemDescription>
	{
		[DomainReloadExclude]
		static readonly Comparison<ListViewUnderlineSampleItemDescription> ItemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();
			DataSource.Comparison = ItemsComparison;
		}
	}
}