// ReSharper disable ConvertToAutoProperty

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET.Helper.Images.Colors;

/// <summary>
/// Represents a color in 24 bit RGB-format.
/// </summary>
[DebuggerDisplay("[A: {A}, R: {R}, G: {G}, B: {B}]")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct ColorRGB : IColor
{
    #region Properties & Fields
    
    /// <inheritdoc />
    public static ColorFormat ColorFormat => ColorFormat.RGB;
    
    private readonly byte _r;
    private readonly byte _g;
    private readonly byte _b;

    // ReSharper disable ConvertToAutoPropertyWhenPossible
    /// <inheritdoc />
    public byte A => byte.MaxValue;

    /// <inheritdoc />
    public byte R => _r;

    /// <inheritdoc />
    public byte G => _g;

    /// <inheritdoc />
    public byte B => _b;
    // ReSharper restore ConvertToAutoPropertyWhenPossible

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorRGB"/> class.
    /// </summary>
    /// <param name="r">The red-component of the color.</param>
    /// <param name="g">The green-component of the color.</param>
    /// <param name="b">The blue-component of the color.</param>
    public ColorRGB(byte r, byte g, byte b)
    {
        this._r = r;
        this._g = g;
        this._b = b;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override string ToString() => $"[A: {A}, R: {_r}, G: {_g}, B: {_b}]";

    #endregion
}