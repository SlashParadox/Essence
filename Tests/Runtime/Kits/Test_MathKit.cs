// Copyright (c) Craig Williams, SlashParadox

using NUnit.Framework;
using SlashParadox.Essence.Kits;

namespace SlashParadox.Essence.Tests
{
    public class TestMathKit
    {
        [Test]
        public void Test_InRange_SByte_ReturnsTrue([Random(0, 10, 3)] sbyte value,
                                                   [Random(-10, -1, 3)] sbyte min,
                                                   [Random(11, 20, 3)] sbyte max)
        {
            Assert.IsTrue(MathKit.InRange(value, min, max));
        }
    }
}