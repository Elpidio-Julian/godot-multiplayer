using System;
using Godot;

namespace Dotplay.Utils;

/// <summary>
/// The vector extension.
/// </summary>
public static class VectorExtension
{
	/// <summary>
	/// Gradually changes a vector towards a desired goal over time.
	/// </summary>
	/// <param name="current"></param>
	/// <param name="target"></param>
	/// <param name="currentVelocity"></param>
	/// <param name="smoothTime"></param>
	/// <param name="deltaTime"></param>
	public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float deltaTime)
	{
		const float maxSpeed = Mathf.Inf;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, deltaTime, maxSpeed);
	}

	/// <summary>
	/// Gradually changes a vector towards a desired goal over time.
	/// </summary>
	/// <param name="current"></param>
	/// <param name="target"></param>
	/// <param name="currentVelocity"></param>
	/// <param name="smoothTime"></param>
	/// <param name="deltaTime"></param>
	/// <param name="maxSpeed"></param>
	public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float deltaTime, float maxSpeed = Mathf.Inf)
	{
		// Based on Game Programming Gems 4 Chapter 1.10
		smoothTime = Mathf.Max(0.0001F, smoothTime);
		float omega = 2F / smoothTime;

		float x = omega * (float)deltaTime;
		float exp = 1F / (1F + x + (0.48F * x * x) + (0.235F * x * x * x));

		float change_x = current.X - target.X;
		float change_y = current.Y - target.Y;
		float change_z = current.Z - target.Z;
		Vector3 originalTo = target;

		// Clamp maximum speed
		float maxChange = maxSpeed * smoothTime;

		float maxChangeSq = maxChange * maxChange;
		float sqrmag = (change_x * change_x) + (change_y * change_y) + (change_z * change_z);
		if (sqrmag > maxChangeSq)
		{
			var mag = (float)Math.Sqrt(sqrmag);
			change_x = change_x / mag * maxChange;
			change_y = change_y / mag * maxChange;
			change_z = change_z / mag * maxChange;
		}

		target.X = current.X - change_x;
		target.Y = current.Y - change_y;
		target.Z = current.Z - change_z;

		float temp_x = (currentVelocity.X + (omega * change_x)) * (float)deltaTime;
		float temp_y = (currentVelocity.Y + (omega * change_y)) * (float)deltaTime;
		float temp_z = (currentVelocity.Z + (omega * change_z)) * (float)deltaTime;

		currentVelocity.X = (currentVelocity.X - (omega * temp_x)) * exp;
		currentVelocity.Y = (currentVelocity.Y - (omega * temp_y)) * exp;
		currentVelocity.Z = (currentVelocity.Z - (omega * temp_z)) * exp;

		float output_x = target.X + ((change_x + temp_x) * exp);
		float output_y = target.Y + ((change_y + temp_y) * exp);
		float output_z = target.Z + ((change_z + temp_z) * exp);

		// Prevent overshooting
		float origMinusCurrent_x = originalTo.X - current.X;
		float origMinusCurrent_y = originalTo.Y - current.Y;
		float origMinusCurrent_z = originalTo.Z - current.Z;
		float outMinusOrig_x = output_x - originalTo.X;
		float outMinusOrig_y = output_y - originalTo.Y;
		float outMinusOrig_z = output_z - originalTo.Z;

		if ((origMinusCurrent_x * outMinusOrig_x) + (origMinusCurrent_y * outMinusOrig_y) + (origMinusCurrent_z * outMinusOrig_z) > 0)
		{
			output_x = originalTo.X;
			output_y = originalTo.Y;
			output_z = originalTo.Z;

			currentVelocity.X = (output_x - originalTo.X) / (float)deltaTime;
			currentVelocity.Y = (output_y - originalTo.Y) / (float)deltaTime;
			currentVelocity.Z = (output_z - originalTo.Z) / (float)deltaTime;
		}

		return new Vector3(output_x, output_y, output_z);
	}
}
