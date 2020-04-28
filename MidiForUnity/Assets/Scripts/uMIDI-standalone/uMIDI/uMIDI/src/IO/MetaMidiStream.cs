using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.Transform;

namespace uMIDI.IO
{
    public class MetaMidiStream
    {
        public MetaState MetaState { get; private set; }
        public MidiStream MidiStream { get; }
        public List<ITransform> Transforms { get; set; }
        public Dictionary<string, List<ITransform>> FileTransforms { get; set; }
        private Dictionary<string, MidiFile> _files;

        public MidiClock Clock { get => MidiStream.State.Clock; }

        public MetaMidiStream()
        {
            MetaState = new MetaState();
            MidiStream = new MidiStream();
            Transforms = new List<ITransform>();
            _files = new Dictionary<string, MidiFile>();
            FileTransforms = new Dictionary<string, List<ITransform>>();
        }

        public MetaMidiStream(int ticksPerBeat)
        {
            MetaState = new MetaState();
            MidiStream = new MidiStream(ticksPerBeat);
            Transforms = new List<ITransform>();
            _files = new Dictionary<string, MidiFile>();
            FileTransforms = new Dictionary<string, List<ITransform>>();
        }

        public MetaMidiStream(int ticksPerBeat, int beatsPerMeasure,
            int subdivision, int bpm, KeySignature key)
        {
            MetaState = new MetaState(beatsPerMeasure, subdivision, bpm, key);
            MidiStream = new MidiStream(ticksPerBeat);
            Transforms = new List<ITransform>();
            _files = new Dictionary<string, MidiFile>();
            FileTransforms = new Dictionary<string, List<ITransform>>();
        }

        public void addFile(string name, MidiFile file)
        {
            _files.Add(name, file);
        }

        public bool removeFile(string name)
        {
            return _files.Remove(name);
        }

        public void PushBuffer(IMessage[] messages)
        {
            // Convert to region
            Region region = Region.Messages2Region(messages, MetaState);

            foreach (ITransform transform in Transforms)
            {
                transform.Apply(region);
            }

            (IMessage[] newBuffer, MetaState newState) = Region.Region2Messages(
                region);
            MetaState = newState;

            MidiStream.PushBuffer(newBuffer);
        }
    }
}
