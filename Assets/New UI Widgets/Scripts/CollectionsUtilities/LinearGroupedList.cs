namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for LinearGroupedList.
	/// Before each header inserted EmptyItem to fill previous block to ItemsPerBlock count.
	/// After each header item inserted EmptyHeaderItem (total ItemsPerBlock - 1).
	/// </summary>
	/// <typeparam name="T">Items type.</typeparam>
	public class LinearGroupedList<T>
	{
		ObservableList<T> input;

		/// <summary>
		/// Original list.
		/// </summary>
		public ObservableList<T> Input
		{
			get
			{
				return input;
			}

			set
			{
				if (input != null)
				{
					input.OnChange -= ProcessChanges;
				}

				input = value;

				if (input != null)
				{
					input.OnChange += ProcessChanges;
				}

				ProcessChanges();
			}
		}

		ObservableList<T> output;

		/// <summary>
		/// List with inserted empty items.
		/// </summary>
		public ObservableList<T> Output
		{
			get
			{
				return output;
			}

			set
			{
				output = value;
				ProcessChanges();
			}
		}

		/// <summary>
		/// Function to check if specified item is header item.
		/// </summary>
		public Func<T, bool> IsHeader
		{
			get;
			set;
		}

		int itemsPerBlock = 1;

		/// <summary>
		/// Items per block (row or column).
		/// </summary>
		public int ItemsPerBlock
		{
			get
			{
				return itemsPerBlock;
			}

			set
			{
				if (itemsPerBlock != value)
				{
					itemsPerBlock = value;
					ProcessChanges();
				}
			}
		}

		T emptyHeaderItem;

		/// <summary>
		/// Empty item to fill header row.
		/// </summary>
		public T EmptyHeaderItem
		{
			get
			{
				return emptyHeaderItem;
			}

			set
			{
				if (!IsItemsEquals(emptyHeaderItem, value))
				{
					emptyHeaderItem = value;
					ProcessChanges();
				}
			}
		}

		T emptyItem;

		/// <summary>
		/// Empty item to fill the last items block.
		/// </summary>
		public T EmptyItem
		{
			get
			{
				return emptyItem;
			}

			set
			{
				if (!IsItemsEquals(emptyItem, value))
				{
					emptyItem = value;
					ProcessChanges();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinearGroupedList{T}"/> class.
		/// </summary>
		/// <param name="isHeader">Function to check if item is header item.</param>
		public LinearGroupedList(Func<T, bool> isHeader)
		{
			IsHeader = isHeader;
		}

		static bool IsItemsEquals(T a, T b)
		{
			return EqualityComparer<T>.Default.Equals(a, b);
		}

		/// <summary>
		/// Process changes.
		/// </summary>
		protected virtual void ProcessChanges()
		{
			if (Output == null)
			{
				return;
			}

			using var _ = Output.BeginUpdate();

			Output.Clear();

			FillOutput();
		}

		/// <summary>
		/// Insert item N times.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="n">Times to insert item.</param>
		protected virtual void Insert(T item, int n)
		{
			for (var i = 0; i < n; i++)
			{
				Output.Add(item);
			}
		}

		/// <summary>
		/// Fill output.
		/// </summary>
		protected virtual void FillOutput()
		{
			if (input == null)
			{
				return;
			}

			for (int i = 0; i < input.Count; i++)
			{
				var item = input[i];
				if (IsHeader(item))
				{
					var n = Output.Count % ItemsPerBlock;
					if (n != 0)
					{
						Insert(EmptyItem, ItemsPerBlock - n);
					}

					Output.Add(item);

					Insert(EmptyHeaderItem, ItemsPerBlock - 1);
				}
				else
				{
					Output.Add(item);
				}
			}
		}
	}
}