using System;
using System.Collections;
using System.Collections.Generic;
using elZach.BulletEngine;
using UnityEngine;

public class AcceleratedTrajectory : BaseTrajectory
{
    public override Vector3 VisualEvaluate(Vector3 startPosition, Vector3 startDirection, float elapsedTime, object parameters)
    {
        var param = (Values) parameters;
        float distance = Mathf.Pow(elapsedTime, param.acceleration) * param.speed;
        return startPosition + startDirection * distance;
    }

    public override bool PhysicalEvaluate(IBulletSource source, Vector3 startPosition, Vector3 startDirection, float elapsedTime,
        Vector3 currentPosition, out Vector3 evaluatedPosition, out RaycastHit hit, object parameters)
    {
        var param = (Values) parameters;
        float distance = Mathf.Pow(elapsedTime, param.acceleration) * param.speed;
        evaluatedPosition = startPosition + startDirection * distance;
        var hitBool = Physics.Linecast(currentPosition, evaluatedPosition, out hit, source.CollisionLayer);
        return distance > param.range || (hitBool && hit.collider != source.Collider);
    }

    public new class Properties : BaseTrajectory.Properties
    {
        public float baseSpeed = 5f;
        public float range = 10f;
        public float acceleration = 1.5f;
    }

    public class Values : IHasSpeed
    {
        public float speed;
        public float range;
        public float acceleration;

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
            acceleration = props.acceleration
        };
    }
}
