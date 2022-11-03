using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class SineTrajectory : BaseTrajectory
    {
        public override Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters)
        {
            var param = (Values) parameters;
            float distance = elapsedTime * param.speed;
            float sin = Mathf.Sin(distance * param.frequency) * param.amplitude;
            Vector3 crossDir = new Vector3(startDirection.z, startDirection.y, -startDirection.x);
            return startPosition + startDirection * distance + crossDir * sin;
        }

        public override bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection, float elapsedTime,
            Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit, object parameters)
        {
            var param = (Values) parameters;
            var distance = elapsedTime * param.speed;
            var pos = VisualEvaluate(startPosition, startDirection, elapsedTime, parameters);
            var hitBool = Physics.Linecast(currentPosition, pos, out hit, source.CollisionLayer);
            evaluatedPosition = pos;
            return distance > param.range || (hitBool && hit.collider != source.Collider);
        }

        public new class Properties : LinearTrajectory.Properties
        {
            public float frequency = 1f;
            public float amplitude = 1f;
            [NonSerialized] public bool pingPong;
        }

        public class Values : IHasSpeed
        {
            public float speed;
            public float range;
            public float frequency;
            public float amplitude;
            
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
            props.pingPong = !props.pingPong;
            return new Values()
            {
                speed = Random.Range(props.speed.x, props.speed.y),
                range = Random.Range(props.range.x, props.range.y),
                frequency = props.frequency * (props.pingPong ? 1f : -1f),
                amplitude = props.amplitude
            };
        }
    }
}
