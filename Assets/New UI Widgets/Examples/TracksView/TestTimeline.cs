namespace UIWidgets.Examples
{
	using System;
	using UIWidgets.Timeline;
	using UnityEngine;

	/// <summary>
	/// Test Timeline.
	/// </summary>
	[RequireComponent(typeof(Timeline))]
	public class TestTimeline : MonoBehaviour
	{
		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			var track1 = new Track<TimelineData, TimeSpan>()
			{
				Name = "Track 1",
			};
			track1.Data.Add(new TimelineData()
			{
				Name = "Current Event",
				StartPoint = new TimeSpan(0, 0, 0),
				EndPoint = new TimeSpan(0, 0, 3),
			});
			track1.Data.Add(new TimelineData()
			{
				Name = "Next Event",
				StartPoint = new TimeSpan(0, 0, 4),
				EndPoint = new TimeSpan(0, 0, 8),
			});
			track1.Data.Add(new TimelineData()
			{
				Name = "Overlapping Event",
				StartPoint = new TimeSpan(0, 0, 2),
				EndPoint = new TimeSpan(0, 0, 15),
			});
			track1.Data.Add(new TimelineData()
			{
				Name = "Future Event",
				StartPoint = new TimeSpan(0, 0, 14),
				EndPoint = new TimeSpan(0, 0, 17),
			});

			var track2 = new Track<TimelineData, TimeSpan>()
			{
				Name = "Track 2",
			};
			track2.Data.Add(new TimelineData()
			{
				Name = "Event 2",
				StartPoint = new TimeSpan(0, 0, 10),
				EndPoint = new TimeSpan(0, 0, 12),
			});
			track2.Data.Add(new TimelineData()
			{
				Name = "Event 3",
				StartPoint = new TimeSpan(0, 0, 22),
				EndPoint = new TimeSpan(0, 0, 29),
			});
			track2.Data.Add(new TimelineData()
			{
				Name = "Event 4",
				StartPoint = new TimeSpan(0, 0, 35),
				EndPoint = new TimeSpan(0, 0, 40),
			});

			TryGetComponent<Timeline>(out var timeline);
			using var _ = timeline.Tracks.BeginUpdate();
			timeline.Tracks.Add(track1);
			timeline.Tracks.Add(track2);
		}

		/// <summary>
		/// Test add.
		/// </summary>
		public void TestAdd()
		{
			TryGetComponent<Timeline>(out var timeline);
			var t = new TimelineData()
			{
				Name = "Start At 0",
				StartPoint = new TimeSpan(0, 0, 0),
				EndPoint = new TimeSpan(0, 0, 4),
			};
			timeline.Tracks[0].Data.Add(t);
		}
	}
}