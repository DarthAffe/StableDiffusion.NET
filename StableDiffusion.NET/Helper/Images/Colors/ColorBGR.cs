// ReSharper disable ConvertToAutoProperty

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET.Helper.Images.Colors;

/// <summary>
/// Represents a color in 24 bit BGR-format.
/// </summary>
[DebuggerDisplay("[A: {A}, R: {R}, G: {G}, B: {B}]")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct ColorBGR : IColor
{
    #region Properties & Fields
    
    /// <inheritdoc />
    public static ColorFormat ColorFormat => ColorFormat.BGR;
    
    private readonly byte _b;
    private readonly byte _g;
    private readonly byte _r;

    // ReSharper disable ConvertToAutoPropertyWhenPossible
    /// <inheritdoc />
    public byte A => byte.MaxValue;

    /// <inheritdoc />
    public byte B => _b;

    /// <inheritdoc />
    public byte G => _g;

    /// <inheritdoc />
    public byte R => _r;
    // ReSharper restore ConvertToAutoPropertyWhenPossible

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorBGR"/> class.
    /// </summary>
    /// <param name="b">The blue-component of the color.</param>
    /// <param name="g">The green-component of the color.</param>
    /// <param name="r">The red-component of the color.</param>
    public ColorBGR(byte b, byte g, byte r)
    {
        this._b = b;
        this._g = g;
        this._r = r;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override string ToString() => $"[A: {A}, R: {_r}, G: {_g}, B: {_b}]";

    #endregion
}