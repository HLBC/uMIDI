using System.Collections.Generic;

namespace uMIDI.IO
{
    public class MidiStreamState
    {
        public ISet<byte> NotesOn { get; }
        public ulong CurrentTick { get; }

        public MidiStreamState()
        {
            NotesOn = new HashSet<byte>();
            CurrentTick = 0;
        }
    }
}
