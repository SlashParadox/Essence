// Copyright (c) SlashParadox

namespace SlashParadox.Essence
{
    /// <summary>
    /// A class of common, reusable literal values.
    /// </summary>
    public static class Literals
    {
        /// <summary>A value indicating an invalid index value.</summary>
        public static readonly int InvalidIndex = -1;

        /// <summary>The number of degrees in a quarter circle.</summary>
        public static readonly double QuarterCircleDegrees = 90.0;
        
        /// <summary>The number of degrees in a semi circle.</summary>
        public static readonly double SemiCircleDegrees = 180.0;
        
        /// <summary>The number of degrees in a quarter-short (3/4) circle.</summary>
        public static readonly double QuarterShortCircleDegrees = 270.0;
        
        /// <summary>The number of degrees in a full circle.</summary>
        public static readonly double FullCircleDegrees = 360.0;
        
        /// <summary>The maximum amount of bits usable to create a secure double.</summary>
        public static readonly int MaxSecureDoubleBits = 53;
    }
}
