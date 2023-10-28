﻿// SPDX-FileCopyrightText: 2021 Frans van Dorsselaer
//
// SPDX-License-Identifier: GPL-3.0-only

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Usbipd.Automation;

public readonly record struct BusId
    : IComparable<BusId>
{
    [JsonConstructor]
    public BusId(ushort bus, ushort port) => (Bus, Port) = (bus, port);

    public ushort Bus { get; init; }
    public ushort Port { get; init; }

    public override readonly string ToString() => $"{Bus}-{Port}";

    public static bool TryParse(string input, out BusId busId)
    {
        // Must be 'x-y', where x and y are positive integers without leading zeros.
        var match = Regex.Match(input, "^([1-9][0-9]*)-([1-9][0-9]*)$");
        if (match.Success
            && ushort.TryParse(match.Groups[1].Value, out var bus) && bus != 0
            && ushort.TryParse(match.Groups[2].Value, out var port) && port != 0)
        {
            busId = new()
            {
                Bus = bus,
                Port = port,
            };
            return true;
        }
        else
        {
            busId = default;
            return false;
        }
    }

    public static BusId Parse(string input)
    {
        if (!TryParse(input, out var busId))
        {
            throw new FormatException();
        }
        return busId;
    }

    #region IComparable<BusId>

    public readonly int CompareTo(BusId other) => ((uint)Bus << 16 | Port).CompareTo((uint)other.Bus << 16 | other.Port);

    public static bool operator <(BusId left, BusId right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(BusId left, BusId right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(BusId left, BusId right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(BusId left, BusId right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion
}
