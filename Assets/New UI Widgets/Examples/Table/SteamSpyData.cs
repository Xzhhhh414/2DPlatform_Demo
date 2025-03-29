namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// SteamSpyData.
	/// </summary>
	public class SteamSpyData : MonoBehaviour
	{
		[DomainReloadExclude]
		static readonly char[] LineEnd = new char[] { '\n' };

		[DomainReloadExclude]
		static readonly char[] Separator = new char[] { '\t' };

		/// <summary>
		/// SteamSpyView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("steamSpyView")]
		protected SteamSpyView SteamSpyView;

		[SerializeField]
		TextAsset csv;

		string content = string.Empty;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Load();
		}

		/// <summary>
		/// Load data.
		/// </summary>
		public void Load()
		{
			StartCoroutine(LoadData());
		}

		/// <summary>
		/// Send web request.
		/// </summary>
		/// <returns>IEnumerator.</returns>
		protected IEnumerator Request()
		{
			var url = "https://ilih.name/steamspy/";
			using var www = UnityEngine.Networking.UnityWebRequest.Get(new System.Uri(url));

			yield return www.SendWebRequest();

			if (Compatibility.IsError(www))
			{
				Debug.Log(www.error);
			}
			else
			{
				content = www.downloadHandler.text;
			}
		}

		/// <summary>
		/// Coroutine to load data from web.
		/// </summary>
		/// <returns>IEnumerator.</returns>
		protected IEnumerator LoadData()
		{
			if (csv != null)
			{
				content = csv.text;
			}
			else
			{
				yield return Request();
			}

			var lines = content.Split(LineEnd);

			using var _ = SteamSpyView.DataSource.BeginUpdate();

			SteamSpyView.DataSource.Clear();

			foreach (var line in lines)
			{
				var item = ParseLine(line);
				if (item != null)
				{
					SteamSpyView.DataSource.Add(item);
				}
			}
		}

		/// <summary>
		/// Parse line and add to SteamSpyView.
		/// </summary>
		/// <param name="line">Line to parse.</param>
		/// <returns>Item.</returns>
		protected static SteamSpyItem ParseLine(string line)
		{
			if (string.IsNullOrEmpty(line))
			{
				return null;
			}

			var info = line.Split(Separator);

			var item = new SteamSpyItem()
			{
				Name = info[0],
				ScoreRank = string.IsNullOrEmpty(info[1]) ? -1 : int.Parse(info[1]),

				Owners = int.Parse(info[2]),
				OwnersVariance = int.Parse(info[3]),

				Players = int.Parse(info[4]),
				PlayersVariance = int.Parse(info[5]),

				PlayersIn2Week = int.Parse(info[6]),
				PlayersIn2WeekVariance = int.Parse(info[7]),

				AverageTimeIn2Weeks = int.Parse(info[8]),
				MedianTimeIn2Weeks = int.Parse(info[9]),
			};

			return item;
		}
	}
}