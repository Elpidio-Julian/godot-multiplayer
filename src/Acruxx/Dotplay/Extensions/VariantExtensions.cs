using Godot;
using Godot.NativeInterop;

namespace Dotplay.Extensions;

/// <summary>
/// The variant extensions.
/// </summary>
public static class VariantExtensions
{
	/// <summary>
	/// Creates the from object.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A Variant.</returns>
	public static Variant CreateFromObject(object value)
	{
		if (value is null)
		{
			return default; // Nil variant
		}

		try
		{
			// Use runtime type for Variant conversion (avoids generic `object` specialization)
			return Variant.From((dynamic)value);
		}
		catch (System.Exception ex)
		{
			GD.PushWarning($"Variant conversion not supported for type '{value.GetType().FullName}': {ex.Message}");
			return default;
		}
	}
}
