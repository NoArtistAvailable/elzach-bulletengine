using System;
using UnityEngine;

namespace elZach.BulletEngine
{
    public class Flatten3DTrajectory : BaseTrajectory
    {
        public new class Properties : BaseTrajectory.Properties
        {
            public float baseSpeed = 5f;
            public float range = 10f;
            public float acceleration = 1.5f;
            public float xRotation = 0;
            public float directionSpeed = 0f;
        }

        public class Values : IHasSpeed
        {
            public float speed;
            public float range;
            public float acceleration;
            public float xRotation;
            public Vector3 rotationAxis;
            public Vector3 directedSpeed;
            
            public float Speed
            {
                get => speed;
                set => speed = value;
            }
        }

        public override Type GetPropertiesType() => typeof(Properties);

        public override object GetParameters(IBulletSource source)
        {
            var props = (Properties) source.Weapon.trajectoryProperties;
            return new Values()
            {
                speed = props.baseSpeed,
                range = props.range,
                acceleration = props.acceleration,
                xRotation = props.xRotation,
                rotationAxis = source.Muzzle.right,
                directedSpeed = source.Muzzle.forward * props.directionSpeed
            };
        }
        
        public override Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters)
        {
            var param = (Values) parameters;
            float distance = Mathf.Pow(elapsedTime, param.acceleration) * param.speed;
            var pos = startPosition + Quaternion.AngleAxis((float) param.xRotation * distance, param.rotationAxis) 
                                    * startDirection * distance 
                                    + param.directedSpeed * elapsedTime;
            pos.y = startPosition.y;
            return pos;
        }

        public override bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection, float elapsedTime,
            Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit, object parameters)
        {
            var param = (Values) parameters;
            float distance = Mathf.Pow(elapsedTime, param.acceleration) * param.speed;
            evaluatedPosition = VisualEvaluate(startPosition, startDirection, elapsedTime, parameters);
            var hitBool = Physics.Linecast(currentPosition, evaluatedPosition, out hit, source.CollisionLayer);
            return distance > param.range || (hitBool && hit.collider != source.Collider);
        }
    }
}