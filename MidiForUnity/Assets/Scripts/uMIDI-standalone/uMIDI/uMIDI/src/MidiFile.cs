using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;

namespace uMIDI
{
    public class MidiFile
    {
        public MetaMidiStream MetaStream { get; }
        public int BufferSize { get; }
        public bool EndOfFile { get; }
        private List<IMessage> _messages;
        private int _playHead;

        public MidiFile(MetaMidiStream stream, List<IMessage> messages, int bufferSize)
        {
            MetaStream = stream;
            BufferSize = bufferSize;
            EndOfFile = false;
            _messages = new List<IMessage>();
            _playHead = 0;
        }

        public void AddMessage(IMessage messageData)
        {
            _messages.Add(messageData);
        }

        public void AddMessage(List<IMessage> messageData)
        {
            _messages.AddRange(messageData);
        }

        public MidiFile(int bufferSize, int beatsPerMeasure, int subdivision,
            int bpm, List<IMessage> messages) : this(
                new MetaMidiStream(beatsPerMeasure, subdivision, bpm),
                messages, bufferSize)
        {

        }

        public void PushNextBuffer()
        {

        }
    }
}
