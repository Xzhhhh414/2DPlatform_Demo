namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Test ScrollRect Events.
	/// </summary>
	[RequireComponent(typeof(ScrollRectEvents))]
	public class TestScrollRectEvents : MonoBehaviourInitiable
	{
		[DomainReloadExclude]
		static readonly char[] LineEnd = new char[] { '\n' };

		[DomainReloadExclude]
		static readonly char[] Separator = new char[] { '\t' };

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// Data
		/// </summary>
		protected ObservableList<ListViewIconsItemDescription> Data;

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void InitOnce()
		{
			base.InitOnce();

			#pragma warning disable 0618
			ListView.Sort = false;
			#pragma warning restore 0618
			Data = ListView.DataSource;
			Data.Comparison = null;
			ListView.Init();

			if (TryGetComponent<ScrollRectEvents>(out var scrollRectEvents))
			{
				scrollRectEvents.OnPullUp.AddListener(Refresh);
				scrollRectEvents.OnPullDown.AddListener(LoadMore);
			}
		}

		/// <summary>
		/// Handle enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Init();
			StartCoroutine(LoadData(0));
		}

		/// <summary>
		/// Load data.
		/// </summary>
		/// <param name="start">Start index.</param>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator LoadData(int start)
		{
			if (start == 0)
			{
				Data.Clear();
			}

			var lines = Compatibility.EmptyArray<string>();

			var url = string.Format("https://ilih.name/steamspy/?start={0}", start.ToString());
#if UNITY_2018_3_OR_NEWER
			using (var www = UnityEngine.Networking.UnityWebRequest.Get(new System.Uri(url)))
			{
				yield return www.SendWebRequest();

				if (Compatibility.IsError(www))
				{
					Debug.Log(www.error);
				}
				else
				{
					lines = www.downloadHandler.text.Split(LineEnd);
				}
			}
#else
			WWW www = new WWW(url);
			yield return www;

			lines = www.text.Split(LineEnd);

			www.Dispose();
#endif

			using var _ = Data.BeginUpdate();

			foreach (var line in lines)
			{
				ParseLine(line);
			}
		}

		/// <summary>
		/// Parse line.
		/// </summary>
		/// <param name="line">Line.</param>
		protected virtual void ParseLine(string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return;
			}

			var info = line.Split(Separator);

			var item = new ListViewIconsItemDescription() { Name = string.Format("{0}. {1}", (Data.Count + 1).ToString(), info[0]), };
			Data.Add(item);
		}

		/// <summary>
		/// Load initial data.
		/// </summary>
		public void Refresh()
		{
			StartCoroutine(LoadData(0));
		}

		/// <summary>
		/// Load more data.
		/// </summary>
		public void LoadMore()
		{
			StartCoroutine(LoadData(Data.Count));
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (TryGetComponent<ScrollRectEvents>(out var scrollRectEvents))
			{
				scrollRectEvents.OnPullUp.RemoveListener(Refresh);
				scrollRectEvents.OnPullDown.RemoveListener(LoadMore);
			}
		}
	}
}