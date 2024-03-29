using pzd.lib.collection;
using pzd.lib.functional;

namespace pzd.lib.serialization.rws {
  static class EitherRW {
    public const byte
      DISCRIMINATOR_LEFT = (byte) 'l',
      DISCRIMINATOR_RIGHT = (byte) 'r';

    public static readonly Rope<byte>
      DISCRIMINATOR_LEFT_ROPE = Rope.a(new[] { DISCRIMINATOR_LEFT }),
      DISCRIMINATOR_RIGHT_ROPE = Rope.a(new[] { DISCRIMINATOR_RIGHT });
  }

  class EitherRW<A, B> : ISerializedRW<Either<A, B>> {
    readonly ISerializedRW<A> aRW;
    readonly ISerializedRW<B> bRW;

    public EitherRW(ISerializedRW<A> aRw, ISerializedRW<B> bRw) {
      aRW = aRw;
      bRW = bRw;
    }

    public Either<string, DeserializeInfo<Either<A, B>>> deserialize(byte[] serialized, int startIndex) {
      if (serialized.Length == 0) return "either deserialization failed: zero length byte array!";
      if (startIndex >= serialized.Length) 
        return $"either deserialization failed: startIndex {startIndex} >= serialized length {serialized.Length}";
      var discriminator = serialized[startIndex];
      switch (discriminator) {
        case EitherRW.DISCRIMINATOR_LEFT:
          return aRW.deserialize(serialized, startIndex + 1).mapRight(info =>
            new DeserializeInfo<Either<A, B>>(Either<A, B>.Left(info.value), info.bytesRead + 1)
          );
        case EitherRW.DISCRIMINATOR_RIGHT:
          return bRW.deserialize(serialized, startIndex + 1).mapRight(info =>
            new DeserializeInfo<Either<A, B>>(Either<A,B>.Right(info.value), info.bytesRead + 1)
          );
        default:
          return $"Unknown either discriminator '{discriminator}'";
      }
    }

    public Rope<byte> serialize(Either<A, B> either) =>
      either.isLeft
        ? EitherRW.DISCRIMINATOR_LEFT_ROPE + aRW.serialize(either.__unsafeGetLeft)
        : EitherRW.DISCRIMINATOR_RIGHT_ROPE + bRW.serialize(either.__unsafeGetRight);
  }
}