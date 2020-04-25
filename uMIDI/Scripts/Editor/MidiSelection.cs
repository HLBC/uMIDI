using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace uMIDI
{

    /// <summary>
    /// Window editor for MIDI file selection
    /// </summary>
    public class MidiSelection : EditorWindow
    {
        public MidiSelection MidiSelectionWindow;

        public string Title;
        public int ColWidth;
        public int ColHeight;
        public int EspaceX;
        public int EspaceY;
        public int TitleHeight;
        public Color BackgroundColor;
        public int SelectedItem;
        public List<string> list;
        private Vector2 scrollPos;
        private int resizedWidth;
        private int resizedHeight;
        private int calculatedColCount;
        private int countRow;
        public Action<string, int> OnSelect;



        void OnGUI()
        {
            try
            {
                list = (new string[] {"test.midi", "background.midi", "effect.midi", "bell.midi"}).ToList();
                ColWidth = 250;
                ColHeight = 30;
                calculatedColCount = 3;
                EspaceX = 5;
                EspaceY = 5;
                TitleHeight = 30;
                countRow = (int)((float)list.Count / (float)calculatedColCount + 1f);
                resizedWidth = (int)MidiSelectionWindow.position.size.x;
                resizedHeight = (int)MidiSelectionWindow.position.size.y;

                DrawWindow();
            }
            catch (Exception ex)
            {
                Debug.Log("MidiSelection Exception: " + ex);
            }
        }

        private void DrawWindow()
        {
            int localstartX = 0;
            int localstartY = 0;
            int boxX = 0;
            int boxY = 0;

            Rect zone = new Rect(localstartX, localstartY, resizedWidth + EspaceX, resizedHeight + EspaceY);
            GUI.color = BackgroundColor;
            GUI.Box(zone, "");
            GUI.color = Color.white;

            Rect listVisibleRect = new Rect(localstartX, localstartY, resizedWidth - localstartX, resizedHeight - EspaceY);
            Rect listContentRect = new Rect(0, 0, calculatedColCount * (ColWidth + EspaceX) + 0, countRow * ColHeight + EspaceY);

            scrollPos = GUI.BeginScrollView(listVisibleRect, scrollPos, listContentRect);

            boxX = 0;
            boxY = 0;

            int indexList = -1;
            try
            {
                foreach (string item in list)
                {
                    if (item != null)
                    {
                        indexList++;

                        Rect rect = new Rect(boxX, boxY, ColWidth, ColHeight);

                        if (GUI.Button(rect, item))
                        {
                            SelectedItem = indexList;
                            if (OnSelect != null)
                                OnSelect(item, indexList);
                            
                            MidiSelectionWindow.Close();
                        }
                        GUI.color = Color.white;

                        if (calculatedColCount <= 1 || indexList % calculatedColCount == calculatedColCount - 1)
                        {
                            boxY += ColHeight;
                            boxX = 0;
                        }
                        else
                            boxX += ColWidth + EspaceX;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("MidiSelection Exception: " + ex);
            }
            GUI.EndScrollView();
        }
    }
}
