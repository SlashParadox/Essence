using UnityEngine;

namespace SlashParadox.Essence
{
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public Vector2 Range { get; }

        public MinMaxRangeAttribute(float min, float max)
        {
            if (min > max)
                throw new MinMaxException<float>(min, max, true);

            Range = new Vector2(min, max);
        }
    }
}
