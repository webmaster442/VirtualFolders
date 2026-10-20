using System.Numerics;

namespace VirtualFolders.Core;

public readonly record struct FileSize :
    IEquatable<FileSize>,
    IComparable,
    IComparable<FileSize>,
    IComparisonOperators<FileSize, FileSize, bool>,
    IEqualityOperators<FileSize, FileSize, bool>,
    IAdditionOperators<FileSize, FileSize, FileSize>,
    ISubtractionOperators<FileSize, FileSize, FileSize>,
    IMultiplyOperators<FileSize, long, FileSize>,
    IDivisionOperators<FileSize, long, FileSize>
{
    private readonly long _bytes;

    public long TotalBytes => _bytes;

    public FileSize(long bytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(bytes, 0, nameof(bytes));
        _bytes = bytes;
    }

    public const long KiloByte = 1024;
    public const long MegaByte = KiloByte * 1024;
    public const long GigaByte = MegaByte * 1024;
    public const long TeraByte = GigaByte * 1024;

    public override string ToString()
    {
        if (_bytes < KiloByte)
            return $"{_bytes} B";
        else if (_bytes < MegaByte)
            return $"{_bytes / 1024.0:F2} KB";
        else if (_bytes < GigaByte)
            return $"{_bytes / (1024.0 * 1024.0):F2} MB";
        else if (_bytes < TeraByte)
            return $"{_bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        else
            return $"{_bytes / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
    }

    public int CompareTo(FileSize other)
        => _bytes.CompareTo(other._bytes);

    public int CompareTo(object? obj)
    {
        if (obj is FileSize other)
            return CompareTo(other);

        return 1;
    }

    public static bool operator >(FileSize left, FileSize right)
        => left._bytes > right._bytes;

    public static bool operator >=(FileSize left, FileSize right)
        => left._bytes >= right._bytes;

    public static bool operator <(FileSize left, FileSize right)
        => left._bytes < right._bytes;

    public static bool operator <=(FileSize left, FileSize right)
        => left._bytes <= right._bytes;

    public static FileSize operator +(FileSize left, FileSize right)
        => new(checked(left._bytes + right._bytes));

    public static FileSize operator -(FileSize left, FileSize right)
        => new(checked(left._bytes - right._bytes));

    public static FileSize operator *(FileSize left, long right)
        => new(checked(left._bytes * right));

    public static FileSize operator /(FileSize left, long right)
        => new(checked(left._bytes / right));

    public static implicit operator FileSize(long bytes)
        => new(bytes);
}