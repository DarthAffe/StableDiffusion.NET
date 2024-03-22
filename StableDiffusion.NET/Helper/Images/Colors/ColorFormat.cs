namespace StableDiffusion.NET.Helper.Images.Colors;

/// <summary>
/// Represents a color format.
/// </summary>
public readonly struct ColorFormat
{
    #region Instances

    public static readonly ColorFormat BGRA = new(1, 4);
    public static readonly ColorFormat ABGR = new(2, 4);
    public static readonly ColorFormat RGBA = new(3, 4);
    public static readonly ColorFormat ARGB = new(4, 4);
    public static readonly ColorFormat BGR = new(5, 3);
    public static readonly ColorFormat RGB = new(6, 3);

    #endregion

    #region Properties & Fields

    /// <summary>
    /// Gets the Id of the color-format.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// Gets the Bytes per pixel for this color-format.
    /// </summary>
    public readonly int BytesPerPixel;

    #endregion

    #region Constructors

    private ColorFormat(int id, int bytesPerPixel)
    {
        this.Id = id;
        this.BytesPerPixel = bytesPerPixel;
    }

    #endregion

    #region Methods

    public bool Equals(ColorFormat other) => Id == other.Id;
    public override bool Equals(object? obj) => obj is ColorFormat other && Equals(other);

    public override int GetHashCode() => Id;

    #endregion

    #region Operators

    public static bool operator ==(ColorFormat left, ColorFormat right) => left.Equals(right);
    public static bool operator !=(ColorFormat left, ColorFormat right) => !(left == right);

    #endregion
}