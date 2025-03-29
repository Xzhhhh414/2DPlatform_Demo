namespace UIWidgets
{
	using System;

	/// <summary>
	/// Observable list.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public class ObservableListFilter<T>
	{
		ObservableList<T> input;

		/// <summary>
		/// Input.
		/// </summary>
		public ObservableList<T> Input
		{
			get => input;
			set
			{
				if (input != null)
				{
					input.OnChange -= UpdateOutput;
				}

				input = value ?? throw new ArgumentNullException(nameof(value));
				input.OnChange += UpdateOutput;

				UpdateOutput();
			}
		}

		readonly ObservableList<T> output = new ObservableList<T>();

		/// <summary>
		/// Output.
		/// </summary>
		public ObservableList<T> Output => output;

		Predicate<T> predicate;

		/// <summary>
		/// Predicate.
		/// </summary>
		public Predicate<T> Predicate
		{
			get => predicate;
			set
			{
				predicate = value ?? throw new ArgumentNullException(nameof(value));
				UpdateOutput();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableListFilter{T}"/> class.
		/// </summary>
		/// <param name="input">Input list.</param>
		/// <param name="predicate">Predicate.</param>
		public ObservableListFilter(ObservableList<T> input, Predicate<T> predicate)
		{
			Input = input;
			Predicate = predicate;
		}

		void UpdateOutput()
		{
			if ((input == null) || (predicate == null))
			{
				return;
			}

			using var _ = output.BeginUpdate();
			output.Clear();

			foreach (var item in input)
			{
				if (predicate(item))
				{
					output.Add(item);
				}
			}
		}

		/// <summary>
		/// Refresh.
		/// </summary>
		public void Refresh()
		{
			UpdateOutput();
		}
	}
}