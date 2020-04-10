using System;
using UnityEngine;

namespace Notes
{
    // Note... are structures used by the system
    public enum NoteName
    {
        C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B
    }

    [Serializable]
    public class Note
    {
        public static readonly int ALL_NOTES = 12;
        public static readonly int BASE_NOTES = 7;

        [SerializeField] private NoteName note;
        public NoteName Name { get { return note; } }
        [SerializeField] private int octave;
        public int Octave { get { return octave; } }

        public Note()
        {
            note = NoteName.A;
            octave = 4;
        }

        public Note(NoteName note, int octave)
        {
            this.note = note;
            this.octave = octave;
        }

        public int KeyPos()
        {
            return 12 * octave + ((int) note - 8);
        }
    }
}