using System;
using System.Collections.Generic;

namespace uMIDI.Common
{
    public enum MetaType
    {
        TEMPO = 0x51,
        TIME_SIGNATURE = 0x58,
        KEY_SIGNATURE = 0x59
    }

    public struct MetaMessage
    {
        public byte MetaType;
        public byte[] Data;
    }

    public abstract class IMetaMessage : IMessage
    {
        public abstract MetaMessage MMessage { get; }
        public abstract long TimeDelta { get; }

        public MidiMessage Message
        {
            get
            {
                // Length of data array, plus one byte for the type code, and
                // one byte for the length
                byte[] data = new byte[MMessage.Data.Length + 2];
                data[0] = MMessage.MetaType;
                data[1] = (byte)MMessage.Data.Length;
                MMessage.Data.CopyTo(data, 2);
                return new MidiMessage
                {
                    Status = 0xff,
                    Data = data,
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    public class TempoMetaMessage : IMetaMessage
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

        public override MetaMessage MMessage
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
                    MetaType = 0x51,
                    Data = new byte[] { data1, data2, data3 }
                };
            }
        }
    }

    public class TimeSignatureMetaMessage : IMetaMessage
    {

        public override long TimeDelta { get; }

        public byte TicksPerBeat { get; set; }
        public byte BeatsPerMeasure { get; set; }
        // Subdivision power of 2 (1 - half note (2), 2 - quarter note (4), etc)
        public byte Subdivision
        {
            get => Subdivision;
            set
            {
                if (!(new List<int>() { 2, 4, 8, 16, 32 }).Contains(value))
                    throw new ArgumentException(
                        "Illegal time signature denominator");
                Subdivision = (byte)(Math.Log(value) / Math.Log(2)); // log2(value) via change base rule
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

        public override MetaMessage MMessage
        {
            get
            {
                return new MetaMessage
                {
                    MetaType = (byte)MetaType.TIME_SIGNATURE,
                    Data = new byte[] { BeatsPerMeasure, Subdivision,
                        TicksPerBeat, ThirtySecondNotesPerBeat }
                };
            }
        }
    }

    public class KeySignatureMetaMessage : IMetaMessage
    {
        public override long TimeDelta { get; }

        private sbyte sharpsFlats;
        private byte majorMinor;
        public KeySignature KeySignature
        {
            get => KeySignature;
            set
            {
                if (value.Scale == Scale.MAJOR)
                {
                    majorMinor = 0;
                    switch (value.Tonic)
                    {
                        // Circle of fifths
                        case Tonic.Db:
                            sharpsFlats = -5;
                            break;
                        case Tonic.Ab:
                            sharpsFlats = -4;
                            break;
                        case Tonic.Eb:
                            sharpsFlats = -3;
                            break;
                        case Tonic.Bb:
                            sharpsFlats = -2;
                            break;
                        case Tonic.F:
                            sharpsFlats = -1;
                            break;
                        case Tonic.C:
                            sharpsFlats = 0;
                            break;
                        case Tonic.G:
                            sharpsFlats = 1;
                            break;
                        case Tonic.D:
                            sharpsFlats = 2;
                            break;
                        case Tonic.A:
                            sharpsFlats = 3;
                            break;
                        case Tonic.E:
                            sharpsFlats = 4;
                            break;
                        case Tonic.B:
                            sharpsFlats = 5;
                            break;
                    }
                }
                else
                {
                    majorMinor = 1;
                    switch (value.Tonic)
                    {
                        // Circle of fifths
                        case Tonic.Bb:
                            sharpsFlats = -5;
                            break;
                        case Tonic.F:
                            sharpsFlats = -4;
                            break;
                        case Tonic.C:
                            sharpsFlats = -3;
                            break;
                        case Tonic.G:
                            sharpsFlats = -2;
                            break;
                        case Tonic.D:
                            sharpsFlats = -1;
                            break;
                        case Tonic.A:
                            sharpsFlats = 0;
                            break;
                        case Tonic.E:
                            sharpsFlats = 1;
                            break;
                        case Tonic.B:
                            sharpsFlats = 2;
                            break;
                        case Tonic.Gb:
                            sharpsFlats = 3;
                            break;
                        case Tonic.Db:
                            sharpsFlats = 4;
                            break;
                        case Tonic.Ab:
                            sharpsFlats = 5;
                            break;
                    }
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

        public override MetaMessage MMessage
        {
            get
            {
                return new MetaMessage
                {
                    MetaType = (byte)MetaType.KEY_SIGNATURE,
                    Data = new byte[] { (byte)sharpsFlats, majorMinor }
                };
            }
        }
    }
}
