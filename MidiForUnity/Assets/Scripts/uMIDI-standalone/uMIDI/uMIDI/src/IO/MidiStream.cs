using System.Collections.Generic;
using uMIDI.Common;

namespace uMIDI.IO
{
    public class MidiStream
    {
        public MidiStreamState State { get; }
        private ISet<IMidiInstrument> instruments;
        private LinkedList<IMessage> buffer;
        private int bufferCount;

        public MidiStream() : this(48)
        {

        }

        public MidiStream(int ticksPerBeat)
        {
            State = new MidiStreamState(ticksPerBeat);
            instruments = new HashSet<IMidiInstrument>();
            bufferCount = 0;
        }

        public void AddInstrument(IMidiInstrument instrument)
        {
            instruments.Add(instrument);
        }

        public void PushBuffer(IMessage[] messages)
        {
            // Add messages to buffer
            foreach (IMessage message in messages)
            {
                buffer.AddLast(message);
                bufferCount++;
            }
        }

        public void PushBuffer(LinkedList<IMessage> messages)
        {
            // Add messages to buffer
            foreach (IMessage message in messages)
            {
                buffer.AddLast(message);
                bufferCount++;
            }
        }

        public double MillisecondsPerTick()
        {
            // TODO
            return 0;
        }

        public void Update()
        {
            // Call ProcessMidi and clear buffer
            IMessage[] bufferArr = new IMessage[bufferCount];
            LinkedListNode<IMessage> msg = buffer.First;
            for (int i = 0; i < bufferCount; i++)
            {
                bufferArr[i] = msg.Value;
                msg = msg.Next;
            }

            foreach (IMidiInstrument inst in instruments)
            {
                inst.ProcessMidi(bufferArr);
            }

            buffer = new LinkedList<IMessage>();
        }
    }
}
