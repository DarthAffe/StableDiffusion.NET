using System;

namespace StableDiffusion.NET.Helper.Images.Colors;

/// <summary>
/// Represents a generic color made of 4 bytes (alpha, red, green and blue)
/// </summary>
public interface IColor
{
    /// <summary>
    /// Gets the red-component of this color.
    /// </summary>
    byte R { get; }

    /// <summary>
    /// Gets the green-component of this color.
    /// </summary>
    byte G { get; }

    /// <summary>
    /// Gets the blue-component of this color.
    /// </summary>
    byte B { get; }

    /// <summary>
    /// Gets the alpha-component of this color.
    /// </summary>
    byte A { get; }

    /// <summary>
    /// Gets the color-format of this color.
    /// </summary>
    public static virtual ColorFormat ColorFormat => throw new NotSupportedException();
}