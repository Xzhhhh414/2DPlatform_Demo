namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListView with Group header and multiple default items.
	/// </summary>
	public class GroupMultipleListView : ListViewCustom<GroupMultipleComponent, GroupMultipleItem>
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		protected class Selector : IListViewTemplateSelector<GroupMultipleComponent, GroupMultipleItem>
		{
			/// <summary>
			/// Group template.
			/// </summary>
			public GroupMultipleComponent GroupTemplate;

			/// <summary>
			/// Item checkbox template.
			/// </summary>
			public GroupMultipleComponent CheckboxTemplate;

			/// <summary>
			/// Item value template.
			/// </summary>
			public GroupMultipleComponent ValueTemplate;

			/// <inheritdoc/>
			public GroupMultipleComponent[] AllTemplates()
			{
				return new[] { GroupTemplate, CheckboxTemplate, ValueTemplate };
			}

			/// <inheritdoc/>
			public GroupMultipleComponent Select(int index, GroupMultipleItem item)
			{
				return item.Mode switch
				{
					GroupMultipleItem.ItemMode.Group => GroupTemplate,
					GroupMultipleItem.ItemMode.Checkbox => CheckboxTemplate,
					GroupMultipleItem.ItemMode.Value => ValueTemplate,
					_ => throw new ArgumentOutOfRangeException(nameof(item), item.Mode, "Unsupported Item Mode"),
				};
			}
		}

		/// <summary>
		/// GroupTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent GroupTemplate;

		/// <summary>
		/// ItemTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent CheckboxTemplate;

		/// <summary>
		/// ItemTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent ValueTemplate;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			CanSelect = IsItem;
		}

		/// <inheritdoc/>
		protected override IListViewTemplateSelector<GroupMultipleComponent, GroupMultipleItem> CreateTemplateSelector()
		{
			return new Selector()
			{
				GroupTemplate = GroupTemplate,
				CheckboxTemplate = CheckboxTemplate,
				ValueTemplate = ValueTemplate,
			};
		}

		bool IsItem(int index)
		{
			return DataSource[index].Mode != GroupMultipleItem.ItemMode.Group;
		}
	}
}