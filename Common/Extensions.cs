using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace AdventOfCode.Common;
internal static class Extensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string value) {
        return (int)value.ToLong();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt(this string value) {
        return (uint)value.ToLong();
    }
    public static long ToLong(this string value) {
        int length = value.Length;
        if (length <= 0) { return 0; }

        int i = 0;
        while (i < length && !char.IsDigit(value[i])) {
            i++;
        }

        bool isNegative = i > 0 ? value[i - 1] == '-' : false;
        long result = 0;
        while (i < length) {
            char c = value[i];
            if (c >= '0' && c <= '9') {
                result = result * 10 + c - '0';
            }
            i++;
        }
        return isNegative ? -result : result;
    }
    public static void Slice(this string input, char splitOn, Action<string> action) {
        foreach (string value in input.Slice(splitOn)) {
            action(value);
        }
    }
    public static IEnumerable<string> Slice(this string input, char splitOn) {
        int start = 0;
        int end = input.IndexOf(splitOn);
        while (end > 0) {
            yield return input[start..end];

            start = end + 1;
            end = input.IndexOf(splitOn, start);
        }
        if (start < input.Length) {
            yield return input[start..];
        }
    }
    public static void Slice(this string input, string splitOn, Action<string> action) {
        foreach (string value in input.Slice(splitOn)) {
            action(value);
        }
    }
    public static IEnumerable<string> Slice(this string input, string splitOn) {
        int start = 0;
        int end = input.IndexOf(splitOn);
        while (end > 0) {
            yield return input[start..end];

            start = end + splitOn.Length;
            end = input.IndexOf(splitOn, start);
        }
        if (start < input.Length) {
            yield return input[start..];
        }
    }
    public static void Sections(this string input, Action<string> action) {
        foreach (string value in input.Sections()) {
            action(value);
        }
    }
    public static IEnumerable<string> Sections(this string input) {
        char last = '\0';
        int start = 0;
        for (int end = 0; end < input.Length; end++) {
            char c = input[end];
            if (c == '\n' && last == '\n') {
                yield return input[start..(end - 1)];
                c = '\0';
                start = end + 1;
            }
            last = c;
        }
        if (start < input.Length) {
            yield return input[start..];
        }
    }
    public static (T, T) GetMinMax<T>(this IEnumerable<T> values) where T : IComparable<T> {
        bool firstElement = true;
        T min = default; T max = default;
        foreach (T value in values) {
            if (firstElement) {
                min = value; max = value; firstElement = false;
            } else {
                if (value.CompareTo(min) < 0) { min = value; }
                if (value.CompareTo(max) > 0) { max = value; }
            }
        }
        return (min, max);
    }
    public static string[] SplitOn(this string input, params string[] splits) {
        string[] results = new string[splits.Length + 1];
        int index = 0;
        for (int i = 0; i < splits.Length; i++) {
            string split = splits[i];
            int newIndex = input.IndexOf(split, index);
            if (newIndex >= 0) {
                results[i] = input.Substring(index, newIndex - index);
                index = newIndex + split.Length;
            } else {
                results[i] = string.Empty;
            }
        }
        if (index < input.Length) {
            results[splits.Length] = input.Substring(index);
        } else {
            results[splits.Length] = string.Empty;
        }
        return results;
    }
    public static IEnumerable<T[]> Permute<T>(this T[] items) {
        return DoPermute(items, 0, items.Length - 1);
    }
    private static IEnumerable<T[]> DoPermute<T>(T[] items, int start, int end) {
        if (start == end) {
            yield return items;
        } else {
            for (var i = start; i <= end; i++) {
                Swap(ref items[start], ref items[i]);
                foreach (T[] seq in DoPermute(items, start + 1, end)) {
                    yield return seq;
                }
                Swap(ref items[start], ref items[i]);
            }
        }
    }
    private static void Swap<T>(ref T a, ref T b) {
        var temp = a;
        a = b;
        b = temp;
    }
    public static IEnumerable<List<T>> Permute<T>(this List<T> items) {
        return DoPermute(items, 0, items.Count - 1);
    }
    private static IEnumerable<List<T>> DoPermute<T>(List<T> items, int start, int end) {
        if (start == end) {
            yield return items;
        } else {
            for (var i = start; i <= end; i++) {
                Swap(items, start, i);
                foreach (List<T> seq in DoPermute(items, start + 1, end)) {
                    yield return seq;
                }
                Swap(items, start, i);
            }
        }
    }
    private static void Swap<T>(List<T> items, int a, int b) {
        var temp = items[a];
        items[a] = items[b];
        items[b] = temp;
    }
    public static int GCD(int a, int b) {
        if (a == 0 && b == 0) { return 1; }
        a = a < 0 ? -a : a;
        b = b < 0 ? -b : b;
        if (a == 0) { return b; }
        if (b == 0) { return a; }

        int r;
        if (b > a) {
            r = b % a;
            if (r == 0) { return a; }
            if (r == 1) { return 1; }
            b = r;
        }
        while (true) {
            r = a % b;
            if (r == 0) { return b; }
            if (r == 1) { return 1; }
            a = b;
            b = r;
        }
    }
    public static long GCD(long a, long b) {
        if (a == 0 && b == 0) { return 1; }
        a = a < 0 ? -a : a;
        b = b < 0 ? -b : b;
        if (a == 0) { return b; }
        if (b == 0) { return a; }

        long r;
        if (b > a) {
            r = b % a;
            if (r == 0) { return a; }
            if (r == 1) { return 1; }
            b = r;
        }
        while (true) {
            r = a % b;
            if (r == 0) { return b; }
            if (r == 1) { return 1; }
            a = b;
            b = r;
        }
    }
}