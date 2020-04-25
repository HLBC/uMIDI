using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


namespace uMIDI
{
    [CustomEditor(typeof(uMIDIController))]
    public class uMIDIEditor : Editor
    {

        private MidiSelection midiSelection;
        private string backgroundSelectionTitle = "Select MIDI background music file";
        private string eventSelectionTitle = "Select a MIDI event file";

        private void onBackgroundSelection(string filename, int index)
        {
            backgroundSelectionTitle = "Background Music File: " + filename;
            Repaint();
        }

        private void onEventSelection(string filename, int index)
        {
            eventSelectionTitle = "Event Music File: " + filename;
            Repaint();
        }

        public void initializeMidiSelection(int selected, Action<string, int> OnSelect)
        {
            try
            {
                midiSelection = EditorWindow.GetWindow<MidiSelection>(true, "Select a Midi File");
                midiSelection.MidiSelectionWindow = midiSelection;
                midiSelection.OnSelect = OnSelect;
                midiSelection.SelectedItem = selected;
                midiSelection.BackgroundColor = new Color(.8f, .8f, .8f, 1f);
            }
            catch (System.Exception ex)
            {
                Debug.Log("uMidiEditor Exception: " + ex);
            }
        }

        public void createSelectionWindow(string title,  Action<string, int> OnSelect) {
            if (GUILayout.Button(title)) {
                initializeMidiSelection(0, OnSelect);
            }
        }

        public override void OnInspectorGUI() {
            SerializedProperty stream = serializedObject.FindProperty("stream");
            EditorGUILayout.PropertyField(stream);
            SerializedProperty volume = serializedObject.FindProperty("volume");
            EditorGUILayout.Slider(volume, 0, 100);
            createSelectionWindow(backgroundSelectionTitle, onBackgroundSelection);
            uMIDIController controller = (uMIDIController) target;
            SerializedProperty events = serializedObject.FindProperty("events");
            for (int i = 0; i < controller.events.Length; i++)
            {
                SerializedProperty eventProperty = events.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();          
    
                EditorGUILayout.PropertyField(eventProperty, true);
            
                if (EditorGUI.EndChangeCheck())
                {              
                    serializedObject.ApplyModifiedProperties();
                }

                createSelectionWindow(eventSelectionTitle, onEventSelection);
            } 
            serializedObject.ApplyModifiedProperties();
        }
    }
}
