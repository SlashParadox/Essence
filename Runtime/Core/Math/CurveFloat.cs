using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    [System.Serializable]
    public class CurveFloat : CurveDataBase<float>
    {
        /// <summary>The sole <see cref="AnimationCurve"/> for the data.</summary>
        [SerializeField] private AnimationCurve curve = new AnimationCurve();

        public override float Evaluate(float time)
        {
            return curve.Evaluate(time);
        }

        protected override void DeserializeFromTable(HashMap<float, float> table)
        {
            if (table == null)
                return;

            curve.ClearKeys();
            foreach (KeyValuePair<float, float> pair in table)
            {
                Keyframe keyframe = new Keyframe();
                keyframe.time = pair.Key;
                keyframe.value = pair.Value;

                curve.AddKey(keyframe);
            }

            curve.SetCurveTangentMode(TableTangentMode);
        }

        protected override void SerializeToTable(HashMap<float, float> table)
        {
            foreach (Keyframe keyframe in curve.keys)
            {
                table.TryAdd(keyframe.time, keyframe.value);
            }
        }
    }
}
