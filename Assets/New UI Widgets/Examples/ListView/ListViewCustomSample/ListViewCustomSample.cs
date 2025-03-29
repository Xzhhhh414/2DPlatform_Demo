namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UIWidgets.Attributes;

	/// <summary>
	/// ListViewCustom sample.
	/// </summary>
	public class ListViewCustomSample : ListViewCustom<ListViewCustomSampleComponent, ListViewCustomSampleItemDescription>
	{
		[DomainReloadExclude]
		static readonly Comparison<ListViewCustomSampleItemDescription> ItemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();
			DataSource.Comparison = ItemsComparison;
		}
	}
}