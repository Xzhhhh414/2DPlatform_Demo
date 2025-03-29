namespace UIWidgets.Examples.Tasks
{
	using UIWidgets;
	using UIWidgets.Attributes;

	/// <summary>
	/// TaskView.
	/// </summary>
	public class TaskView : ListViewCustom<TaskComponent, Task>
	{
		/// <summary>
		/// Tasks comparison.
		/// </summary>
		[DomainReloadExclude]
		static readonly System.Comparison<Task> ItemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			DataSource.Comparison = ItemsComparison;
		}
	}
}