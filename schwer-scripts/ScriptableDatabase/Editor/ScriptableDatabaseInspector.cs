﻿using UnityEditor;
using UnityEngine;

namespace SchwerEditor.Database {
    using Schwer.Database;

    // [CustomEditor(typeof(TDatabase))]
    public class ScriptableDatabaseInspector<TDatabase, TElement> : Editor
        where TDatabase : ScriptableDatabase<TElement>
        where TElement : ScriptableObject {
        
        public override void OnInspectorGUI() {
            if (GUILayout.Button($"Regenerate {typeof(TDatabase).Name}")) {
                ScriptableDatabaseUtility<TDatabase, TElement>.GenerateDatabase();
            }
            GUILayout.Space(5);

            var arrayProperty = new SerializedObject((TDatabase)target).GetIterator();
            // `arrayProperty`: `Base`(?) to `Script`
            arrayProperty.NextVisible(true);
            // `arrayProperty`: `Script` to array – relies on the first serializable property being an array (or list)
            arrayProperty.NextVisible(true);
            if (arrayProperty.isArray && arrayProperty.propertyType != SerializedPropertyType.String) {
                var size = arrayProperty.arraySize;
                var name = arrayProperty.displayName;
                GUILayout.Label($"{name} ({size})");

                foreach (SerializedProperty elementProperty in arrayProperty) {
                    if (elementProperty.propertyType == SerializedPropertyType.ObjectReference) {
                        using (new EditorGUI.DisabledScope(true)) {
                            EditorGUILayout.PropertyField(elementProperty, GUIContent.none);
                        }
                    }
                }
            }
            else {
                EditorGUILayout.HelpBox($"Expected first serializable property in `{typeof(TDatabase).Name}` to be an array.", MessageType.Error);
            }
        }
    }
}
