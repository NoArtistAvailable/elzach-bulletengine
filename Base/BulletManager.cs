using System;
using System.Collections.Generic;
using System.Linq;
using elZach.Common;
using UnityEngine;

namespace elZach.BulletEngine
{
    public class BulletManager : MonoBehaviour
    {
        private static BulletManager _instance;
        public static BulletManager Instance => _instance.OrSet(ref _instance, FindObjectOfType<BulletManager>);

        public Dictionary<BaseTrajectory, List<(Bullet entity, Action<Vector3> positionCallback)>> bullets =
            new Dictionary<BaseTrajectory, List<(Bullet entity, Action<Vector3> positionCallback)>>();

        private List<(Bullet entity, Action<Vector3> positionCallback)> toDestroy = new List<(Bullet entity, Action<Vector3> positionCallback)>();
        private List<(Bullet entity, Action<Vector3> positionCallback)> toDestroy2 = new List<(Bullet entity, Action<Vector3> positionCallback)>();

        private FutureStep stepNow = new FutureStep() {delay = 0f};
        private FutureStep stepImmediate = new FutureStep() {delay = 0.1f};
        private FutureStep stepHalf = new FutureStep() {delay = 0.25f};
        private FutureStep stepFull = new FutureStep() {delay = 0.5f};

        public static float quadrantSize = 10f;
        
        public class FutureStep
        {
            public float delay = 1f;

            public Dictionary<Vector2Int, List<(Bullet entity, Vector3 position)>> quadrants =
                new Dictionary<Vector2Int, List<(Bullet entity, Vector3 position)>>();
            // public List<(Bullet entity, Vector3 position)> futurePositions = new List<(Bullet, Vector3)>();

            public void Add(Bullet entity, BaseTrajectory trajectory)
            {
                var pos = trajectory.VisualEvaluate(entity.startPosition, entity.startDirection, Time.time + delay, entity.parameters);
                Vector2Int quadrant = new Vector2Int(Mathf.FloorToInt(pos.x / quadrantSize), Mathf.FloorToInt(pos.z / quadrantSize));

                if (!quadrants.ContainsKey(quadrant))
                {
                    quadrants.Add(quadrant,new List<(Bullet entity, Vector3 position)>());
                    // Debug.Log($"Added bullet {pos} to new quadrant {quadrant}");
                }
                quadrants[quadrant].Add((entity, pos));
            }

            public void AddCurrentAndPos(Bullet entity)
            {
                Vector2Int quadrant = new Vector2Int(Mathf.FloorToInt(entity.currentPosition.x / quadrantSize), Mathf.FloorToInt(entity.currentPosition.z / quadrantSize));
                if (!quadrants.ContainsKey(quadrant))
                {
                    quadrants.Add(quadrant,new List<(Bullet entity, Vector3 position)>());
                    // Debug.Log($"Added bullet {pos} to new quadrant {quadrant}");
                }
                quadrants[quadrant].Add((entity, entity.currentPosition));
            }

            public void Clear() => quadrants.Clear();
        }

        void FixedUpdate()
        {
            foreach (var (trajectory, entityTransformPairs) in bullets)
            {
                for (var i = entityTransformPairs.Count - 1; i >= 0; i--)
                {
                    var bullet = entityTransformPairs[i];
                    if (!trajectory.PhysicalEvaluate(bullet.entity, out var hit)) continue;
                    if (hit.collider)
                    {
                        bullet.positionCallback?.Invoke(hit.point);
                        bullet.entity.Hit(hit);
                    }
                    toDestroy.Add(bullet);
                    entityTransformPairs.RemoveAt(i);
                }
            }
        }

        protected virtual void Update()
        {
            foreach (var bullet in toDestroy2) bullet.entity.Destroy();
            toDestroy2.Clear();
            foreach (var bullet in toDestroy) toDestroy2.Add(bullet);
            toDestroy.Clear();
            
            
            stepNow.Clear();
            stepImmediate.Clear();
            stepHalf.Clear();
            stepFull.Clear();
            
            foreach (var (trajectory, entityTransformPair) in bullets)
            foreach (var (entity, positionCallback) in entityTransformPair)
            {
                positionCallback?.Invoke(trajectory.VisualEvaluate(entity));
                stepNow.AddCurrentAndPos(entity);
                stepImmediate.Add(entity, trajectory);
                stepHalf.Add(entity, trajectory);
                stepFull.Add(entity, trajectory);
            }
            // Debug.Log($"halfStep quadrants: {stepHalf.quadrants.Count} values: {stepHalf.quadrants.Values.Count}");
        }

        public static void AddBullet(Bullet bullet, Weapon weapon) => Instance.ImplementAddBullet(bullet, weapon);

        public virtual void ImplementAddBullet(Bullet bullet, Weapon weapon){}

        public bool IsItSafe(IBulletSource source, Vector3 position, float sqrRadius, out Vector3 incoming)
        {
            var quadrant = new Vector2Int(Mathf.FloorToInt(position.x / quadrantSize),
                Mathf.FloorToInt(position.z / quadrantSize));

            // for (int x = -initialQuadrant.x; x <= initialQuadrant.x; x++)
            // for (int z = -initialQuadrant.y; z <= initialQuadrant.y; z++)
            {
                // var quadrant = new Vector2Int(x, z);
                if (stepNow.quadrants.ContainsKey(quadrant))
                    foreach (var bullet in stepNow.quadrants[quadrant].Where(bullet => bullet.entity.source != source))
                    {
                        incoming = bullet.position;
                        if ((position - bullet.position).sqrMagnitude < sqrRadius) return false;
                    }

                if (stepImmediate.quadrants.ContainsKey(quadrant))
                    foreach (var bullet in stepImmediate.quadrants[quadrant]
                        .Where(bullet => bullet.entity.source != source))
                    {
                        incoming = bullet.position;
                        if ((position - bullet.position).sqrMagnitude < sqrRadius) return false;
                    }
                
                if (stepHalf.quadrants.ContainsKey(quadrant))
                    foreach (var bullet in stepHalf.quadrants[quadrant].Where(bullet => bullet.entity.source != source))
                    {
                        incoming = bullet.position;
                        if ((position - bullet.position).sqrMagnitude < sqrRadius) return false;
                    }
                
                if (stepFull.quadrants.ContainsKey(quadrant))
                    foreach (var bullet in stepFull.quadrants[quadrant].Where(bullet => bullet.entity.source != source))
                    {
                        incoming = bullet.position;
                        if ((position - bullet.position).sqrMagnitude < sqrRadius) return false;
                    }
            }

            incoming = Vector3.zero;
            return true;
        }

    }
}
