using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using StableDiffusion.NET.Helper.Images.Colors;

namespace StableDiffusion.NET.Helper.Images;

public readonly ref struct RefImage<TColor>
    where TColor : struct, IColor
{
    #region Properties & Fields

    private readonly ReadOnlySpan<TColor> _pixels;

    private readonly int _x;
    private readonly int _y;

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the stride (entries per row) of the underlying buffer.
    /// Only useful if you want to work with a pinned buffer.
    /// </summary>
    public int RawStride { get; }

    #endregion

    #region Indexer

    public ref readonly TColor this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height)) throw new IndexOutOfRangeException();

            ref TColor r0 = ref MemoryMarshal.GetReference(_pixels);
            nint offset = (nint)(uint)((_y + y) * RawStride) + (_x + x);
            return ref Unsafe.Add(ref r0, offset);
        }
    }

    public RefImage<TColor> this[int x, int y, int width, int height]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((x < 0) || (y < 0) || (width <= 0) || (height <= 0) || ((x + width) > Width) || ((y + height) > Height)) throw new IndexOutOfRangeException();

            return new RefImage<TColor>(_pixels, _x + x, _y + y, width, height, RawStride);
        }
    }

    public ImageRows Rows
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pixels, _x, _y, Width, Height, RawStride);
    }
    public ImageColumns Columns
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pixels, _x, _y, Width, Height, RawStride);
    }

    #endregion

    #region Constructors

    public RefImage(ReadOnlySpan<TColor> pixels, int x, int y, int width, int height, int stride)
    {
        this._pixels = pixels;
        this._x = x;
        this._y = y;
        this.Width = width;
        this.Height = height;
        this.RawStride = stride;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Copies the contents of this <see cref="RefImage{TColor}"/> into a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="RefImage{TColor}"/> instance.
    /// </exception>
    public void CopyTo(in Span<TColor> destination)
    {
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        if (destination.Length < (Width * Height)) throw new ArgumentException("The destination is too small to fit this image.", nameof(destination));

        ImageRows rows = Rows;
        Span<TColor> target = destination;
        foreach (ReadOnlyRefEnumerable<TColor> row in rows)
        {
            row.CopyTo(target);
            target = target[Width..];
        }
    }

    /// <summary>
    /// Allocates a new array and copies this <see cref="RefImage{TColor}"/> into it.
    /// </summary>
    /// <returns>The new array containing the data of this <see cref="RefImage{TColor}"/>.</returns>
    public TColor[] ToArray()
    {
        TColor[] array = new TColor[Width * Height];
        CopyTo(array);
        return array;
    }

    /// <summary>
    /// Returns a reference to the first element of this image inside the full image buffer.
    /// </summary>
    public ref readonly TColor GetPinnableReference()
    {
        if (_pixels.Length == 0)
            return ref Unsafe.NullRef<TColor>();

        int offset = (_y * RawStride) + _x;
        return  ref MemoryMarshal.GetReference(_pixels[offset..]);
    }

    /// <inheritdoc cref="System.Collections.IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImageEnumerator GetEnumerator() => new(_pixels);

    #endregion

    public ref struct ImageEnumerator
    {
        #region Properties & Fields

        private readonly ReadOnlySpan<TColor> _pixels;
        private int _position;

        /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current"/>
        public TColor Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pixels[_position];
        }

        #endregion

        #region Constructors


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ImageEnumerator(ReadOnlySpan<TColor> pixels)
        {
            this._pixels = pixels;

            _position = -1;
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_position < _pixels.Length;

        #endregion
    }

    #region Indexer-Structs

    public readonly ref struct ImageRows
    {
        #region Properties & Fields

        private readonly ReadOnlySpan<TColor> _pixels;
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;

        public int Count => _height;

        #endregion

        #region Indexer

        public readonly ReadOnlyRefEnumerable<TColor> this[int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((row < 0) || (row > _height)) throw new IndexOutOfRangeException();

                ref TColor r0 = ref MemoryMarshal.GetReference(_pixels);
                ref TColor rr = ref Unsafe.Add(ref r0, (nint)(uint)(((row + _y) * _stride) + _x));

                return new ReadOnlyRefEnumerable<TColor>(rr, _width, 1);
            }
        }

        #endregion

        #region Constructors

        public ImageRows(ReadOnlySpan<TColor> pixels, int x, int y, int width, int height, int stride)
        {
            this._pixels = pixels;
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
            this._stride = stride;
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="System.Collections.IEnumerable.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageRowsEnumerator GetEnumerator() => new(this);

        #endregion

        public ref struct ImageRowsEnumerator
        {
            #region Properties & Fields

            private readonly ImageRows _rows;
            private int _position;

            /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current"/>
            public ReadOnlyRefEnumerable<TColor> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _rows[_position];
            }

            #endregion

            #region Constructors


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ImageRowsEnumerator(ImageRows rows)
            {
                this._rows = rows;

                _position = -1;
            }

            #endregion

            #region Methods

            /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_position < _rows._height;

            #endregion
        }
    }

    public readonly ref struct ImageColumns
    {
        #region Properties & Fields

        private readonly ReadOnlySpan<TColor> _pixels;
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;

        public int Count => _width;

        #endregion

        #region Indexer

        public ReadOnlyRefEnumerable<TColor> this[int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((column < 0) || (column > _width)) throw new IndexOutOfRangeException();

                ref TColor r0 = ref MemoryMarshal.GetReference(_pixels);
                ref TColor rc = ref Unsafe.Add(ref r0, (nint)(uint)((_y * _stride) + (column + _x)));

                return new ReadOnlyRefEnumerable<TColor>(rc, _height, _stride);
            }
        }

        #endregion

        #region Constructors

        public ImageColumns(ReadOnlySpan<TColor> pixels, int x, int y, int width, int height, int stride)
        {
            this._pixels = pixels;
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
            this._stride = stride;
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="System.Collections.IEnumerable.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageColumnsEnumerator GetEnumerator() => new(this);

        #endregion

        public ref struct ImageColumnsEnumerator
        {
            #region Properties & Fields

            private readonly ImageColumns _columns;
            private int _position;

            /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current"/>
            public ReadOnlyRefEnumerable<TColor> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _columns[_position];
            }

            #endregion

            #region Constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ImageColumnsEnumerator(ImageColumns columns)
            {
                this._columns = columns;
                this._position = -1;
            }

            #endregion

            #region Methods

            /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_position < _columns._width;

            #endregion
        }
    }

    #endregion
}