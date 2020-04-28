using System;
using uMIDI;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDITests
{
    /// <summary>
    /// Simply prints out messages given to it. Used for debugging.
    /// </summary>
    public class PrintInstrument : IMidiInstrument
    {
        public MidiStream Stream { get; }

        public PrintInstrument(MidiStream stream)
        {
            Stream = stream;
        }

        public void ProcessMidi(IMessage[] messages)
        {
            foreach (IMessage message in messages)
            {
                Console.WriteLine(message);
            }
        }
    }
}
