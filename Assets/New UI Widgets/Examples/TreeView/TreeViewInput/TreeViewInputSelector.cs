namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Template selector for the TreeViewInput.
	/// </summary>
	public class TreeViewInputSelector : MonoBehaviour, IListViewTemplateSelector<TreeViewComponent, ListNode<TreeViewItem>>
	{
		/// <summary>
		/// Template Category.
		/// </summary>
		[SerializeField]
		protected TreeViewComponent TemplateCategory;

		/// <summary>
		/// TemplateBool.
		/// </summary>
		[SerializeField]
		protected TreeViewInputBool TemplateBool;

		/// <summary>
		/// TemplateInt.
		/// </summary>
		[SerializeField]
		protected TreeViewInputInt TemplateInt;

		/// <summary>
		/// TemplateString.
		/// </summary>
		[SerializeField]
		protected TreeViewInputString TemplateString;

		/// <summary>
		/// TemplateVector3.
		/// </summary>
		[SerializeField]
		protected TreeViewInputVector3 TemplateVector3;

		/// <summary>
		/// TemplateColor.
		/// </summary>
		[SerializeField]
		protected TreeViewInputColor TemplateColor;

		[NonSerialized]
		TreeViewComponent[] templates;

		/// <summary>
		/// Get all templates.
		/// </summary>
		/// <returns>Templates.</returns>
		public TreeViewComponent[] AllTemplates()
		{
			templates ??= new[]
			{
				TemplateCategory,
				TemplateBool,
				TemplateInt,
				TemplateString,
				TemplateVector3,
				TemplateString,
			};

			return templates;
		}

		/// <inheritdoc/>
		public TreeViewComponent Select(int index, ListNode<TreeViewItem> item)
		{
			if (item.Depth == 0)
			{
				return TemplateCategory;
			}

			if (item.Node.Item.Tag is bool)
			{
				return TemplateBool;
			}

			if (item.Node.Item.Tag is int)
			{
				return TemplateInt;
			}

			if (item.Node.Item.Tag is string)
			{
				return TemplateString;
			}

			if (item.Node.Item.Tag is Vector3)
			{
				return TemplateVector3;
			}

			if (item.Node.Item.Tag is Color)
			{
				return TemplateColor;
			}

			return TemplateCategory;
		}
	}
}