﻿using NUnit.Framework;

namespace Wkb2Gltf.Tests
{
    public class BoundingBox3DTests
    {
        [Test]
        public void BoudingBox3DCenterTest()
        {
            // Arrange
            var bb = new BoundingBox3D() { XMin = 0, YMin = 0, ZMin = 0, XMax = 2, YMax = 2, ZMax = 2 };

            // act
            var center = bb.GetCenter();

            // assert
            Assert.IsTrue(center.X == 1 && center.Y==1 && center.Z == 1);
        }

        [Test]
        public void BoundingBox3DTransformYToZTest()
        {
            // Arrange
            var bb = new BoundingBox3D() { XMin = -105.4645, YMin = -11.3846445, ZMin = 72.374, XMax = -58.6345, YMax = -7.228368, ZMax = 127.598 };

            // act 
            var zUpBox = bb.TransformYToZ();

            // assert
            Assert.IsTrue(zUpBox.XMin == -105.4645);
            Assert.IsTrue(zUpBox.YMin == -72.374);
            Assert.IsTrue(zUpBox.ZMin == -11.3846445);

            Assert.IsTrue(zUpBox.XMax == -58.6345);
            Assert.IsTrue(zUpBox.YMax == -72.374);
            Assert.IsTrue(zUpBox.ZMax == -7.228368);
        }
    }
}
