using System;
using System.Diagnostics.CodeAnalysis;
using uMIDI.Common;

/// <summary>
/// Abstractions on top of <see cref="uMIDI.Common"/> to make transformations
/// more intuitive.
/// </summary>
namespace uMIDI.IO
{
    /// <summary>
    /// Abstraction upon <see cref="Note"/> which adds a start and end time
    /// to a note.
    /// </summary>
    public class TimedNote : Note, IEquatable<TimedNote>
    {
        // Absolute start time of note. null value denotes note is already being
        // played at instantiation.
        public long? StartTime { get; set; }
        // Absolute end time of note. null value denotes note is not released.
        public long? EndTime { get; set; }

        public TimedNote(byte channel, byte pitch, byte velocity,
            long? startTime, long? endTime) : base(channel, pitch, velocity)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool Equals([AllowNull] TimedNote other)
        {
            return base.Equals(other) && StartTime == other.StartTime
                && EndTime == other.EndTime;
        }

        public override string ToString()
        {
            return String.Format("(Note {2} {0} ({1}) {3} Start: {4} End: {5})",
                Pitch, Name(), Channel, Velocity, StartTime, EndTime);
        }
    }
}
