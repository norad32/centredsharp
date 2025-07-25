﻿using System.Buffers;
using System.Runtime.InteropServices;

namespace CentrED.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public record struct PointU16(ushort X, ushort Y)
{
    public const int SIZE = 4;

    public void Write(BinaryWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
    }
};

public static class SpanReaderBlockCoords
{
    public static PointU16 ReadPointU16(this ref SpanReader reader)
    {
        return new PointU16(reader.ReadUInt16(), reader.ReadUInt16());
    }
}