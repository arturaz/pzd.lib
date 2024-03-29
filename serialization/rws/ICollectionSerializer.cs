using System;
using System.Collections.Generic;
using pzd.lib.collection;

namespace pzd.lib.serialization.rws {
  class ICollectionSerializer<A, C> : ISerializer<C> where C : ICollection<A> {
    readonly ISerializer<A> serializer;

    public ICollectionSerializer(ISerializer<A> serializer) { this.serializer = serializer; }

    public Rope<byte> serialize(C c) {
      var count = c.Count;
      var rope = Rope.a(BitConverter.GetBytes(count));
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var a in c) {
        var aRope = serializer.serialize(a);
        rope += aRope;
      }
      return rope;
    }
  }
}