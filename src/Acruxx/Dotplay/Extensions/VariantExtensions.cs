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
				return Variant.From(in b);
			case sbyte sb:
				{ long lv = sb; return Variant.From(in lv); }
			case byte by:
				{ long lv = by; return Variant.From(in lv); }
			case short sh:
				{ long lv = sh; return Variant.From(in lv); }
			case ushort ush:
				{ long lv = ush; return Variant.From(in lv); }
			case int i32:
				{ long lv = i32; return Variant.From(in lv); }
			case uint ui32:
				{ long lv = unchecked((long)ui32); return Variant.From(in lv); }
			case long l:
				return Variant.From(in l);
			case float f:
				return Variant.From(in f);
			case double d:
				return Variant.From(in d);
			case string s:
				return Variant.From(in s);
			case Enum e:
				{ long lv = Convert.ToInt64(e); return Variant.From(in lv); }
			case GodotObject go:
				return Variant.From(in go);
			case Vector2 v2:
				return Variant.From(in v2);
			case Vector3 v3:
				return Variant.From(in v3);
			case Vector2I v2i:
				return Variant.From(in v2i);
			case Vector3I v3i:
				return Variant.From(in v3i);
			case Vector4 v4:
				return Variant.From(in v4);
			case Vector4I v4i:
				return Variant.From(in v4i);
			case Color color:
				return Variant.From(in color);
			case Quaternion quat:
				return Variant.From(in quat);
			case Basis basis:
				return Variant.From(in basis);
			case Transform3D xform:
				return Variant.From(in xform);
			case Aabb aabb:
				return Variant.From(in aabb);
			case Rect2 rect2:
				return Variant.From(in rect2);
			case Rect2I rect2i:
				return Variant.From(in rect2i);
			case Plane plane:
				return Variant.From(in plane);
			case Rid rid:
				return Variant.From(in rid);
			case NodePath nodePath:
				return Variant.From(in nodePath);
			case StringName stringName:
				return Variant.From(in stringName);
			case Callable callable:
				return Variant.From(in callable);
		}

		GD.PushWarning($"Variant conversion not supported for type '{value.GetType().FullName}'");
		return default;
	}
}
