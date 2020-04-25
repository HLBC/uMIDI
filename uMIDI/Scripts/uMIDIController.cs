using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uMIDI
{   
    public class uMIDIController : MonoBehaviour
    {
        [SerializeField]
        public MidiStream stream;

        public float volume;

        public uMIDIEvent[] events = new uMIDIEvent[5];
    }

    [System.Serializable]
    public struct uMIDIEvent {
        [SerializeField]
        public Instrument[] instruments;

        [SerializeField]
        public Transformation[] transformations;

        public MidiEvent midiEvent;
    }

    [System.Serializable]
    public enum Transformation {
        Harmonize,
        PitchShift, 
        ChangeTemp
    }
}
