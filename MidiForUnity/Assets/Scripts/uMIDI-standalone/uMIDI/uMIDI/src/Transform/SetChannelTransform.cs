using System;
using uMIDI.IO;

namespace uMIDI.Transform
{
    public class SetChannelTransform : ITransform
    {
        public byte Channel { get; }

        public SetChannelTransform(byte channel)
        {
            if (channel < 0 || channel >= 16)
            {
                throw new ArgumentException("Channel must be between 0 and 15");
            }
            Channel = channel;
        }

        public void Apply(Region region)
        {
            foreach (TimedNote note in region)
            {
                note.Channel = Channel;
            }
        }
    }
}
