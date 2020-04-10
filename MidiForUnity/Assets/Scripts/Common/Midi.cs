namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte MsgSize;
        public byte[] Data;
        public long Time;
    }

    public interface IMessage
    {
        MidiMessage Message { get; }
    }

    public class NoteOffMessage : IMessage
    {
        public Note Note { get; }

        public long Time { get; }

        public NoteOffMessage(Note note, long time)
        {
            Note = note;
            Time = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x80 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    Time = Time
                };
            }
        }
    }

    public class NoteOnMessage : IMessage
    {
        public Note Note { get; }

        public long Time { get; }

        public NoteOnMessage(Note note, long time)
        {
            Note = note;
            Time = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x90 + Note.Channel),
                    MsgSize = 2,
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    Time = Time
                };
            }
        }
    }

    public class TimingTickMessage : IMessage
    {
        public long Time { get; }

        public TimingTickMessage(long time)
        {
            Time = time;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = 0xf8,
                    MsgSize = 0,
                    Data = new byte[0],
                    Time = Time
                };
            }
        }
    }
}
