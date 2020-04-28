using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace uMIDI.Common
{
    /// <summary>
    /// Struct containing byte data for a MIDI message. (Just a status byte,
    /// array of data bytes, and time delta).
    /// </summary>
    public struct MidiMessage : IEquatable<MidiMessage>
    {
        public byte Status;
        public byte[] Data;
        public long TimeDelta;

        public bool Equals([AllowNull] MidiMessage other)
        {
            return Status == other.Status && Data.SequenceEqual(other.Data)
                && TimeDelta == other.TimeDelta;
        }

        public byte[] ToBytes()
        {
            // TODO return TimeDelta as well?
            byte[] arr = new byte[Data.Length + 1];
            arr[0] = Status;
            Data.CopyTo(arr, 1);
            return arr;
        }

        public override string ToString()
        {
            return String.Format("MidiMessage [Status: {0}, Data: [{1}], " +
                "TimeDelta: {2}]", Status, String.Join(", ", Data), TimeDelta);
        }
    }

    /// <summary>
    /// Interface wrapper for MidiMessages.
    /// </summary>
    public interface IMessage : IEquatable<IMessage>
    {
        MidiMessage Message { get; }
        long TimeDelta { get; }
    }

    /// <summary>
    /// Abstract implementation of IMessage. MidiMessage classes should extend
    /// this class. Default Equals() method defines equality as Message and
    /// TimeDelta properties being equal.
    /// </summary>
    public abstract class AbstractMessage : IMessage
    {
        public abstract MidiMessage Message { get; }

        public abstract long TimeDelta { get; }

        public bool Equals([AllowNull] IMessage other)
        {
            return Message.Equals(other.Message)
                && TimeDelta.Equals(other.TimeDelta);
        }
    }

    /// <summary>
    /// "note off" MIDI message.
    /// </summary>
    public class NoteOffMessage : AbstractMessage
    {
        public Note Note { get; }

        public override long TimeDelta { get; }

        public NoteOffMessage(Note note, long timeDelta)
        {
            Note = note;
            TimeDelta = timeDelta;
        }

        public override MidiMessage Message
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

    /// <summary>
    /// "note on" MIDI message.
    /// </summary>
    public class NoteOnMessage : AbstractMessage
    {
        public Note Note { get; }

        public override long TimeDelta { get; }

        public NoteOnMessage(Note note, long timeDelta)
        {
            Note = note;
            TimeDelta = timeDelta;
        }

        public override MidiMessage Message
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
