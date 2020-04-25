using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public interface ITransform
    {
        MidiStream MidiStream { get; }
        LinkedList<IMessage> Apply(LinkedList<IMessage> buffer);
    }
}
