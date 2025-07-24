// Detects different publisher events when you switch from edit mode to play mode.
using UnityEngine;
using UnityEditor;

namespace JsonQuizEditor.Scripts
{
    [InitializeOnLoadAttribute]
    public static class PlayModeChangedEditor
    {
        static PlayModeChangedEditor () {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state) {            
            JsonQuizWindow winCurrentEditor = null;
            JsonQuizWindow [] winEditors = Resources.FindObjectsOfTypeAll<JsonQuizWindow>();
            if (winEditors != null) {
                foreach (JsonQuizWindow winEditor in winEditors) {
                    winCurrentEditor = winEditor; // Find the reference for the question editor.
                }
            }
            // Check the status of the publisher and send an event when changing operating modes.
            if ( state == PlayModeStateChange.EnteredEditMode ) {
                if (winCurrentEditor != null) {
                    winCurrentEditor.SendEvent(EditorGUIUtility.CommandEvent(GlobalEditorVariables.enteredEditModeEvent));
                }
            }
            if ( state == PlayModeStateChange.ExitingEditMode ) {
                if (winCurrentEditor != null) {
                    winCurrentEditor.SendEvent(EditorGUIUtility.CommandEvent(GlobalEditorVariables.exitingEditModeEvent));
                }                
            }
            if ( state == PlayModeStateChange.EnteredPlayMode ) {
                if (winCurrentEditor != null) {
                    winCurrentEditor.SendEvent(EditorGUIUtility.CommandEvent(GlobalEditorVariables.enteredPlayModeEvent));
                }                
            }
            if ( state == PlayModeStateChange.ExitingPlayMode ) {
                if (winCurrentEditor != null) {
                    winCurrentEditor.SendEvent(EditorGUIUtility.CommandEvent(GlobalEditorVariables.exitingPlayModeEvent));
                }                 
            }                                   
        }
    }
}