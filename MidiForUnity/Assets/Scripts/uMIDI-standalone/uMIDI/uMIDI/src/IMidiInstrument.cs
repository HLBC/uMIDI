using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI
{
    public interface IMidiInstrument
    {
        void ProcessMidi(IMessage[] messages);
        MidiStream Stream { get; }
    }
}
