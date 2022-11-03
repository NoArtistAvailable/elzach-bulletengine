using System;
using UnityEngine;

namespace elZach.BulletEngine
{
    public abstract class BaseTrajectory : ScriptableObject
    {
        public abstract Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters);
        public abstract bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection,
            float elapsedTime, Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit,
            object parameters);
        
        [Serializable]
        public abstract class Properties{ }
        public abstract Type GetPropertiesType();
        public abstract object GetParameters(IBulletSource source);
        
        public Vector3 VisualEvaluate(Bullet bullet) => VisualEvaluate(bullet.startPosition, bullet.startDirection,
            Time.time - bullet.dynamicStartTime, 
            bullet.parameters);
        public virtual bool PhysicalEvaluate(Bullet bullet, out RaycastHit hit) => PhysicalEvaluate(bullet.source,
            bullet.startPosition, bullet.startDirection, 
            Time.fixedTime - bullet.physicsStartTime, 
            bullet.currentPosition, 
            out bullet.currentPosition, 
            out hit,
            bullet.parameters);

        public interface IHasSpeed
        {
            public float Speed { get; set; }
        }
    }
}
