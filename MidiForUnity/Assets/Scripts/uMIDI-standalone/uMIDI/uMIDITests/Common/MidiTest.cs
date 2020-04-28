using Xunit;
using uMIDI.Common;

namespace uMIDITests.Common
{
    public class MidiTest
    {
        [Fact]
        public void TestNoteOff()
        {
            byte channel = 0;
            byte pitch = 60;
            byte velocity = 50;

            NoteOffMessage message = new NoteOffMessage(
                new Note(
                    channel,
                    pitch,
                    velocity
                    ),
                0
                );

            MidiMessage bytes = message.Message;
            MidiMessage expected = new MidiMessage
            {
                Status = (byte)(0x80 + channel),
                Data = new byte[] { pitch, velocity },
                TimeDelta = 0
            };

            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void TestNoteOn()
        {
            byte channel = 0;
            byte pitch = 60;
            byte velocity = 50;

            NoteOnMessage message = new NoteOnMessage(
                new Note(
                    channel,
                    pitch,
                    velocity
                    ),
                0
                );

            MidiMessage bytes = message.Message;
            MidiMessage expected = new MidiMessage
            {
                Status = (byte)(0x90 + channel),
                Data = new byte[] { pitch, velocity },
                TimeDelta = 0
            };

            Assert.Equal(expected, bytes);
        }
    }
}
