namespace UIWidgets
{
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>
	/// IObservableList.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public interface IObservableList<T> : IList<T>, INotifyPropertyChanged, ICollectionChanged, ICollectionItemChanged, IListUpdatable
	{
	}
}