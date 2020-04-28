using System.Collections.Generic;

namespace uMIDI.IO
{
    public class MidiStreamState
    {
        public ISet<byte> NotesOn { get; }
        public MidiClock Clock { get; }

        public MidiStreamState(int ticksPerBeat)
        {
            NotesOn = new HashSet<byte>();
            Clock = new MidiClock(ticksPerBeat);
        }
    }
}
