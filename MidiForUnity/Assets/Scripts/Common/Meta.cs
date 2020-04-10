using System;
using System.Collections.Generic;

namespace uMIDI.Common
{
    public struct MetaMessage
    {
        public byte MetaType;
        public byte DataLength;
        public byte[] Data;
    }

    public abstract class IMetaMessage : IMessage
    {
        public MetaMessage MetaMessage { get; }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = 0xff,
                    MsgSize = 0, // TODO
                    Data = new byte[0], // TODO
                    Time = 0
                };
            }
        }
    }

    public class TempoMetaMessage : IMetaMessage
    {
        // Tempo in *quarter notes* per minute
        public double Tempo { get; set; }

        public long Time { get; set; }

        public TempoMetaMessage(double tempo, long time)
        {
            // Minimum tempo is 0xffffff microseconds per beat (~3.6 BPM)
            if (tempo < 1 / ((double)(0xffffff) / 1e6 / 60))
                throw new ArgumentException("Tempo is too slow");
            Tempo = tempo;
            Time = time;
        }

        public MetaMessage MetaMessage
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
                    DataLength = 3,
                    Data = new byte[] { data1, data2, data3 }
                };
            }
        }
    }

    public class TimeSignatureMetaMessage : IMetaMessage
    {

        public long Time { get; set; }


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
                Subdivision = (byte)Math.Log(value);
            }
        }

        public TimeSignatureMetaMessage(int numerator, int denominator, long time)
        {
            BeatsPerMeasure = (byte)numerator;
            Subdivision = (byte)denominator;
            Time = time;
        }

        public MetaMessage MetaMessage
        {
            get
            {
                byte metronomeClick = 0; //TODO
                return new MetaMessage
                {
                    MetaType = 0x58,
                    DataLength = 4,
                    Data = new byte[] { BeatsPerMeasure, Subdivision,
                        metronomeClick, 8 }
                };
            }
        }
    }

    public class KeySignatureMetaMessage : IMetaMessage
    {
        public long Time { get; set; }

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

        public KeySignatureMetaMessage(sbyte sharpsFlats, byte majorMinor, long time)
        {
            sharpsFlats = sharpsFlats;
            majorMinor = majorMinor;
            Time = time;
        }

        public MetaMessage MetaMessage
        {
            get
            {
                return new MetaMessage
                {
                    MetaType = 0x59,
                    DataLength = 2,
                    Data = new byte[] { (byte)sharpsFlats, majorMinor }
                };
            }
        }
    }
}
