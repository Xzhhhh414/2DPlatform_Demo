namespace UIThemes
{
	using System;

	/// <summary>
	/// Addressable asset info.
	/// </summary>
	public readonly struct AddressableAsset
	{
		/// <summary>
		/// Addressable address.
		/// </summary>
		public readonly string Address;

		/// <summary>
		/// GUID.
		/// </summary>
		public readonly string GUID;

		/// <summary>
		/// Is valid info?
		/// </summary>
		public readonly bool Valid;

		/// <summary>
		/// Initializes a new instance of the <see cref="AddressableAsset"/> struct.
		/// </summary>
		/// <param name="address">Addressable address.</param>
		/// <param name="guid">GUID.</param>
		public AddressableAsset(string address, string guid)
		{
			Address = address;
			GUID = guid;
			Valid = true;
		}

		private AddressableAsset(bool valid)
		{
			Address = string.Empty;
			GUID = string.Empty;
			Valid = false;
		}

		/// <summary>
		/// Fail instance.
		/// </summary>
		public static AddressableAsset Fail => new AddressableAsset(false);
	}
}
