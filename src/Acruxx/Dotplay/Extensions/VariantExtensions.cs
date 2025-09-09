using System;
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

		// Explicit mapping avoids generic ref overload issues, especially for float (System.Single)
		switch (value)
		{
			case Variant v:
				return v;
			case bool b:
				return Variant.From(ref b);
			case sbyte sb:
				{ long lv = sb; return Variant.From(ref lv); }
			case byte by:
				{ long lv = by; return Variant.From(ref lv); }
			case short sh:
				{ long lv = sh; return Variant.From(ref lv); }
			case ushort ush:
				{ long lv = ush; return Variant.From(ref lv); }
			case int i32:
				{ long lv = i32; return Variant.From(ref lv); }
			case uint ui32:
				{ long lv = unchecked((long)ui32); return Variant.From(ref lv); }
			case long l:
				return Variant.From(ref l);
			case float f:
				return Variant.From(ref f);
			case double d:
				return Variant.From(ref d);
			case string s:
				return Variant.From(ref s);
			case Enum e:
				{ long lv = Convert.ToInt64(e); return Variant.From(ref lv); }
			case GodotObject go:
				return Variant.From(ref go);
			case Vector2 v2:
				return Variant.From(ref v2);
			case Vector3 v3:
				return Variant.From(ref v3);
			case Vector2I v2i:
				return Variant.From(ref v2i);
			case Vector3I v3i:
				return Variant.From(ref v3i);
			case Vector4 v4:
				return Variant.From(ref v4);
			case Vector4I v4i:
				return Variant.From(ref v4i);
			case Color color:
				return Variant.From(ref color);
			case Quaternion quat:
				return Variant.From(ref quat);
			case Basis basis:
				return Variant.From(ref basis);
			case Transform3D xform:
				return Variant.From(ref xform);
			case Aabb aabb:
				return Variant.From(ref aabb);
			case Rect2 rect2:
				return Variant.From(ref rect2);
			case Rect2I rect2i:
				return Variant.From(ref rect2i);
			case Plane plane:
				return Variant.From(ref plane);
			case Rid rid:
				return Variant.From(ref rid);
			case NodePath nodePath:
				return Variant.From(ref nodePath);
			case StringName stringName:
				return Variant.From(ref stringName);
			case Callable callable:
				return Variant.From(ref callable);
		}

		GD.PushWarning($"Variant conversion not supported for type '{value.GetType().FullName}'");
		return default;
	}
}
