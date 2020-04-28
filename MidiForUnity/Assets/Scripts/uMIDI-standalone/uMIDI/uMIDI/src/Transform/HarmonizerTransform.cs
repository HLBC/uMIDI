using System;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public class HarmonizerTransform
    {
        private MetaMidiStream _harmony;

        public HarmonizerTransform(MetaMidiStream harmony)
        {
            _harmony = harmony;
        }

        public void Apply(Region region)
        {
            // Just transposes, all we had time to implement
            KeySignature harmonyKey = _harmony.MetaState.KeySignature;
            KeySignature melodyKey = region.State.KeySignature;
            int harmonyMajorTonic = harmonyKey.Scale == Scale.MAJOR
                ? (int)harmonyKey.Tonic : (int)harmonyKey.Tonic + 3;
            int melodyMajorTonic = harmonyKey.Scale == Scale.MAJOR
                ? (int)melodyKey.Tonic : (int)melodyKey.Tonic + 3;

            int interval = (harmonyMajorTonic - melodyMajorTonic) % 12;

            region.State.KeySignature = harmonyKey;

            foreach (TimedNote note in region)
            {
                note.Pitch += (byte)interval;
            }
        }
    }
}
