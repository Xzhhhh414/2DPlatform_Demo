namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker RGB block.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/colorpicker.html")]
	public class ColorPickerRGBBlock : ColorPickerBlock
	{
		[SerializeField]
		Slider rSlider;

		/// <summary>
		/// Gets or sets the Red slider.
		/// </summary>
		/// <value>The Red slider.</value>
		public Slider RSlider
		{
			get => rSlider;

			set => SetRSlider(value);
		}

		[SerializeField]
		Spinner rInput;

		/// <summary>
		/// Gets or sets the Red input.
		/// </summary>
		/// <value>The Red input.</value>
		public Spinner RInput
		{
			get => rInput;

			set => SetRInput(value);
		}

		[SerializeField]
		Image rSliderBackground;

		/// <summary>
		/// Gets or sets the Red slider background.
		/// </summary>
		/// <value>The Red slider background.</value>
		public Image RSliderBackground
		{
			get => rSliderBackground;

			set
			{
				rSliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Slider gSlider;

		/// <summary>
		/// Gets or sets the Green slider.
		/// </summary>
		/// <value>The Green slider.</value>
		public Slider GSlider
		{
			get => gSlider;

			set => SetGSlider(value);
		}

		[SerializeField]
		Spinner gInput;

		/// <summary>
		/// Gets or sets the Green input.
		/// </summary>
		/// <value>The Green input.</value>
		public Spinner GInput
		{
			get => gInput;

			set => SetGInput(value);
		}

		[SerializeField]
		Image gSliderBackground;

		/// <summary>
		/// Gets or sets the Green slider background.
		/// </summary>
		/// <value>The Green slider background.</value>
		public Image GSliderBackground
		{
			get => gSliderBackground;

			set
			{
				gSliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Slider bSlider;

		/// <summary>
		/// Gets or sets the Blue slider.
		/// </summary>
		/// <value>The Blue slider.</value>
		public Slider BSlider
		{
			get => bSlider;

			set => SetBSlider(value);
		}

		[SerializeField]
		Spinner bInput;

		/// <summary>
		/// Gets or sets the Blue input.
		/// </summary>
		/// <value>The Blue input.</value>
		public Spinner BInput
		{
			get => bInput;

			set => SetBInput(value);
		}

		[SerializeField]
		Image bSliderBackground;

		/// <summary>
		/// Gets or sets the Blue slider background.
		/// </summary>
		/// <value>The Blue slider background.</value>
		public Image BSliderBackground
		{
			get => bSliderBackground;

			set
			{
				bSliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader defaultShader;

		/// <summary>
		/// Gets or sets the default shader to display gradients for sliders background.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShader
		{
			get => defaultShader;

			set
			{
				defaultShader = value;
				UpdateMaterial();
			}
		}

		ColorPickerInputMode inputMode;

		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		public ColorPickerInputMode InputMode
		{
			get => inputMode;

			set
			{
				inputMode = value;

				gameObject.SetActive(inputMode == ColorPickerInputMode.RGB);
				UpdateView();
			}
		}

		ColorPickerPaletteMode paletteMode;

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		public ColorPickerPaletteMode PaletteMode
		{
			get => paletteMode;

			set => paletteMode = value;
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		public ColorRGBChangedEvent OnChangeRGB = new ColorRGBChangedEvent();

		/// <summary>
		/// OnChangeHSV event.
		/// </summary>
		public ColorHSVChangedEvent OnChangeHSV = new ColorHSVChangedEvent();

		/// <summary>
		/// OnChangeAlpha event.
		/// </summary>
		public ColorAlphaChangedEvent OnChangeAlpha = new ColorAlphaChangedEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			RSlider = rSlider;
			RInput = rInput;
			RSliderBackground = rSliderBackground;

			GSlider = gSlider;
			GInput = gInput;
			GSliderBackground = gSliderBackground;

			BSlider = bSlider;
			BInput = bInput;
			BSliderBackground = bSliderBackground;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Sets the Red slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetRSlider(Slider value)
		{
			if (rSlider != null)
			{
				rSlider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			rSlider = value;
			if (rSlider != null)
			{
				rSlider.onValueChanged.AddListener(SliderValueChanged);
			}
		}

		/// <summary>
		/// Sets the Red input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetRInput(Spinner value)
		{
			if (rInput != null)
			{
				rInput.onValueChangeInt.RemoveListener(SpinnerValueChanged);
			}

			rInput = value;
			if (rInput != null)
			{
				rInput.onValueChangeInt.AddListener(SpinnerValueChanged);
			}
		}

		/// <summary>
		/// Sets the Green slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetGSlider(Slider value)
		{
			if (gSlider != null)
			{
				gSlider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			gSlider = value;
			if (gSlider != null)
			{
				gSlider.onValueChanged.AddListener(SliderValueChanged);
			}
		}

		/// <summary>
		/// Sets the Green input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetGInput(Spinner value)
		{
			if (gInput != null)
			{
				gInput.onValueChangeInt.RemoveListener(SpinnerValueChanged);
			}

			gInput = value;
			if (gInput != null)
			{
				gInput.onValueChangeInt.AddListener(SpinnerValueChanged);
			}
		}

		/// <summary>
		/// Sets the Blue slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetBSlider(Slider value)
		{
			if (bSlider != null)
			{
				bSlider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			bSlider = value;
			if (bSlider != null)
			{
				bSlider.onValueChanged.AddListener(SliderValueChanged);
			}
		}

		/// <summary>
		/// Sets the Blue input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetBInput(Spinner value)
		{
			if (bInput != null)
			{
				bInput.onValueChangeInt.RemoveListener(SpinnerValueChanged);
			}

			bInput = value;
			if (bInput != null)
			{
				bInput.onValueChangeInt.AddListener(SpinnerValueChanged);
			}
		}

		void SpinnerValueChanged(int value)
		{
			ValueChanged(isSlider: false);
		}

		void SliderValueChanged(float value)
		{
			ValueChanged();
		}

		/// <summary>
		/// If in update mode?
		/// </summary>
		protected bool inUpdateMode;

		/// <summary>
		/// Values the changed.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		protected virtual void ValueChanged(bool isSlider = true)
		{
			if (!IsInteractable() || inUpdateMode)
			{
				return;
			}

			var color = new Color32(
				GetRed(isSlider),
				GetGreen(isSlider),
				GetBlue(isSlider),
				currentColor.a);

			OnChangeRGB.Invoke(color);
		}

		/// <summary>
		/// Gets the red.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		/// <returns>The red.</returns>
		protected byte GetRed(bool isSlider = true)
		{
			if ((rSlider != null) && isSlider)
			{
				return (byte)rSlider.value;
			}

			if (rInput != null)
			{
				return (byte)rInput.Value;
			}

			return currentColor.r;
		}

		/// <summary>
		/// Gets the green.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		/// <returns>The green.</returns>
		protected byte GetGreen(bool isSlider = true)
		{
			if ((gSlider != null) && isSlider)
			{
				return (byte)gSlider.value;
			}

			if (gInput != null)
			{
				return (byte)gInput.Value;
			}

			return currentColor.g;
		}

		/// <summary>
		/// Gets the blue.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		/// <returns>The blue.</returns>
		protected byte GetBlue(bool isSlider = true)
		{
			if ((bSlider != null) && isSlider)
			{
				return (byte)bSlider.value;
			}

			if (bInput != null)
			{
				return (byte)bInput.Value;
			}

			return currentColor.b;
		}

		/// <summary>
		/// Current color.
		/// </summary>
		protected Color32 currentColor;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();

			Compatibility.ToggleGameObject(rSlider);
			Compatibility.ToggleGameObject(gSlider);
			Compatibility.ToggleGameObject(bSlider);
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			inUpdateMode = true;

			if (rSlider != null)
			{
				rSlider.value = currentColor.r;
			}

			if (rInput != null)
			{
				rInput.Value = currentColor.r;
			}

			if (gSlider != null)
			{
				gSlider.value = currentColor.g;
			}

			if (gInput != null)
			{
				gInput.Value = currentColor.g;
			}

			if (bSlider != null)
			{
				bSlider.value = currentColor.b;
			}

			if (bInput != null)
			{
				bInput.Value = currentColor.b;
			}

			if (rSliderBackground != null)
			{
				rSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Left, new Color32(0, currentColor.g, currentColor.b, 255));
				rSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Right, new Color32(255, currentColor.g, currentColor.b, 255));
				rSliderBackground.SetMaterialDirty();
			}

			if (gSliderBackground != null)
			{
				gSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Left, new Color32(currentColor.r, 0, currentColor.b, 255));
				gSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Right, new Color32(currentColor.r, 255, currentColor.b, 255));
				gSliderBackground.SetMaterialDirty();
			}

			if (bSliderBackground != null)
			{
				bSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Left, new Color32(currentColor.r, currentColor.g, 0, 255));
				bSliderBackground.material.SetColor(ColorPicker.ShaderIDs.Right, new Color32(currentColor.r, currentColor.g, 255, 255));
				bSliderBackground.SetMaterialDirty();
			}

			inUpdateMode = false;
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			if (defaultShader == null)
			{
				return;
			}

			if (rSliderBackground != null)
			{
				rSliderBackground.material = new Material(defaultShader);
			}

			if (gSliderBackground != null)
			{
				gSliderBackground.material = new Material(defaultShader);
			}

			if (bSliderBackground != null)
			{
				bSliderBackground.material = new Material(defaultShader);
			}

			UpdateViewReal();
		}

		/// <inheritdoc/>
		public override void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), rSliderBackground, nameof(Graphic.color), this);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), gSliderBackground, nameof(Graphic.color), this);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), bSliderBackground, nameof(Graphic.color), this);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			rSlider = null;
			rInput = null;

			gSlider = null;
			gInput = null;

			bSlider = null;
			bInput = null;
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if ((rSlider != null) && (rSlider.handleRect != null) && rSlider.handleRect.TryGetComponent<Image>(out var r_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(rSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(r_img);
			}

			if ((gSlider != null) && (gSlider.handleRect != null) && gSlider.handleRect.TryGetComponent<Image>(out var g_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(gSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(g_img);
			}

			if ((bSlider != null) && (bSlider.handleRect != null) && bSlider.handleRect.TryGetComponent<Image>(out var b_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(bSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(b_img);
			}

			if (rInput != null)
			{
				rInput.SetStyle(styleColorPicker.InputSpinner, style);
			}

			if (gInput != null)
			{
				gInput.SetStyle(styleColorPicker.InputSpinner, style);
			}

			if (bInput != null)
			{
				bInput.SetStyle(styleColorPicker.InputSpinner, style);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if ((rSlider != null) && (rSlider.handleRect != null) && rSlider.handleRect.TryGetComponent<Image>(out var r_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(rSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(r_img);
			}

			if ((gSlider != null) && (gSlider.handleRect != null) && gSlider.handleRect.TryGetComponent<Image>(out var g_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(gSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(g_img);
			}

			if ((bSlider != null) && (bSlider.handleRect != null) && bSlider.handleRect.TryGetComponent<Image>(out var b_img))
			{
				var handle_style = UtilitiesUI.IsHorizontal(bSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(b_img);
			}

			if (rInput != null)
			{
				rInput.GetStyle(styleColorPicker.InputSpinner, style);
			}

			if (gInput != null)
			{
				gInput.GetStyle(styleColorPicker.InputSpinner, style);
			}

			if (bInput != null)
			{
				bInput.GetStyle(styleColorPicker.InputSpinner, style);
			}
		}
	}
}