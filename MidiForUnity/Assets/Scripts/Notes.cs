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
        [SerializeField] private NoteName note;
        [SerializeField] private int octave;

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