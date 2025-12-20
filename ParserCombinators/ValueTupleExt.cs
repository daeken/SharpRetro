using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParserCombinators;

/// <summary>
/// Represents an 8-tuple, or octuple, as a value type.
/// </summary>
/// <typeparam name="T1">The type of the tuple's first component.</typeparam>
/// <typeparam name="T2">The type of the tuple's second component.</typeparam>
/// <typeparam name="T3">The type of the tuple's third component.</typeparam>
/// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
/// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
/// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
/// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
/// <typeparam name="T8">The type of the tuple's eighth component.</typeparam>
[Serializable]
[StructLayout(LayoutKind.Auto)]
public struct EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>
: IEquatable<EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8>>, ITuple
{
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's first component.
    /// </summary>
    public T1 Item1;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's second component.
    /// </summary>
    public T2 Item2;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's third component.
    /// </summary>
    public T3 Item3;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's fourth component.
    /// </summary>
    public T4 Item4;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's fifth component.
    /// </summary>
    public T5 Item5;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's sixth component.
    /// </summary>
    public T6 Item6;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's seventh component.
    /// </summary>
    public T7 Item7;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance's eighth component.
    /// </summary>
    public T8 Item8;

    /// <summary>
    /// Initializes a new instance of the <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> value type.
    /// </summary>
    /// <param name="item1">The value of the tuple's first component.</param>
    /// <param name="item2">The value of the tuple's second component.</param>
    /// <param name="item3">The value of the tuple's third component.</param>
    /// <param name="item4">The value of the tuple's fourth component.</param>
    /// <param name="item5">The value of the tuple's fifth component.</param>
    /// <param name="item6">The value of the tuple's sixth component.</param>
    /// <param name="item7">The value of the tuple's seventh component.</param>
    /// <param name="item8">The value of the tuple's eighth component.</param>
    public EValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
        Item4 = item4;
        Item5 = item5;
        Item6 = item6;
        Item7 = item7;
        Item8 = item8;
    }

    /// <summary>
    /// Returns a value that indicates whether the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
    /// <list type="bullet">
    ///     <item><description>It is a <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> value type.</description></item>
    ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
    ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
    /// </list>
    /// </remarks>
    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> tuple && Equals(tuple);
    }

    /// <summary>
    /// Returns a value that indicates whether the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/>
    /// instance is equal to a specified <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
    /// </summary>
    /// <param name="other">The tuple to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
    /// are equal to that of the current instance, using the default comparer for that field's type.
    /// </remarks>
    public bool Equals(EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other)
    {
        return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
            && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
            && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
            && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
            && EqualityComparer<T5>.Default.Equals(Item5, other.Item5)
            && EqualityComparer<T6>.Default.Equals(Item6, other.Item6)
            && EqualityComparer<T7>.Default.Equals(Item7, other.Item7)
            && EqualityComparer<T8>.Default.Equals(Item8, other.Item8);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer) =>
        other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> vt &&
        comparer.Equals(Item1, vt.Item1) &&
        comparer.Equals(Item2, vt.Item2) &&
        comparer.Equals(Item3, vt.Item3) &&
        comparer.Equals(Item4, vt.Item4) &&
        comparer.Equals(Item5, vt.Item5) &&
        comparer.Equals(Item6, vt.Item6) &&
        comparer.Equals(Item7, vt.Item7) &&
        comparer.Equals(Item8, vt.Item8);

    int IComparable.CompareTo(object other)
    {
        if (other is not null)
        {
            if (other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> objTuple)
            {
                return CompareTo(objTuple);
            }

            throw new NotSupportedException();
        }

        return 1;
    }

    /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
    /// <param name="other">An instance to compare.</param>
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
    /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
    /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
    /// than <paramref name="other"/>.
    /// </returns>
    public int CompareTo(EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> other)
    {
        int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
        if (c != 0) return c;

        c = Comparer<T2>.Default.Compare(Item2, other.Item2);
        if (c != 0) return c;

        c = Comparer<T3>.Default.Compare(Item3, other.Item3);
        if (c != 0) return c;

        c = Comparer<T4>.Default.Compare(Item4, other.Item4);
        if (c != 0) return c;

        c = Comparer<T5>.Default.Compare(Item5, other.Item5);
        if (c != 0) return c;

        c = Comparer<T6>.Default.Compare(Item6, other.Item6);
        if (c != 0) return c;

        c = Comparer<T7>.Default.Compare(Item7, other.Item7);
        if (c != 0) return c;

        return Comparer<T8>.Default.Compare(Item8, other.Item8);
    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        if (other is not null)
        {
            if (other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8> objTuple)
            {
                int c = comparer.Compare(Item1, objTuple.Item1);
                if (c != 0) return c;

                c = comparer.Compare(Item2, objTuple.Item2);
                if (c != 0) return c;

                c = comparer.Compare(Item3, objTuple.Item3);
                if (c != 0) return c;

                c = comparer.Compare(Item4, objTuple.Item4);
                if (c != 0) return c;

                c = comparer.Compare(Item5, objTuple.Item5);
                if (c != 0) return c;

                c = comparer.Compare(Item6, objTuple.Item6);
                if (c != 0) return c;

                c = comparer.Compare(Item7, objTuple.Item7);
                if (c != 0) return c;

                return comparer.Compare(Item8, objTuple.Item8);
            }

            throw new NotSupportedException();
        }

        return 1;
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T9}"/> instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Item1?.GetHashCode() ?? 0,
                                Item2?.GetHashCode() ?? 0,
                                Item3?.GetHashCode() ?? 0,
                                Item4?.GetHashCode() ?? 0,
                                Item5?.GetHashCode() ?? 0,
                                Item6?.GetHashCode() ?? 0,
                                Item7?.GetHashCode() ?? 0,
                                Item8?.GetHashCode() ?? 0);
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return GetHashCodeCore(comparer);
    }

    private int GetHashCodeCore(IEqualityComparer comparer)
    {
        return HashCode.Combine(comparer.GetHashCode(Item1!), comparer.GetHashCode(Item2!), comparer.GetHashCode(Item3!),
                                comparer.GetHashCode(Item4!), comparer.GetHashCode(Item5!), comparer.GetHashCode(Item6!),
                                comparer.GetHashCode(Item7!));
    }

    /// <summary>
    /// Returns a string that represents the value of this <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance.
    /// </summary>
    /// <returns>The string representation of this <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> instance.</returns>
    /// <remarks>
    /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9)</c>.
    /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
    /// </remarks>
    public override string ToString()
    {
        return "(" + Item1?.ToString() + ", " + Item2?.ToString() + ", " + Item3?.ToString() + ", " + Item4?.ToString() + ", " + Item5?.ToString() + ", " + Item6?.ToString() + ", " + Item7?.ToString() + ", " + Item8?.ToString() + ")";
    }

    /// <summary>
    /// The number of positions in this data structure.
    /// </summary>
    int ITuple.Length => 8;

    /// <summary>
    /// Get the element at position <param name="index"/>.
    /// </summary>
    object ITuple.this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return Item1;
                case 1:
                    return Item2;
                case 2:
                    return Item3;
                case 3:
                    return Item4;
                case 4:
                    return Item5;
                case 5:
                    return Item6;
                case 6:
                    return Item7;
                case 7:
                    return Item8;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}

/// <summary>
/// Represents an 9-tuple, or nonuple, as a value type.
/// </summary>
/// <typeparam name="T1">The type of the tuple's first component.</typeparam>
/// <typeparam name="T2">The type of the tuple's second component.</typeparam>
/// <typeparam name="T3">The type of the tuple's third component.</typeparam>
/// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
/// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
/// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
/// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
/// <typeparam name="T8">The type of the tuple's eighth component.</typeparam>
/// <typeparam name="T9">The type of the tuple's ninth component.</typeparam>
[Serializable]
[StructLayout(LayoutKind.Auto)]
public struct EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
: IEquatable<EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>>, ITuple
{
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's first component.
    /// </summary>
    public T1 Item1;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's second component.
    /// </summary>
    public T2 Item2;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's third component.
    /// </summary>
    public T3 Item3;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's fourth component.
    /// </summary>
    public T4 Item4;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's fifth component.
    /// </summary>
    public T5 Item5;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's sixth component.
    /// </summary>
    public T6 Item6;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's seventh component.
    /// </summary>
    public T7 Item7;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's eighth component.
    /// </summary>
    public T8 Item8;
    /// <summary>
    /// The current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance's ninth component.
    /// </summary>
    public T9 Item9;

    /// <summary>
    /// Initializes a new instance of the <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> value type.
    /// </summary>
    /// <param name="item1">The value of the tuple's first component.</param>
    /// <param name="item2">The value of the tuple's second component.</param>
    /// <param name="item3">The value of the tuple's third component.</param>
    /// <param name="item4">The value of the tuple's fourth component.</param>
    /// <param name="item5">The value of the tuple's fifth component.</param>
    /// <param name="item6">The value of the tuple's sixth component.</param>
    /// <param name="item7">The value of the tuple's seventh component.</param>
    /// <param name="item8">The value of the tuple's eighth component.</param>
    /// <param name="item9">The value of the tuple's ninth component.</param>
    public EValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
        Item4 = item4;
        Item5 = item5;
        Item6 = item6;
        Item7 = item7;
        Item8 = item8;
        Item9 = item9;
    }

    /// <summary>
    /// Returns a value that indicates whether the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
    /// <list type="bullet">
    ///     <item><description>It is a <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> value type.</description></item>
    ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
    ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
    /// </list>
    /// </remarks>
    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> tuple && Equals(tuple);
    }

    /// <summary>
    /// Returns a value that indicates whether the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>
    /// instance is equal to a specified <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
    /// </summary>
    /// <param name="other">The tuple to compare with this instance.</param>
    /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
    /// are equal to that of the current instance, using the default comparer for that field's type.
    /// </remarks>
    public bool Equals(EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> other)
    {
        return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
            && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
            && EqualityComparer<T3>.Default.Equals(Item3, other.Item3)
            && EqualityComparer<T4>.Default.Equals(Item4, other.Item4)
            && EqualityComparer<T5>.Default.Equals(Item5, other.Item5)
            && EqualityComparer<T6>.Default.Equals(Item6, other.Item6)
            && EqualityComparer<T7>.Default.Equals(Item7, other.Item7)
            && EqualityComparer<T8>.Default.Equals(Item8, other.Item8)
            && EqualityComparer<T9>.Default.Equals(Item9, other.Item9);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer) =>
        other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> vt &&
        comparer.Equals(Item1, vt.Item1) &&
        comparer.Equals(Item2, vt.Item2) &&
        comparer.Equals(Item3, vt.Item3) &&
        comparer.Equals(Item4, vt.Item4) &&
        comparer.Equals(Item5, vt.Item5) &&
        comparer.Equals(Item6, vt.Item6) &&
        comparer.Equals(Item7, vt.Item7) &&
        comparer.Equals(Item8, vt.Item8) &&
        comparer.Equals(Item9, vt.Item9);

    int IComparable.CompareTo(object other)
    {
        if (other is not null)
        {
            if (other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> objTuple)
            {
                return CompareTo(objTuple);
            }

            throw new NotSupportedException();
        }

        return 1;
    }

    /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
    /// <param name="other">An instance to compare.</param>
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
    /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
    /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
    /// than <paramref name="other"/>.
    /// </returns>
    public int CompareTo(EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> other)
    {
        int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
        if (c != 0) return c;

        c = Comparer<T2>.Default.Compare(Item2, other.Item2);
        if (c != 0) return c;

        c = Comparer<T3>.Default.Compare(Item3, other.Item3);
        if (c != 0) return c;

        c = Comparer<T4>.Default.Compare(Item4, other.Item4);
        if (c != 0) return c;

        c = Comparer<T5>.Default.Compare(Item5, other.Item5);
        if (c != 0) return c;

        c = Comparer<T6>.Default.Compare(Item6, other.Item6);
        if (c != 0) return c;

        c = Comparer<T7>.Default.Compare(Item7, other.Item7);
        if (c != 0) return c;

        c = Comparer<T8>.Default.Compare(Item8, other.Item8);
        if (c != 0) return c;

        return Comparer<T9>.Default.Compare(Item9, other.Item9);
    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        if (other is not null)
        {
            if (other is EValueTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> objTuple)
            {
                int c = comparer.Compare(Item1, objTuple.Item1);
                if (c != 0) return c;

                c = comparer.Compare(Item2, objTuple.Item2);
                if (c != 0) return c;

                c = comparer.Compare(Item3, objTuple.Item3);
                if (c != 0) return c;

                c = comparer.Compare(Item4, objTuple.Item4);
                if (c != 0) return c;

                c = comparer.Compare(Item5, objTuple.Item5);
                if (c != 0) return c;

                c = comparer.Compare(Item6, objTuple.Item6);
                if (c != 0) return c;

                c = comparer.Compare(Item7, objTuple.Item7);
                if (c != 0) return c;

                c = comparer.Compare(Item8, objTuple.Item8);
                if (c != 0) return c;

                return comparer.Compare(Item9, objTuple.Item9);
            }

            throw new NotSupportedException();
        }

        return 1;
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T9}"/> instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Item1?.GetHashCode() ?? 0,
                                Item2?.GetHashCode() ?? 0,
                                Item3?.GetHashCode() ?? 0,
                                Item4?.GetHashCode() ?? 0,
                                Item5?.GetHashCode() ?? 0,
                                Item6?.GetHashCode() ?? 0,
                                Item7?.GetHashCode() ?? 0);
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return GetHashCodeCore(comparer);
    }

    private int GetHashCodeCore(IEqualityComparer comparer)
    {
        return HashCode.Combine(comparer.GetHashCode(Item1!), comparer.GetHashCode(Item2!), comparer.GetHashCode(Item3!),
                                comparer.GetHashCode(Item4!), comparer.GetHashCode(Item5!), comparer.GetHashCode(Item6!),
                                comparer.GetHashCode(Item7!));
    }

    /// <summary>
    /// Returns a string that represents the value of this <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance.
    /// </summary>
    /// <returns>The string representation of this <see cref="EValueTuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance.</returns>
    /// <remarks>
    /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9)</c>.
    /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
    /// </remarks>
    public override string ToString()
    {
        return "(" + Item1?.ToString() + ", " + Item2?.ToString() + ", " + Item3?.ToString() + ", " + Item4?.ToString() + ", " + Item5?.ToString() + ", " + Item6?.ToString() + ", " + Item7?.ToString() + ", " + Item8?.ToString() + ", " + Item9.ToString() + ")";
    }

    /// <summary>
    /// The number of positions in this data structure.
    /// </summary>
    int ITuple.Length => 8;

    /// <summary>
    /// Get the element at position <param name="index"/>.
    /// </summary>
    object ITuple.this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return Item1;
                case 1:
                    return Item2;
                case 2:
                    return Item3;
                case 3:
                    return Item4;
                case 4:
                    return Item5;
                case 5:
                    return Item6;
                case 6:
                    return Item7;
                case 7:
                    return Item8;
                case 8:
                    return Item9;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}
