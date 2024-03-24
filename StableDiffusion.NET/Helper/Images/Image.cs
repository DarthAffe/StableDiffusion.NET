using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using StableDiffusion.NET.Helper.Images.Colors;

namespace StableDiffusion.NET.Helper.Images;

/// <inheritdoc />
public sealed class Image<TColor> : IImage
    where TColor : struct, IColor
{
    #region Properties & Fields

    private readonly byte[] _buffer;

    private readonly int _x;
    private readonly int _y;
    private readonly int _stride;

    /// <inheritdoc />
    public ColorFormat ColorFormat
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => TColor.ColorFormat;
    }

    /// <inheritdoc />
    public int Width { get; }

    /// <inheritdoc />
    public int Height { get; }

    /// <inheritdoc />
    public int SizeInBytes => Width * Height * ColorFormat.BytesPerPixel;

    #endregion

    #region Indexer

    /// <inheritdoc />
    public IColor this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height)) throw new IndexOutOfRangeException();

            return MemoryMarshal.Cast<byte, TColor>(_buffer)[((_y + y) * _stride) + (_x + x)];
        }
    }

    /// <inheritdoc />
    public IImage this[int x, int y, int width, int height]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((x < 0) || (y < 0) || (width <= 0) || (height <= 0) || ((x + width) > Width) || ((y + height) > Height)) throw new IndexOutOfRangeException();

            return new Image<TColor>(_buffer, _x + x, _y + y, width, height, _stride);
        }
    }

    /// <inheritdoc />
    public IImage.IImageRows Rows
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new ImageRows(_buffer, _x, _y, Width, Height, _stride);
    }

    /// <inheritdoc />
    public IImage.IImageColumns Columns
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new ImageColumns(_buffer, _x, _y, Width, Height, _stride);
    }

    #endregion

    #region Constructors

    public Image(byte[] buffer, int x, int y, int width, int height, int stride)
    {
        this._buffer = buffer;
        this._x = x;
        this._y = y;
        this.Width = width;
        this.Height = height;
        this._stride = stride;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public void CopyTo(in Span<byte> destination)
    {
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        if (destination.Length < SizeInBytes) throw new ArgumentException("The destination is too small to fit this image.", nameof(destination));

        int targetStride = Width * ColorFormat.BytesPerPixel;
        IImage.IImageRows rows = Rows;
        Span<byte> target = destination;
        foreach (IImage.IImageRow row in rows)
        {
            row.CopyTo(target);
            target = target[targetStride..];
        }
    }

    /// <inheritdoc />
    public byte[] ToArray()
    {
        byte[] array = new byte[SizeInBytes];
        CopyTo(array);
        return array;
    }

    /// <inheritdoc />
    public RefImage<T> AsRefImage<T>()
        where T : struct, IColor
    {
        if (typeof(T) != typeof(TColor)) throw new ArgumentException("The requested color format does not fit this image.", nameof(T));

        return new RefImage<T>(MemoryMarshal.Cast<byte, T>(_buffer), _x, _y, Width, Height, _stride);
    }

    /// <inheritdoc />
    public IEnumerator<IColor> GetEnumerator()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                yield return this[x, y];
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Indexer-Classes

    /// <inheritdoc />
    private sealed class ImageRows : IImage.IImageRows
    {
        #region Properties & Fields

        private readonly byte[] _buffer;
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;

        /// <inheritdoc />
        public int Count => _height;

        #endregion

        #region Indexer

        /// <inheritdoc />
        public IImage.IImageRow this[int row]
        {
            get
            {
                if ((row < 0) || (row >= _height)) throw new IndexOutOfRangeException();

                return new ImageRow(_buffer, (((row + _y) * _stride) + _x), _width);
            }
        }

        #endregion

        #region Constructors

        internal ImageRows(byte[] buffer, int x, int y, int width, int height, int stride)
        {
            this._buffer = buffer;
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
            this._stride = stride;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public IEnumerator<IImage.IImageRow> GetEnumerator()
        {
            for (int y = 0; y < _height; y++)
                yield return this[y];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    /// <inheritdoc />
    private sealed class ImageRow : IImage.IImageRow
    {
        #region Properties & Fields

        private readonly byte[] _buffer;
        private readonly int _start;
        private readonly int _length;

        /// <inheritdoc />
        public int Length => _length;

        /// <inheritdoc />
        public int SizeInBytes => Length * TColor.ColorFormat.BytesPerPixel;

        #endregion

        #region Indexer

        /// <inheritdoc />
        public IColor this[int x]
        {
            get
            {
                if ((x < 0) || (x >= _length)) throw new IndexOutOfRangeException();

                ReadOnlySpan<TColor> row = MemoryMarshal.Cast<byte, TColor>(_buffer)[_start..];
                return row[x];
            }
        }

        #endregion

        #region Constructors

        internal ImageRow(byte[] buffer, int start, int length)
        {
            this._buffer = buffer;
            this._start = start;
            this._length = length;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void CopyTo(in Span<byte> destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (destination.Length < SizeInBytes) throw new ArgumentException("The destination is too small to fit this image.", nameof(destination));

            MemoryMarshal.Cast<byte, TColor>(_buffer).Slice(_start, _length).CopyTo(MemoryMarshal.Cast<byte, TColor>(destination));
        }

        /// <inheritdoc />
        public byte[] ToArray()
        {
            byte[] array = new byte[SizeInBytes];
            CopyTo(array);
            return array;
        }

        /// <inheritdoc />
        public IEnumerator<IColor> GetEnumerator()
        {
            for (int x = 0; x < _length; x++)
                yield return this[x];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    /// <inheritdoc />
    private sealed class ImageColumns : IImage.IImageColumns
    {
        #region Properties & Fields

        private readonly byte[] _buffer;
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;

        /// <inheritdoc />
        public int Count => _width;

        #endregion

        #region Indexer

        /// <inheritdoc />
        public IImage.IImageColumn this[int column]
        {
            get
            {
                if ((column < 0) || (column >= _width)) throw new IndexOutOfRangeException();

                return new ImageColumn(_buffer, (_y * _stride) + _x + column, _height, _stride);
            }
        }

        #endregion

        #region Constructors

        internal ImageColumns(byte[] buffer, int x, int y, int width, int height, int stride)
        {
            this._buffer = buffer;
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
            this._stride = stride;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public IEnumerator<IImage.IImageColumn> GetEnumerator()
        {
            for (int y = 0; y < _height; y++)
                yield return this[y];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    /// <inheritdoc />
    private sealed class ImageColumn : IImage.IImageColumn
    {
        #region Properties & Fields

        private readonly byte[] _buffer;
        private readonly int _start;
        private readonly int _length;
        private readonly int _step;

        /// <inheritdoc />
        public int Length => _length;

        /// <inheritdoc />
        public int SizeInBytes => Length * TColor.ColorFormat.BytesPerPixel;

        #endregion

        #region Indexer

        /// <inheritdoc />
        public IColor this[int y]
        {
            get
            {
                if ((y < 0) || (y >= _length)) throw new IndexOutOfRangeException();

                ReadOnlySpan<TColor> data = MemoryMarshal.Cast<byte, TColor>(_buffer)[_start..];
                return data[y * _step];
            }
        }

        #endregion

        #region Constructors

        internal ImageColumn(byte[] buffer, int start, int length, int step)
        {
            this._buffer = buffer;
            this._start = start;
            this._length = length;
            this._step = step;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void CopyTo(in Span<byte> destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (destination.Length < SizeInBytes) throw new ArgumentException("The destination is too small to fit this image.", nameof(destination));

            if (_step == 1)
                _buffer.AsSpan(_start, SizeInBytes).CopyTo(destination);
            else
            {
                ReadOnlySpan<TColor> data = MemoryMarshal.Cast<byte, TColor>(_buffer)[_start..];
                Span<TColor> target = MemoryMarshal.Cast<byte, TColor>(destination);
                for (int i = 0; i < Length; i++)
                    target[i] = data[i * _step];
            }
        }

        /// <inheritdoc />
        public byte[] ToArray()
        {
            byte[] array = new byte[SizeInBytes];
            CopyTo(array);
            return array;
        }

        /// <inheritdoc />
        public IEnumerator<IColor> GetEnumerator()
        {
            for (int y = 0; y < _length; y++)
                yield return this[y];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    #endregion
}