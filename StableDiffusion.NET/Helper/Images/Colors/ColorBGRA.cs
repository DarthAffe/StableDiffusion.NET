// ReSharper disable ConvertToAutoProperty

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET.Helper.Images.Colors;

/// <summary>
/// Represents a color in 32 bit BGRA-format.
/// </summary>
[DebuggerDisplay("[A: {A}, R: {R}, G: {G}, B: {B}]")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct ColorBGRA : IColor
{
    #region Properties & Fields
    
    /// <inheritdoc />
    public static ColorFormat ColorFormat => ColorFormat.BGRA;
    
    private readonly byte _b;
    private readonly byte _g;
    private readonly byte _r;
    private readonly byte _a;

    // ReSharper disable ConvertToAutoPropertyWhenPossible
    /// <inheritdoc />
    public byte B => _b;

    /// <inheritdoc />
    public byte G => _g;

    /// <inheritdoc />
    public byte R => _r;

    /// <inheritdoc />
    public byte A => _a;
    // ReSharper restore ConvertToAutoPropertyWhenPossible

#endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorBGRA"/> class.
    /// </summary>
    /// <param name="b">The blue-component of the color.</param>
    /// <param name="g">The green-component of the color.</param>
    /// <param name="r">The red-component of the color.</param>
    /// <param name="a">The alpha-component of the color.</param>
    public ColorBGRA(byte b, byte g, byte r, byte a)
    {
        this._b = b;
        this._g = g;
        this._r = r;
        this._a = a;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override string ToString() => $"[A: {_a}, R: {_r}, G: {_g}, B: {_b}]";

    #endregion
}