﻿using System.Collections.Generic;
using Gltf.Core;
using glTFLoader.Schema;
using Wkx;

namespace B3dm.Tile
{
    public static class Gltf2Loader
    {
        public static GltfAll GetGltf(PolyhedralSurface polyhedralsurface, float[] translation)
        {
            var gltfArray = Gltf2Loader.GetGltfArray(polyhedralsurface);
            var body = gltfArray.AsBinary();
            var gltf = Gltf2Loader.GetGltf(gltfArray, translation);
            var all = new GltfAll() { Gltf = gltf, Body = body };
            return all;
        }

        public static GltfArray GetGltfArray(PolyhedralSurface polyhedralsurface)
        {
            var triangles = Triangulator.Triangulate(polyhedralsurface);
            var bytesVertices = triangles.PositionsToBinary();
            var bytesNormals = triangles.NormalsToBinary();

            var bb = polyhedralsurface.GetBoundingBox3D();

            var gltfArray = new GltfArray(bytesVertices) {
                Vertices = bytesVertices,
                Normals = bytesNormals,
                BBox = bb
            };
            return gltfArray;
        }

        public static glTFLoader.Schema.Gltf GetGltf(GltfArray gltfArray, float[] translation)
        {
            var gltf = new glTFLoader.Schema.Gltf();
            gltf.Asset = GetAsset();
            gltf.Scene = 0;
            gltf.Materials = GetMaterials();
            gltf.Nodes = GetNodes(translation);
            gltf.Buffers = GetBuffers(gltfArray.Vertices.Length);
            gltf.Meshes = GetMeshes();
            gltf.BufferViews = GetBufferViews(gltfArray.Vertices.Length);
            gltf.Accessors = GetAccessors(gltfArray.BBox, gltfArray.Count);
            gltf.Scenes = GetScenes(gltf.Nodes.Length);
            return gltf;
        }

        private static glTFLoader.Schema.Scene[] GetScenes(int nodes)
        {
            var scene = new glTFLoader.Schema.Scene();
            scene.Nodes = new int[nodes];
            return new glTFLoader.Schema.Scene[] { scene };
        }

        private static glTFLoader.Schema.Accessor[] GetAccessors(BoundingBox3D bb, int n)
        {
            // q: max and min are reversed in next py code?
            var max = new float[3] { (float)bb.YMin, (float)bb.ZMin, (float)bb.XMin };
            var min = new float[3] { (float)bb.YMax, (float)bb.ZMax, (float)bb.XMax };
            var accessor = GetAccessor(0, n, min, max, glTFLoader.Schema.Accessor.TypeEnum.VEC3);
            max = new float[3] { 1, 1, 1 };
            min = new float[3] { -1, -1, -1 };
            var accessorNormals = GetAccessor(1, n, min, max, glTFLoader.Schema.Accessor.TypeEnum.VEC3);
            var batchLength = 1;
            max = new float[1] { batchLength };
            min = new float[1] { 0 };
            var accessorBatched = GetAccessor(2, n, min, max, glTFLoader.Schema.Accessor.TypeEnum.SCALAR);
            return new glTFLoader.Schema.Accessor[] { accessor, accessorNormals, accessorBatched };
        }

        private static glTFLoader.Schema.Accessor GetAccessor(int bufferView, int n, float[] min, float[] max, glTFLoader.Schema.Accessor.TypeEnum type)
        {
            var accessor = new glTFLoader.Schema.Accessor();
            accessor.BufferView = bufferView;
            accessor.ByteOffset = 0;
            accessor.ComponentType = glTFLoader.Schema.Accessor.ComponentTypeEnum.FLOAT;
            accessor.Count = n;
            accessor.Min = min;
            accessor.Max = max;
            accessor.Type = type;
            return accessor;
        }

        private static BufferView[] GetBufferViews(int verticesLength)
        {
            // q: whats the logic here?
            var bv1 = GetBufferView(verticesLength, 0);
            var bv2 = GetBufferView(verticesLength, verticesLength);
            var bv3 = GetBufferView(verticesLength / 3, 2 * verticesLength);
            return new BufferView[] { bv1, bv2, bv3 };
        }

        private static BufferView GetBufferView(int byteLength, int byteOffset)
        {
            var bufferView1 = new BufferView();
            bufferView1.Buffer = 0;
            bufferView1.ByteLength = byteLength;
            bufferView1.ByteOffset = byteOffset;
            bufferView1.Target = BufferView.TargetEnum.ARRAY_BUFFER;
            return bufferView1;
        }

        private static glTFLoader.Schema.Mesh[] GetMeshes()
        {
            var mesh = new glTFLoader.Schema.Mesh();

            var attributes = new Dictionary<string, int>();
            attributes.Add("POSITION", 0);
            attributes.Add("NORMAL", 1);
            attributes.Add("_BATCHID", 2);

            var primitive = new MeshPrimitive();
            primitive.Attributes = attributes;
            primitive.Material = 0;
            primitive.Mode = MeshPrimitive.ModeEnum.TRIANGLES;
            mesh.Primitives = new MeshPrimitive[] { primitive };
            return new glTFLoader.Schema.Mesh[] { mesh };
        }

        private static glTFLoader.Schema.Buffer[] GetBuffers(int verticesLength)
        {
            var byteLength = verticesLength * 2;
            byteLength += verticesLength / 3;

            var buffer = new glTFLoader.Schema.Buffer() {
                ByteLength = byteLength
            };
            return new glTFLoader.Schema.Buffer[] { buffer };
        }

        private static glTFLoader.Schema.Node[] GetNodes(float[] translation)
        {
            var node = new glTFLoader.Schema.Node() {
                Translation = translation,
                Mesh = 0
            };
            return new glTFLoader.Schema.Node[] { node };
        }

        private static glTFLoader.Schema.Material[] GetMaterials()
        {
            var material = new glTFLoader.Schema.Material() {
                Name = "Material",
                PbrMetallicRoughness = new MaterialPbrMetallicRoughness() { MetallicFactor = 0 }
            };
            return new glTFLoader.Schema.Material[] { material };
        }

        private static glTFLoader.Schema.Asset GetAsset()
        {
            var asset = new glTFLoader.Schema.Asset();
            asset.Generator = "Glt.Core";
            asset.Version = "2.0";
            return asset;
        }

    }
}
