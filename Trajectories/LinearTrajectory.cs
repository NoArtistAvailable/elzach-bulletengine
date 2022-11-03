using System;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class LinearTrajectory : BaseTrajectory
    {
        public override object GetParameters(IBulletSource source)
        {
            var props = (Properties) source.Weapon.trajectoryProperties;
            var array = new Values()
            {
                speed = Random.Range(props.speed.x, props.speed.y),
                range = Random.Range(props.range.x, props.range.y)
            };
            return array;
        }

        public override Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters)
        {
            var param = (Values) parameters;
            return startPosition + startDirection * elapsedTime * param.speed;
        }

        public override bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection, float elapsedTime,
            Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit, object parameters)
        {
            var param = (Values) parameters;
            var distance = elapsedTime * param.speed;
            var pos = startPosition + startDirection * distance;
            var hitBool = Physics.Linecast(currentPosition, pos, out hit, source.CollisionLayer);
            evaluatedPosition = pos;
            return distance > param.range || (hitBool && hit.collider != source.Collider);
        }

        public new class Properties : BaseTrajectory.Properties
        {
            public Vector2 speed = new Vector2(20, 22);
            public Vector2 range = new Vector2(30, 35);
        }

        public class Values : IHasSpeed
        {
            public float speed;
            public float range;
            public float Speed
            {
                get => speed;
                set => speed = value;
            }
        }
        public override Type GetPropertiesType() => typeof(Properties);
    }
}