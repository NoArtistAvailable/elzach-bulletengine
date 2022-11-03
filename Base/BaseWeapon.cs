using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using elZach.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    [CreateAssetMenu(menuName = "BulletEngine/Weapon")]
    public class BaseWeapon : ScriptableObject
    {
        public GameObject weaponAppearance;
        public object GetTrajectoryValues(IBulletSource source) => weaponData.trajectory.GetParameters(source);
        
        public Weapon weaponData;
        
        void OnValidate()
        {
            if (!weaponData.trajectory) weaponData.trajectoryProperties = null;
            else if (weaponData.trajectoryProperties == null || !(weaponData.trajectoryProperties.GetType() == weaponData.trajectory.GetPropertiesType()))
                weaponData.trajectoryProperties = (BaseTrajectory.Properties) Activator.CreateInstance(weaponData.trajectory.GetPropertiesType());
            
            if (!weaponData.emitter) weaponData.emitterProperties = null;
            else if (weaponData.emitterProperties == null || !(weaponData.emitterProperties.GetType() == weaponData.emitter.GetPropertiesType()))
                weaponData.emitterProperties = (BaseEmitter.Properties) Activator.CreateInstance(weaponData.emitter.GetPropertiesType());
        }
    }

    [Serializable]
    public class Weapon
    {
        public BaseTrajectory trajectory;
        public BaseEmitter emitter;
        public object GetTrajectoryValues(IBulletSource source) => trajectory.GetParameters(source);
        
        [Range(0f,1f)] 
        public float movementModifier = 0.3f;
        public float fireRate = 2f;
        public int magazineSize = 3;
        public float reloadTime = 0f;
        public int damage = 1;

        [SerializeReference] public BaseTrajectory.Properties trajectoryProperties;
        [SerializeReference] public BaseEmitter.Properties emitterProperties;
        public BulletSettings bulletSettings;
        
        void OnValidate()
        {
            if (!trajectory) trajectoryProperties = null;
            else if (trajectoryProperties == null || trajectoryProperties.GetType() != trajectory.GetPropertiesType())
                trajectoryProperties = (BaseTrajectory.Properties) Activator.CreateInstance(trajectory.GetPropertiesType());
            
            if (!emitter) emitterProperties = null;
            else if (emitterProperties == null || emitterProperties.GetType() != emitter.GetPropertiesType())
                emitterProperties = (BaseEmitter.Properties) Activator.CreateInstance(emitter.GetPropertiesType());
        }
    }

}
