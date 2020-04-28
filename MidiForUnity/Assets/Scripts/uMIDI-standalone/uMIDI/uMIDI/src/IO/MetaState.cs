using uMIDI.Common;

namespace uMIDI.IO
{
    public class MetaState
    {
        public int BeatsPerMeasure { get; set; }
        // Subdivision power of 2 (1 - half note (2), 2 - quarter note (4), etc)
        public int Subdivision { get; set; }
        public int BPM { get; set; }
        public KeySignature KeySignature { get; set; }

        public MetaState() : this(4, 2, 120,
            new KeySignature(Tonic.C, Scale.MAJOR))
        {
        }

        public MetaState(int beatsPerMeasure, int subdivision, int bpm,
            KeySignature key)
        {
            BeatsPerMeasure = beatsPerMeasure;
            Subdivision = subdivision;
            BPM = bpm;
            KeySignature = key;
        }
    }
}
