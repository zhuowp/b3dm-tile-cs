﻿using System;
using System.IO;
using B3dm.Tile;
using glTFLoader;
using Wkb.Triangulate;
using Wkb2Gltf;
using Wkx;

namespace sample_wkb_2_b3dm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sample conversion from wkb to b3dm.");
            var inputfile = @"testfixtures/building.wkb";
            var f = File.OpenRead(inputfile);
            var g = Geometry.Deserialize<WkbSerializer>(f);
            var translation = new double[] { 539085.1, 6989220.68, 52.98 };

            var surface = (PolyhedralSurface)g;
            var triangles = Triangulator.Triangulate(surface);
            var bb = surface.GetBoundingBox3D();
            var gltfArray = Gltf2Loader.GetGltfArray(triangles, bb);
            var gltfall = Gltf2Loader.ToGltf(gltfArray, translation);
            var ms = new MemoryStream();
            gltfall.Gltf.SaveBinaryModel(gltfall.Body, ms);
            var glb = ms.ToArray();
            var b3dm = GlbToB3dmConvertor.Convert(glb);
            B3dmWriter.WriteB3dm("building.b3dm", b3dm);
            Console.WriteLine($"File building.b3dm is written...");
            Console.WriteLine($"Press any key to continue...");
            Console.ReadKey();
        }
    }
}