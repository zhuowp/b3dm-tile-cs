﻿using System;
using System.Linq;

namespace Gltf.Core
{
    public class GltfArray
    {
        private byte[] vertices;
        private int n;

        public GltfArray(byte[] Vertices)
        {
            this.vertices = Vertices;
            this.n = (int)Math.Round((double)vertices.Length / 12, 0);

        }
        public byte[] Vertices { get; set; }
        public byte[] Normals { get; set; }
        public byte[] Ids { get { return BinaryConvertor.ToBinary(new float[n]); } }

        public int Count { get { return n; } }
        public byte[] Uvs { get; set; }
        public BoundingBox3D BBox { get; set; }

        public byte[] AsBinary()
        {
            return Vertices.Concat(Normals).Concat(Ids).ToArray();
        }
        // body = vertices + normals + ids + uvs
    }
}
