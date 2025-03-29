namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ChatLine component.
	/// </summary>
	public class ChatLineComponent : ListViewItem, IViewData<ChatLine>
	{
		/// <summary>
		/// Username.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with UserNameAdapter.")]
		public Text UserName;

		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MessageAdapter.")]
		public Text Message;

		/// <summary>
		/// Message time.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TimeAdapter.")]
		public Text Time;

		/// <summary>
		/// Username.
		/// </summary>
		[SerializeField]
		public TextAdapter UserNameAdapter;

		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		public TextAdapter MessageAdapter;

		/// <summary>
		/// Message time.
		/// </summary>
		[SerializeField]
		public TextAdapter TimeAdapter;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public Image Image;

		/// <summary>
		/// AudioPlayer.
		/// </summary>
		[SerializeField]
		public AudioPlayer Audio;

		/// <summary>
		/// Lightbox to display image.
		/// </summary>
		[SerializeField]
		public Lightbox Lightbox;

		/// <summary>
		/// Message data.
		/// </summary>
		protected ChatLine Item;

		/// <summary>
		/// Chat.
		/// </summary>
		[SerializeField]
		public ChatView Chat;

		/// <summary>
		/// Invoke chat event.
		/// </summary>
		public void ChatEventInvoke()
		{
			Chat.MyEvent.Invoke();
			Debug.Log("Chat.MyEvent.Invoke()");
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				#pragma warning restore
				GraphicsBackgroundVersion = 1;
			}

			base.GraphicsBackgroundInit();
		}

		/// <summary>
		/// Display ChatLine.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ChatLine item)
		{
			Item = item;

			UserNameAdapter.text = item.UserName;
			MessageAdapter.text = item.Message;
			TimeAdapter.text = item.Time.ToString("[HH:mm:ss]");

			MessageAdapter.gameObject.SetActive(item.Message != null);

			if (Image != null)
			{
				Image.gameObject.SetActive(item.Image != null);
				Image.sprite = item.Image;
			}

			if (Audio != null)
			{
				Audio.gameObject.SetActive(item.Audio != null);
				Audio.SetAudioClip(item.Audio);
			}
		}

		/// <summary>
		/// Show lightbox with image.
		/// </summary>
		public void ShowLightbox()
		{
			Lightbox.Show(Item.Image);
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Image, nameof(Image.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Image, nameof(Image.color), owner);
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

#pragma warning disable 0612, 0618
			Utilities.RequireComponent(UserName, ref UserNameAdapter);
			Utilities.RequireComponent(Message, ref MessageAdapter);
			Utilities.RequireComponent(Time, ref TimeAdapter);
#pragma warning restore 0612, 0618
		}
	}
}