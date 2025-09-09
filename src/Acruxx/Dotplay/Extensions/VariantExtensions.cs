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
        return Variant.From(value);
    }
}