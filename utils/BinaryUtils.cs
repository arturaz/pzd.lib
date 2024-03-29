using System;
using System.IO;
using JetBrains.Annotations;

namespace pzd.lib.utils {
  [PublicAPI] public static class BinaryUtils {
    public static bool ensureSpace(ref byte[] buffer, int neededSize, int factor = 2) {
      var needsIncrease = neededSize > buffer.Length;
      if (needsIncrease) {
        var newSize = Math.Max(neededSize, buffer.Length * factor);
        Array.Resize(ref buffer, newSize);
      }
      return needsIncrease;
    }
    
    public static unsafe void WriteIntBigEndian(this Stream stream, int i) {
      var block = stackalloc byte[4];
      var asIntPtr = (int*) block;
      *asIntPtr = i;
      if (BitConverter.IsLittleEndian) {
        stream.WriteByte(block[3]);
        stream.WriteByte(block[2]);
        stream.WriteByte(block[1]);
        stream.WriteByte(block[0]);
      }
      else {
        stream.WriteByte(block[0]);
        stream.WriteByte(block[1]);
        stream.WriteByte(block[2]);
        stream.WriteByte(block[3]);
      }
    }
    
    #region int
    
    public static unsafe void WriteIntBigEndian(this byte[] writeBuffer, int i, int offset) {
      var block = stackalloc byte[4];
      var asIntPtr = (int*) block;
      *asIntPtr = i;
      if (BitConverter.IsLittleEndian) {
        writeBuffer[offset]     = block[3];
        writeBuffer[offset + 1] = block[2];
        writeBuffer[offset + 2] = block[1];
        writeBuffer[offset + 3] = block[0];
      }
      else {
        writeBuffer[offset]     = block[0];
        writeBuffer[offset + 1] = block[1];
        writeBuffer[offset + 2] = block[2];
        writeBuffer[offset + 3] = block[3];
      }
    }
    
    public static int ReadIntBigEndian(this byte[] data, int offset) => 
      data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3];
    
    #endregion
    
    #region long

    const int SIZE_LONG = 8;
    
    public static unsafe void WriteLongBigEndian(this byte[] writeBuffer, long i, int offset) {
      var block = stackalloc byte[SIZE_LONG];
      var asIntPtr = (long*) block;
      *asIntPtr = i;
      if (BitConverter.IsLittleEndian) {
        for (var idx = 0; idx < SIZE_LONG; idx++) {
          writeBuffer[offset + idx] = block[SIZE_LONG - 1 - idx];
        }
      }
      else {
        for (var idx = 0; idx < SIZE_LONG; idx++) {
          writeBuffer[offset + idx] = block[idx];
        }
      }
    }

    public static long ReadLongBigEndian(this byte[] data, int offset) {
      var l = 0L;
      for (var idx = 0; idx < SIZE_LONG; idx++) {
#pragma warning disable 675
        l |= data[offset + idx] << 8 * (SIZE_LONG - 1 - idx);
#pragma warning restore 675
      }
      return l;
    }
    
    #endregion
    
    #region ushort
    
    public static unsafe void WriteUShortBigEndian(this byte[] writeBuffer, ushort s, int offset) {
      var block = stackalloc byte[2];
      var asUShortPtr = (ushort*) block;
      *asUShortPtr = s;
      if (BitConverter.IsLittleEndian) {
        writeBuffer[offset]     = block[1];
        writeBuffer[offset + 1] = block[0];
      }
      else {
        writeBuffer[offset]     = block[0];
        writeBuffer[offset + 1] = block[1];
      }
    }
    
    public static ushort ReadUShortBigEndian(this byte[] data, int offset) => 
      (ushort) (data[offset] << 8 | data[offset + 1]);
    
    #endregion
  }
}