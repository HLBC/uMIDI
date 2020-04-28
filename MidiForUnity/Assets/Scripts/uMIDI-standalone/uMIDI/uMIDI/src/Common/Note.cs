using System;
using System.Diagnostics.CodeAnalysis;

namespace uMIDI.Common
{
    /// <summary>
    /// Represents a note with a channel, pitch, and velocity. There are 16
    /// possible channels, 128 possible pitches, and 128 possible velocity
    /// values.
    /// </summary>
    public class Note : IEquatable<Note>
    {
        private byte _channel;
        public byte Channel
        {
            get => _channel;
            set
            {
                if (value < 16 && value >= 0)
                {
                    _channel = value;
                }
                else
                {
                    throw new ArgumentException(
                        "Channel must be between 0 and 15");
                }
            }
        }

        private byte _pitch;
        public byte Pitch {
            get => _pitch;
            set
            {
                if (value < 128 && value >= 0)
                {
                    _pitch = value;
                }
                else
                {
                    throw new ArgumentException(
                        "Channel must be between 0 and 127");
                }
            }
        }

        private byte _velocity;
        public byte Velocity {
            get => _velocity;
            set
            {
                if (value < 128 && value >= 0)
                {
                    _velocity = value;
                }
                else
                {
                    throw new ArgumentException(
                        "Velocity must be between 0 and 127");
                }
            }
        }

        public Note(byte channel, byte pitch, byte velocity)
        {
            Channel = channel;
            Pitch = pitch;
            Velocity = velocity;
        }

        /// <summary>
        /// Returns a new <see cref="Note"/> object transposed by a given
        /// interval.
        /// </summary>
        /// <param name="interval">Number of semitones by which the
        /// transposition is performed. Positive values represent an increase
        /// in pitch, and vice versa.</param>
        /// <returns>A new <see cref="Note"/> object.</returns>
        public Note Transpose(byte interval)
        {
            int newPitch = Pitch + interval;
            if (newPitch < 0 || newPitch > 127)
            {
                throw new ArgumentException("Resulting pitch is too high/low!");
            }
            return new Note(Channel, (byte) newPitch, Velocity);
        }

        /// <summary>
        /// Returns the note name as a string. Only returns sharps.
        /// </summary>
        /// <returns></returns>
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

        public bool Equals([AllowNull] Note other)
        {
            return Channel == other.Channel && Pitch == other.Pitch
                && Velocity == other.Velocity;
        }

        public override string ToString()
        {
            return String.Format("(Note {2} {0} ({1}) {3})",
                Pitch, Name(), Channel, Velocity);
        }
    }

    /// <summary>
    /// Represents a key signature: a tonic center, and a scale (currently only
    /// the major scale and natural minor scale are supported.)
    /// </summary>
    public class KeySignature : IEquatable<KeySignature>
    {
        public Tonic Tonic { get; set; }
        public Scale Scale { get; set; }

        public KeySignature(Tonic tonic, Scale scale)
        {
            Tonic = tonic;
            Scale = scale;
        }

        public bool Equals([AllowNull] KeySignature other)
        {
            return Tonic == other.Tonic && Scale == other.Scale;
        }
    }

    /// <summary>
    /// Enum of the 12-tone chromatic scale. Enharmonic notes are not supported.
    /// </summary>
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

    /// <summary>
    /// Enum of scales. Currently only the major and natural minor scales are
    /// supported.
    /// </summary>
    public enum Scale
    {
        MAJOR, MINOR
    }

    /// <summary>
    /// Enum representing interval names and the associated difference in pitch
    /// in semitones.
    /// </summary>
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
