using System;
using System.Collections;
using System.Collections.Generic;
using elZach.BulletEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Weapon weaponData = new Weapon();
    
    void OnValidate()
    {
        if (!weaponData.trajectory) weaponData.trajectoryProperties = null;
        else if (weaponData.trajectoryProperties == null || weaponData.trajectoryProperties.GetType() != weaponData.trajectory.GetPropertiesType())
            weaponData.trajectoryProperties = (BaseTrajectory.Properties) Activator.CreateInstance(weaponData.trajectory.GetPropertiesType());
            
        if (!weaponData.emitter) weaponData.emitterProperties = null;
        else if (weaponData.emitterProperties == null || weaponData.emitterProperties.GetType() != weaponData.emitter.GetPropertiesType())
            weaponData.emitterProperties = (BaseEmitter.Properties) Activator.CreateInstance(weaponData.emitter.GetPropertiesType());
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponHolder))]
    public class Inspector : Editor
    {
        private SerializedProperty property;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // property = serializedObject.FindProperty(nameof(WeaponHolder.weaponData));
            // foreach(var prop in GetVisibleChildren(property)) EditorGUILayout.PropertyField(prop);
            // serializedObject.ApplyModifiedProperties();
            
            // var enumerator = property.GetEnumerator();
            // while (property.NextVisible(false)) {
            //     var prop = property as SerializedProperty;
            //     if (prop == null) continue;
            //     Debug.Log(prop.name);
            //     //Add your treatment to the current child property...
            //     EditorGUILayout.PropertyField(prop);
            // }
        }
        
        public static IEnumerable<SerializedProperty> GetVisibleChildren(SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }
 
            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;
 
                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
        
    }
#endif
    
}
