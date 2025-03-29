namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// FileListViewPath.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/filelistview.html")]
	public class FileListViewPath : UIBehaviourInteractable, IStylable
	{
		/// <summary>
		/// FileView.
		/// </summary>
		[HideInInspector]
		public FileListView FileView;

		/// <summary>
		/// Current path.
		/// </summary>
		protected string path;

		/// <summary>
		/// Current path.
		/// </summary>
		public string Path
		{
			get => path;

			set => SetPath(value);
		}

		/// <summary>
		/// FileListViewPathComponent template.
		/// </summary>
		[SerializeField]
		public FileListViewPathComponentBase Template;

		/// <summary>
		/// Disabled color.
		/// </summary>
		[SerializeField]
		public Color DisabledColor = new Color32(200, 200, 200, 128);

		/// <summary>
		/// Used components.
		/// </summary>
		[HideInInspector]
		protected List<FileListViewPathComponentBase> Components = new List<FileListViewPathComponentBase>();

		/// <summary>
		/// Directories list from current to root.
		/// </summary>
		protected List<string> CurrentDirectories = new List<string>();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Template.gameObject.SetActive(false);
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			base.OnInteractableChange(interactableState);

			SetViewInteractable(interactableState);
		}

		/// <summary>
		/// Set view interactable.
		/// </summary>
		/// <param name="interactableState">Interactable state.</param>
		protected virtual void SetViewInteractable(bool interactableState)
		{
			var color = interactableState ? Color.white : DisabledColor;
			foreach (var component in Components)
			{
				component.SetInteractableState(color);
			}
		}

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="newPath">New path.</param>
		protected virtual void SetPath(string newPath)
		{
			path = newPath;

			CurrentDirectories.Clear();
			do
			{
				CurrentDirectories.Add(newPath);
				newPath = System.IO.Path.GetDirectoryName(newPath);
			}
			while (!string.IsNullOrEmpty(newPath));

			CurrentDirectories.Reverse();

			for (int i = Components.Count - 1; i >= CurrentDirectories.Count; i--)
			{
				var c = Components[i];
				Components.RemoveAt(i);
				c.Owner = null;
				c.Free();
			}

			for (int i = Components.Count; i < CurrentDirectories.Count; i++)
			{
				Components.Add(Template.Instance());
			}

			for (int i = 0; i < CurrentDirectories.Count; i++)
			{
				SetCurrentDirectories(CurrentDirectories[i], i);
			}

			SetViewInteractable(IsInteractable());
		}

		/// <summary>
		/// Set current directories.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="index">Index of the component.</param>
		protected void SetCurrentDirectories(string path, int index)
		{
			Components[index].Owner = this;
			Components[index].SetPath(path);
		}

		/// <summary>
		/// Open directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		public virtual void Open(string directory)
		{
			var index = CurrentDirectories.IndexOf(directory);
			var select_directory = index == (CurrentDirectories.Count - 1) ? string.Empty : CurrentDirectories[index + 1];
			FileView.CurrentDirectory = directory;
			if (!string.IsNullOrEmpty(select_directory))
			{
				FileView.Select(FileView.FullName2Index(select_directory));
			}
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			foreach (var c in Components)
			{
				FreeComponent(c);
			}

			Components.Clear();
		}

		/// <summary>
		/// Free component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void FreeComponent(FileListViewPathComponentBase component)
		{
			component.Owner = null;
			component.Free();
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (Template != null)
			{
				Template.SetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].SetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (Template != null)
			{
				Template.GetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			return true;
		}
		#endregion
	}
}