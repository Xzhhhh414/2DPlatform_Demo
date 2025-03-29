namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// ChatView.
	/// </summary>
	public class ChatView : ListViewCustomHeight<ChatLineComponent, ChatLine>
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		protected class Selector : IListViewTemplateSelector<ChatLineComponent, ChatLine>
		{
			/// <summary>
			/// Incoming template.
			/// </summary>
			public ChatLineComponent IncomingTemplate;

			/// <summary>
			/// Outgoing template.
			/// </summary>
			public ChatLineComponent OutgoingTemplate;

			/// <inheritdoc/>
			public ChatLineComponent[] AllTemplates()
			{
				return new[] { IncomingTemplate, OutgoingTemplate };
			}

			/// <inheritdoc/>
			public ChatLineComponent Select(int index, ChatLine item)
			{
				return (item.Type == ChatLineType.Incoming) ? IncomingTemplate : OutgoingTemplate;
			}
		}

		/// <summary>
		/// Chat event.
		/// </summary>
		[SerializeField]
		public UnityEvent MyEvent;

		#region DataSource wrapper and Filter

		ObservableList<ChatLine> fullDataSource;

		/// <summary>
		/// All messages.
		/// </summary>
		public ObservableList<ChatLine> FullDataSource
		{
			get
			{
				return fullDataSource;
			}

			set
			{
				// unsubscribe update event
				fullDataSource?.OnChangeMono.RemoveListener(UpdateDataSource);

				fullDataSource = value;

				// subscribe update event
				fullDataSource?.OnChangeMono.AddListener(UpdateDataSource);

				UpdateDataSource();
			}
		}

		Func<ChatLine, bool> filter;

		/// <summary>
		/// Messages filter.
		/// </summary>
		public Func<ChatLine, bool> Filter
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				UpdateDataSource();
			}
		}

		void UpdateDataSource()
		{
			using var _ = DataSource.BeginUpdate();

			DataSource.Clear();
			if (filter != null)
			{
				foreach (var item in FullDataSource)
				{
					if (Filter(item))
					{
						DataSource.Add(item);
					}
				}
			}
			else
			{
				DataSource.AddRange(FullDataSource);
			}
		}

		/// <summary>
		/// IncomingTemplate.
		/// </summary>
		[SerializeField]
		protected ChatLineComponent IncomingTemplate;

		/// <summary>
		/// OutgoingTemplate.
		/// </summary>
		[SerializeField]
		protected ChatLineComponent OutgoingTemplate;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			if (fullDataSource == null)
			{
				fullDataSource = new ObservableList<ChatLine>();
				fullDataSource.AddRange(DataSource);
				fullDataSource.OnChangeMono.AddListener(UpdateDataSource);

				UpdateDataSource();
			}

			base.InitOnce();
		}
		#endregion

		/// <inheritdoc/>
		protected override IListViewTemplateSelector<ChatLineComponent, ChatLine> CreateTemplateSelector()
		{
			return new Selector()
			{
				IncomingTemplate = IncomingTemplate,
				OutgoingTemplate = OutgoingTemplate,
			};
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			fullDataSource?.OnChangeMono.RemoveListener(UpdateDataSource);

			base.OnDestroy();
		}
	}
}