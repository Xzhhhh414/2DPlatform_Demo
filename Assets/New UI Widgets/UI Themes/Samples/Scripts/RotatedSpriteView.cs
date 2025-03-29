namespace UIThemes.Samples
{
	using UnityEngine;
	using UnityEngine.UIElements;
	#if UNITY_2022_1_OR_NEWER
	using FloatField = UnityEngine.UIElements.FloatField;
	#elif UNITY_EDITOR
	using FloatField = UnityEditor.UIElements.FloatField;
	#endif

	/// <summary>
	/// Field view for the rotated sprite.
	/// </summary>
	public class RotatedSpriteView : FieldView<RotatedSprite>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedSpriteView"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public RotatedSpriteView(string undoName, Theme.ValuesWrapper<RotatedSprite> values)
			: base(undoName, values)
		{
		}

		/// <inheritdoc/>
		protected override VisualElement CreateView(VariationId variationId, OptionId optionId, RotatedSprite value)
		{
			#if UNITY_EDITOR
			var block = new VisualElement();
			block.style.flexDirection = FlexDirection.Column;

			var input = new UnityEditor.UIElements.ObjectField
			{
				value = value.Sprite,
				objectType = typeof(Sprite),
			};
			input.RegisterValueChangedCallback(x =>
			{
				value.Sprite = x.newValue as Sprite;
				Save(variationId, optionId, value);
			});
			block.Add(input);

			var rotation = new FloatField("Rotation.Z")
			{
				value = value.RotationZ,
			};
			rotation.RegisterValueChangedCallback(x =>
			{
				value.RotationZ = x.newValue;
				Save(variationId, optionId, value);
			});
			block.Add(rotation);

			return block;
			#else
			return null;
			#endif
		}

		/// <inheritdoc/>
		public override void UpdateValue(VisualElement view, RotatedSprite value)
		{
			#if UNITY_EDITOR
			var block = new VisualElement();
			block.style.flexDirection = FlexDirection.Column;

			if (view.ElementAt(0) is UnityEditor.UIElements.ObjectField input)
			{
				input.value = value.Sprite;
				input.objectType = typeof(Sprite);
			}

			if (view.ElementAt(1) is FloatField rotation)
			{
				rotation.value = value.RotationZ;
			}
			#endif
		}
	}
}