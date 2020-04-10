using System;

namespace uMIDI.Common
{
    public class Note
    {
        public byte Channel { get; set; }
        public byte Pitch { get; set; }
        public byte Velocity { get; set; }

        public long Time { get; set; }

        public Note(byte channel, byte pitch, byte velocity, long time)
        {
            Channel = channel;
            Pitch = pitch;
            Velocity = velocity;
            Time = time;
        }

        private int HalfSteps(int intervalAboveBase)
        {
            int x = (intervalAboveBase - 1) / 7;
            return 2 * (intervalAboveBase - 1) - 2 * x + ((intervalAboveBase - 1) < 7 * x + 3 ? 0 : 1);
        }

        public Note Transpose(byte interval)
        {
            int newPitch = Pitch + HalfSteps(interval);
            if (newPitch < 0 || newPitch > 127)
            {
                throw new ArgumentException("Resulting pitch is too high/low!");
            }
            return new Note(Channel, (byte) newPitch, Velocity, Time);
        }

        public string Name()
        {
            string noteName;
            switch (Pitch % 12)
            {
                case 0:
                    noteName = "C";
                    break;
                case 1:
                    noteName = "C#";
                    break;
                case 2:
                    noteName = "D";
                    break;
                case 3:
                    noteName = "D#";
                    break;
                case 4:
                    noteName = "E";
                    break;
                case 5:
                    noteName = "F";
                    break;
                case 6:
                    noteName = "F#";
                    break;
                case 7:
                    noteName = "G";
                    break;
                case 8:
                    noteName = "G#";
                    break;
                case 9:
                    noteName = "G#";
                    break;
                case 10:
                    noteName = "A";
                    break;
                case 11:
                    noteName = "A#";
                    break;
                default:
                    noteName = "B";
                    break;
            }

            int octave = (int)(Pitch / 12) - 1;

            return String.Format("{0}{1}", noteName, octave);
        }

        public string Name(KeySignature key)
        {
            // TODO (maybe; maybe we don't need it)
            throw new NotImplementedException();
        }
    }

    public class KeySignature
    {
        public Tonic Tonic { get; set; }
        public Scale Scale { get; set; }

        public KeySignature(Tonic tonic, Scale scale)
        {
            Tonic = tonic;
            Scale = scale;
        }
    }

    public enum Tonic
    {
        C = 0,
        Db = 1,
        D = 2,
        Eb = 3,
        E = 4,
        F = 5,
        Gb = 6,
        G = 7,
        Ab = 8,
        A = 9,
        Bb = 10,
        B = 11
    }

    public enum Scale
    {
        MAJOR, MINOR
    }

    public enum Interval
    {
        UNISON = 0,
        MINOR_SECOND = 1,
        HALF_STEP = 1,
        MAJOR_SECOND = 2,
        WHOLE_STEP = 2,
        MINOR_THIRD = 3,
        MAJOR_THIRD = 4,
        FOURTH = 5,
        TRITONE = 6,
        FIFTH = 7,
        MINOR_SIXTH = 8,
        MAJOR_SIXTH = 9,
        MINOR_SEVENTH = 10,
        MAJOR_SEVENTH = 11,
        OCTAVE = 12
    }
}
