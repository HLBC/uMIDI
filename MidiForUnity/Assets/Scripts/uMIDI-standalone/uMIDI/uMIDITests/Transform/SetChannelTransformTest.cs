using System;
using System.Collections.Generic;
using uMIDI.Common;
using uMIDI.IO;
using uMIDI.Transform;
using Xunit;

namespace uMIDITests.Transform
{
    public class SetChannelTransformTest
    {
        private TimedNote[] notes;
        private TimedNote[] notesSameChannel;
        private SetChannelTransform transform;

        public SetChannelTransformTest()
        {
            Console.WriteLine("Setting up...");

            notes = new TimedNote[]
            {
                new TimedNote(1, 60, 127, null, 100),
                new TimedNote(2, 62, 127, 100, 200),
                new TimedNote(3, 64, 127, 200, 300),
                new TimedNote(4, 62, 127, 300, 400),
                new TimedNote(5, 60, 127, 400, 500)
            };

            notesSameChannel = new TimedNote[]
            {
                new TimedNote(1, 60, 127, null, 100),
                new TimedNote(1, 62, 127, 100, 200),
                new TimedNote(1, 64, 127, 200, 300),
                new TimedNote(1, 62, 127, 300, 400),
                new TimedNote(1, 60, 127, 400, 500)
            };

            transform = new SetChannelTransform(1);
        }

        [Fact]
        public void TestSameChannel()
        {
            MetaState metaState = new MetaState(4, 2, 120,
                new KeySignature(Tonic.C, Scale.MAJOR));
            Region region = new Region(new List<TimedNote>(notes), metaState);

            Region expected = new Region(new List<TimedNote>(notesSameChannel),
                metaState);

            transform.Apply(region);

            Assert.Equal(expected, region);
        }
    }
}
