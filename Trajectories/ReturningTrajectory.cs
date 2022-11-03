using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class ReturningTrajectory : BaseTrajectory
    {
        public override Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters)
        {
            var param = (Values) parameters;
            float distance = elapsedTime * (float) param.speed;
            float progress = distance / (float) param.range;
            distance = param.returnalCurve.Evaluate(progress) * param.range;
            return startPosition + Quaternion.Euler(0f,param.rotation * progress,0f) * startDirection * distance;
        }

        public override bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection, float elapsedTime,
            Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit, object parameters)
        {
            var param = (Values) parameters;
            var distance = elapsedTime * (float) param.speed;
            var pos = VisualEvaluate(startPosition, startDirection, elapsedTime, parameters);
            var hitBool = Physics.Linecast(currentPosition, pos, out hit, source.CollisionLayer);
            evaluatedPosition = pos;
            return distance > param.range || (hitBool && hit.collider != source.Collider);
        }

        public new class Properties : LinearTrajectory.Properties
        {
            public AnimationCurve returnalCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
            public float rotation;
        }

        public class Values : IHasSpeed
        {
            public float speed;
            public float range;
            public AnimationCurve returnalCurve;
            public float rotation;
            
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
                speed = Random.Range(props.speed.x, props.speed.y),
                range = Random.Range(props.range.x, props.range.y),
                returnalCurve = props.returnalCurve,
                rotation = props.rotation
            };
        }
    }
}