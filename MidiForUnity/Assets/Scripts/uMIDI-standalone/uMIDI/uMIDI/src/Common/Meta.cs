using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace uMIDI.Common
{
    /// <summary>
    /// Enum for meta type codes for midi meta events.
    /// </summary>
    public enum MetaType
    {
        TEMPO = 0x51,
        TIME_SIGNATURE = 0x58,
        KEY_SIGNATURE = 0x59
    }

    /// <summary>
    /// Struct containing byte data for a MIDI meta message.
    /// </summary>
    public struct MetaMessage : IEquatable<MetaMessage>
    {
        public byte MetaType;
        public byte[] Data;
        public long TimeDelta;

        public bool Equals([AllowNull] MetaMessage other)
        {
            return MetaType == other.MetaType && Data.SequenceEqual(other.Data)
                && TimeDelta == other.TimeDelta;
        }

        public override string ToString()
        {
            return String.Format("MetaMessage [MetaType: {0}, Data: [{1}], " +
                "TimeDelta: {2}]", MetaType, String.Join(", ", Data),
                TimeDelta);
        }
    }

    /// <summary>
    /// Interface for MIDI meta messages
    /// </summary>
    public interface IMetaMessage : IMessage
    {
        /// <summary>
        /// Implement this method to return the byte data
        /// </summary>
        MetaMessage MetaMessage { get; }
    }

    /// <summary>
    /// Implementation of IMetaMessage. MetaMessage classes should extend this
    /// class.
    /// </summary>
    public abstract class AbstractMetaMessage : AbstractMessage, IMetaMessage
    {
        public abstract MetaMessage MetaMessage { get; }

        /// <summary>
        /// Converts to a normal MidiMessage struct object.
        /// </summary>
        public override MidiMessage Message
        {
            get
            {
                // Length of data array, plus one byte for the type code, and
                // one byte for the length
                byte[] data = new byte[MetaMessage.Data.Length + 2];
                data[0] = MetaMessage.MetaType;
                data[1] = (byte)MetaMessage.Data.Length;
                MetaMessage.Data.CopyTo(data, 2);
                return new MidiMessage
                {
                    Status = 0xff,
                    Data = data,
                    TimeDelta = MetaMessage.TimeDelta
                };
            }
        }
    }

    /// <summary>
    /// Tempo change meta message.
    /// </summary>
    public class TempoMetaMessage : AbstractMetaMessage
    {
        // Tempo in *quarter notes* per minute
        public double Tempo { get; set; }

        public override long TimeDelta { get; }

        public TempoMetaMessage(double tempo, long timeDelta)
        {
            // Minimum tempo is 0xffffff microseconds per beat (~3.6 BPM)
            if (tempo < 1 / ((double)(0xffffff) / 1e6 / 60))
                throw new ArgumentException("Tempo is too slow");
            Tempo = tempo;
            TimeDelta = timeDelta;
        }

        public override MetaMessage MetaMessage
        {
            get
            {
                // TODO maybe write function that breaks big numbers into bytes
                int microsecondsPerBeat = (int)(60 * 1e6 / Tempo);
                byte data1 = (byte)(microsecondsPerBeat / 0x10000);
                byte data2 = (byte)((microsecondsPerBeat % 0x10000) / 0x100);
                byte data3 = (byte)(microsecondsPerBeat % 0x100);
                return new MetaMessage
                {
                    MetaType = (byte)MetaType.TEMPO,
                    Data = new byte[] { data1, data2, data3 },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    /// <summary>
    /// Time signature change meta message.
    /// </summary>
    public class TimeSignatureMetaMessage : AbstractMetaMessage
    {

        public override long TimeDelta { get; }

        public byte TicksPerBeat { get; set; }
        public byte BeatsPerMeasure { get; set; }
        // Subdivision power of 2 (1 - half note (2), 2 - quarter note (4), etc)
        private byte _subdivision;
        public byte Subdivision
        {
            get => _subdivision;
            set
            {
                if (!((new List<int>() { 2, 4, 8, 16, 32 }).Contains(value)))
                    throw new ArgumentException(
                        "Illegal time signature denominator");
                // log2(value) via change base rule
                _subdivision = (byte)(Math.Log(value) / Math.Log(2));
            }
        }
        public byte ThirtySecondNotesPerBeat { get; set; }

        public TimeSignatureMetaMessage(int numerator, int denominator,
            byte ticksPerBeat, byte thirtySecondNotesPerBeat, long timeDelta)
        {
            BeatsPerMeasure = (byte)numerator;
            Subdivision = (byte)denominator;
            TicksPerBeat = ticksPerBeat;
            ThirtySecondNotesPerBeat = thirtySecondNotesPerBeat;
            TimeDelta = timeDelta;
        }

        public override MetaMessage MetaMessage
        {
            get
            {
                return new MetaMessage
                {
                    MetaType = (byte)MetaType.TIME_SIGNATURE,
                    Data = new byte[] { BeatsPerMeasure, Subdivision,
                        TicksPerBeat, ThirtySecondNotesPerBeat },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    /// <summary>
    /// Key signature change meta message.
    /// </summary>
    public class KeySignatureMetaMessage : AbstractMetaMessage
    {
        public override long TimeDelta { get; }

        private sbyte sharpsFlats;
        private byte majorMinor;
        public KeySignature KeySignature
        {
            get
            {
                Scale scale;
                Tonic tonic;
                if (majorMinor == 0)
                {
                    scale = Scale.MAJOR;
                    tonic = sharpsFlats switch
                    {
                        -5 => Tonic.Db,
                        -4 => Tonic.Ab,
                        -3 => Tonic.Eb,
                        -2 => Tonic.Bb,
                        -1 => Tonic.F,
                        0 => Tonic.C,
                        1 => Tonic.G,
                        2 => Tonic.D,
                        3 => Tonic.A,
                        4 => Tonic.E,
                        5 => Tonic.B,
                        6 => Tonic.Gb,
                        _ => throw new Exception("This will never happen"),
                    };
                }
                else
                {
                    scale = Scale.MINOR;
                    tonic = sharpsFlats switch
                    {
                        -5 => Tonic.Bb,
                        -4 => Tonic.F,
                        -3 => Tonic.C,
                        -2 => Tonic.G,
                        -1 => Tonic.D,
                        0 => Tonic.A,
                        1 => Tonic.E,
                        2 => Tonic.B,
                        3 => Tonic.Gb,
                        4 => Tonic.Db,
                        5 => Tonic.Ab,
                        6 => Tonic.Eb,
                        _ => throw new Exception("This will never happen"),
                    };
                }
                return new KeySignature(tonic, scale);
            }

            set
            {
                if (value.Scale == Scale.MAJOR)
                {
                    majorMinor = 0;
                    sharpsFlats = value.Tonic switch
                    {
                        // Circle of fifths
                        Tonic.Db => -5,
                        Tonic.Ab => -4,
                        Tonic.Eb => -3,
                        Tonic.Bb => -2,
                        Tonic.F => -1,
                        Tonic.C => 0,
                        Tonic.G => 1,
                        Tonic.D => 2,
                        Tonic.A => 3,
                        Tonic.E => 4,
                        Tonic.B => 5,
                        Tonic.Gb => 6,
                        _ => throw new ArgumentException(
                            "Invalid number of sharps/flats"),
                    };
                }
                else
                {
                    majorMinor = 1;
                    sharpsFlats = value.Tonic switch
                    {
                        // Circle of fifths
                        Tonic.Bb => -5,
                        Tonic.F => -4,
                        Tonic.C => -3,
                        Tonic.G => -2,
                        Tonic.D => -1,
                        Tonic.A => 0,
                        Tonic.E => 1,
                        Tonic.B => 2,
                        Tonic.Gb => 3,
                        Tonic.Db => 4,
                        Tonic.Ab => 5,
                        Tonic.Eb => 6,
                        _ => throw new ArgumentException(
                            "Invalid number of sharps/flats"),
                    };
                }
            }
        }

        public KeySignatureMetaMessage(sbyte sharpsFlats, byte majorMinor,
            long timeDelta)
        {
            this.sharpsFlats = sharpsFlats;
            this.majorMinor = majorMinor;
            TimeDelta = timeDelta;
        }

        public KeySignatureMetaMessage(KeySignature keySignature,
            long timeDelta)
        {
            KeySignature = keySignature;
            TimeDelta = timeDelta;
        }

        public override MetaMessage MetaMessage
        {
            get
            {
                return new MetaMessage
                {
                    MetaType = (byte)MetaType.KEY_SIGNATURE,
                    Data = new byte[] { (byte)sharpsFlats, majorMinor },
                    TimeDelta = TimeDelta
                };
            }
        }
    }
}
