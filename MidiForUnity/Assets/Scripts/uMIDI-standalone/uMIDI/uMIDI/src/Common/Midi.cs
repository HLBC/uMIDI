using System;
using System.Linq;

namespace uMIDI.Common
{
    public struct MidiMessage
    {
        public byte Status;
        public byte[] Data;
        public long TimeDelta;

        public byte[] ToBytes()
        {
            // TODO return TimeDelta as well?
            byte[] arr = new byte[Data.Length + 1];
            arr[0] = Status;
            Data.CopyTo(arr, 1);
            return arr;
        }
    }

    public interface IMessage
    {
        MidiMessage Message { get; }

        long TimeDelta { get; }
    }

    public class NoteOffMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta {
            get => Note.Time;
        }

        public NoteOffMessage(Note note)
        {
            Note = note;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x80 + Note.Channel),
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }

    public class NoteOnMessage : IMessage
    {
        public Note Note { get; }

        public long TimeDelta { get => Note.Time; }

        public NoteOnMessage(Note note)
        {
            Note = note;
        }

        public MidiMessage Message
        {
            get
            {
                return new MidiMessage
                {
                    Status = (byte)(0x90 + Note.Channel),
                    Data = new byte[] { Note.Pitch, Note.Velocity },
                    TimeDelta = TimeDelta
                };
            }
        }
    }
}
