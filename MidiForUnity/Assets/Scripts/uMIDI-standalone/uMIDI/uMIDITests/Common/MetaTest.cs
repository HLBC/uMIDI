using Xunit;
using uMIDI.Common;
using System;

namespace uMIDITests.Common
{
    public class MetaTest
    {
        [Fact]
        public void TestTempo()
        {
            double tempo = 100;
            TempoMetaMessage message = new TempoMetaMessage(tempo, 0);

            int microsecondsPerBeat = (int)(60 * 1e6 / tempo);
            byte data1 = (byte)(microsecondsPerBeat / 0x10000);
            byte data2 = (byte)((microsecondsPerBeat % 0x10000) / 0x100);
            byte data3 = (byte)(microsecondsPerBeat % 0x100);

            MetaMessage bytes = message.MetaMessage;
            MetaMessage expected = new MetaMessage
            {
                MetaType = 0x51,
                Data = new byte[] { data1, data2, data3 },
                TimeDelta = 0
            };

            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void TestTimeSignature()
        {
            int numerator = 4;
            int denominator = 4;
            byte ticksPerBeat = 24;
            byte thirtySecondNotesPerBeat = 8;

            TimeSignatureMetaMessage message = new TimeSignatureMetaMessage(
                numerator,
                denominator,
                ticksPerBeat,
                thirtySecondNotesPerBeat,
                0
                );

            int subdivision = (int)(Math.Log(denominator) / Math.Log(2));

            MetaMessage bytes = message.MetaMessage;
            MetaMessage expected = new MetaMessage
            {
                MetaType = 0x58,
                Data = new byte[] { (byte)numerator, (byte)subdivision,
                    ticksPerBeat, thirtySecondNotesPerBeat },
                TimeDelta = 0
            };

            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void TestKeySignature()
        {
            // D major
            sbyte sharpsFlats = 2;
            byte majorMinor = 0;

            KeySignature keySignatureExpected =
                new KeySignature(Tonic.D, Scale.MAJOR);

            KeySignatureMetaMessage message1 = new KeySignatureMetaMessage(
                keySignatureExpected, 0);
            KeySignatureMetaMessage message2 = new KeySignatureMetaMessage(
                sharpsFlats, majorMinor, 0);

            Assert.True(message2.KeySignature.Equals(keySignatureExpected));

            MetaMessage bytes1 = message1.MetaMessage;
            MetaMessage bytes2 = message2.MetaMessage;
            MetaMessage expected = new MetaMessage
            {
                MetaType = 0x59,
                Data = new byte[] { (byte)sharpsFlats, majorMinor },
                TimeDelta = 0
            };

            Assert.Equal(expected, bytes1);
            Assert.Equal(expected, bytes2);
        }
    }
}
