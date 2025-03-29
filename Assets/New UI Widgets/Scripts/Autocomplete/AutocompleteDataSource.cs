namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// AutocompleteDataSource.
	/// Set Autocomplete.DataSource with strings from file.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Collections/Autocomplete DataSource")]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/autocomplete-datasource.html")]
	public class AutocompleteDataSource : MonoBehaviourInitiable
	{
		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File
		{
			get => file;

			set
			{
				file = value;

				if (file != null)
				{
					SetDataSource(file);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>() { "#", "//" };

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected override void InitOnce()
		{
			base.InitOnce();

			File = file;
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		public virtual void SetDataSource(TextAsset sourceFile)
		{
			if (file == null)
			{
				return;
			}

			var data = new List<string>();

			foreach (var item in sourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
			{
				var trimmed = item.TrimEnd();
				if (string.IsNullOrEmpty(trimmed) || IsComment(trimmed))
				{
					continue;
				}

				data.Add(trimmed);
			}

			if (TryGetComponent<AutocompleteString>(out var autocomplete))
			{
				autocomplete.DataSource = data;
			}
			#pragma warning disable 0618
			else if (TryGetComponent<Autocomplete>(out var autocomplete_obsolete))
			{
				autocomplete_obsolete.DataSource = data;
			}
			#pragma warning restore 0618
		}

		/// <summary>
		/// Is comment?
		/// </summary>
		/// <param name="str">String to check.</param>
		/// <returns>true if input is comment; otherwise false.</returns>
		protected virtual bool IsComment(string str)
		{
			for (int i = 0; i < CommentsStartWith.Count; i++)
			{
				if (str.StartsWith(CommentsStartWith[i], StringComparison.InvariantCulture))
				{
					return true;
				}
			}

			return false;
		}
	}
}