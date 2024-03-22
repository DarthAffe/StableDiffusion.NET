// DarthAffe 05.09.2023: Based on https://github.com/CommunityToolkit/dotnet/blob/b0d6c4f9c0cfb5d860400abb00b0ca1b3e94dfa4/src/CommunityToolkit.HighPerformance/Enumerables/ReadOnlyRefEnumerable%7BT%7D.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET.Helper.Images;

/// <summary>
/// A <see langword="ref"/> <see langword="struct"/> that iterates readonly items from arbitrary memory locations.
/// </summary>
/// <typeparam name="T">The type of items to enumerate.</typeparam>
public readonly ref struct ReadOnlyRefEnumerable<T>
{
    #region Properties & Fields

    /// <summary>
    /// The <see cref="ReadOnlySpan{T}"/> instance pointing to the first item in the target memory area.
    /// </summary>
    /// <remarks>The <see cref="ReadOnlySpan{T}.Length"/> field maps to the total available length.</remarks>
    private readonly ReadOnlySpan<T> _span;

    /// <summary>
    /// The distance between items in the sequence to enumerate.
    /// </summary>
    /// <remarks>The distance refers to <typeparamref name="T"/> items, not byte offset.</remarks>
    private readonly int _step;

    /// <summary>
    /// Gets the total available length for the sequence.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    /// <summary>
    /// Gets the element at the specified zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid.
    /// </exception>
    public ref readonly T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((uint)index >= (uint)Length) throw new IndexOutOfRangeException();

            ref T r0 = ref MemoryMarshal.GetReference(_span);
            nint offset = (nint)(uint)index * (nint)(uint)_step;
            ref T ri = ref Unsafe.Add(ref r0, offset);

            return ref ri;
        }
    }

    /// <summary>
    /// Gets the element at the specified zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid.
    /// </exception>
    public ref readonly T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[index.GetOffset(Length)];
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyRefEnumerable{T}"/> struct.
    /// </summary>
    /// <param name="reference">A reference to the first item of the sequence.</param>
    /// <param name="length">The number of items in the sequence.</param>
    /// <param name="step">The distance between items in the sequence to enumerate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlyRefEnumerable(in T reference, int length, int step)
    {
        this._step = step;

        _span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(reference), length);
    }

    #endregion

    #region Methods

    /// <inheritdoc cref="System.Collections.IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(_span, _step);

    public T[] ToArray()
    {
        int length = _span.Length;

        // Empty array if no data is mapped
        if (length == 0)
            return Array.Empty<T>();

        T[] array = new T[length];
        CopyTo(array);

        return array;
    }

    /// <summary>
    /// Copies the contents of this <see cref="ReadOnlyRefEnumerable{T}"/> into a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="ReadOnlyRefEnumerable{T}"/> instance.
    /// </exception>
    public void CopyTo(Span<T> destination)
    {
        if (_step == 1)
        {
            _span.CopyTo(destination);
            return;
        }

        ref T sourceRef = ref MemoryMarshal.GetReference(_span);
        int length = _span.Length;
        if ((uint)destination.Length < (uint)length)
            throw new ArgumentException("The target span is too short to copy all the current items to.");

        ref T destinationRef = ref MemoryMarshal.GetReference(destination);

        CopyTo(ref sourceRef, ref destinationRef, (nint)(uint)length, (nint)(uint)_step);
    }

    /// <summary>
    /// Attempts to copy the current <see cref="ReadOnlyRefEnumerable{T}"/> instance to a destination <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Span{T}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo(Span<T> destination)
    {
        if (destination.Length >= _span.Length)
        {
            CopyTo(destination);
            return true;
        }

        return false;
    }

    private static void CopyTo(ref T sourceRef, ref T destinationRef, nint length, nint sourceStep)
    {
        nint sourceOffset = 0;
        nint destinationOffset = 0;

        while (length >= 8)
        {
            Unsafe.Add(ref destinationRef, destinationOffset + 0) = Unsafe.Add(ref sourceRef, sourceOffset);
            Unsafe.Add(ref destinationRef, destinationOffset + 1) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 2) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 3) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 4) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 5) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 6) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 7) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

            length -= 8;
            sourceOffset += sourceStep;
            destinationOffset += 8;
        }

        if (length >= 4)
        {
            Unsafe.Add(ref destinationRef, destinationOffset + 0) = Unsafe.Add(ref sourceRef, sourceOffset);
            Unsafe.Add(ref destinationRef, destinationOffset + 1) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 2) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);
            Unsafe.Add(ref destinationRef, destinationOffset + 3) = Unsafe.Add(ref sourceRef, sourceOffset += sourceStep);

            length -= 4;
            sourceOffset += sourceStep;
            destinationOffset += 4;
        }

        while (length > 0)
        {
            Unsafe.Add(ref destinationRef, destinationOffset) = Unsafe.Add(ref sourceRef, sourceOffset);

            length -= 1;
            sourceOffset += sourceStep;
            destinationOffset += 1;
        }
    }

    #endregion

    /// <summary>
    /// A custom enumerator type to traverse items within a <see cref="ReadOnlyRefEnumerable{T}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
        #region Properties & Fields

        /// <inheritdoc cref="ReadOnlyRefEnumerable{T}._span"/>
        private readonly ReadOnlySpan<T> _span;

        /// <inheritdoc cref="ReadOnlyRefEnumerable{T}._step"/>
        private readonly int _step;

        /// <summary>
        /// The current position in the sequence.
        /// </summary>
        private int _position;

        /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current"/>
        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref T r0 = ref MemoryMarshal.GetReference(_span);

                nint offset = (nint)(uint)_position * (nint)(uint)_step;
                ref T ri = ref Unsafe.Add(ref r0, offset);

                return ref ri;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> instance with the info on the items to traverse.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ReadOnlySpan<T> span, int step)
        {
            this._span = span;
            this._step = step;

            _position = -1;
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_position < _span.Length;

        #endregion
    }
}
