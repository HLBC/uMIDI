using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;
using uMIDI.Transform;

namespace uMIDI
{
    public class MidiFile
    {
        public MetaMidiStream MetaStream { get; set; }
        public int BufferSize { get; }
        public bool EndOfFile { get; private set; }
        public int MessageCount { get => _messages.Count; }
        private List<IMessage> _messages;
        private int _playHead;

        // Re-use the same memory for performance
        private IMessage[] _buffer;
        private IMessage[] _lastbuffer;

        public MidiFile(int bufferSize, int ticksPerBeat,
            List<IMessage> messages) :
            this (new MetaMidiStream(ticksPerBeat), messages, bufferSize)
        {

        }

        public MidiFile(MetaMidiStream stream, List<IMessage> messages,
            int bufferSize)
        {
            MetaStream = stream;
            BufferSize = bufferSize;
            EndOfFile = false;
            _messages = messages;
            _playHead = 0;
            _buffer = new IMessage[BufferSize];
            _lastbuffer = new IMessage[MessageCount % BufferSize];
        }

        public MidiFile(int ticksPerBeat, int bufferSize, int beatsPerMeasure,
            int subdivision, int bpm, List<IMessage> messages,
            KeySignature key) :
            this(new MetaMidiStream(ticksPerBeat, beatsPerMeasure, subdivision,
                    bpm, key), messages, bufferSize)
        {

        }

        public void AddMessage(IMessage messageData)
        {
            _messages.Add(messageData);
            _lastbuffer = new IMessage[MessageCount % BufferSize];
        }

        public void AddMessage(List<IMessage> messageData)
        {
            _messages.AddRange(messageData);
            _lastbuffer = new IMessage[MessageCount % BufferSize];
        }

        public void Reset()
        {
            _playHead = 0;
            _lastbuffer = new IMessage[MessageCount % BufferSize];
        }

        public void PushNextBuffer()
        {
            IMessage[] buffer;
            int bufferSize;
            if (_playHead + BufferSize >= MessageCount)
            {
                buffer = _lastbuffer;
                bufferSize = _lastbuffer.Length;
                EndOfFile = true;
            }
            else
            {
                buffer = _buffer;
                bufferSize = BufferSize;
            }

            for (int i = 0; i < bufferSize; i++)
            {
                buffer[i] = _messages[_playHead + i];
            }
            _playHead += bufferSize;

            MetaStream.PushBuffer(buffer);
        }

        public Region ToRegion()
        {
            return Region.Messages2Region(_messages.ToArray(),
                MetaStream.MetaState);
        }
    }
}
