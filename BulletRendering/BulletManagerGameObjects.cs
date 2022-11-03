using System;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class BulletManagerGameObjects : BulletManager
    {
        public override void ImplementAddBullet(Bullet bullet, Weapon weapon)
        {
            var bulletObject = weapon.bulletSettings.prefab.Spawn().transform;
            var scale = weapon.bulletSettings.bulletSize.GetRandom();
            bulletObject.localScale = Vector3.one * scale;
            bulletObject.position = bullet.startPosition;
            bulletObject.rotation = Quaternion.LookRotation(bullet.startDirection, Vector3.up);
            var trail = bulletObject.GetComponent<TrailRenderer>();
            if (trail)
            {
                trail.widthMultiplier = scale * weapon.bulletSettings.relativeTrailWidth;
                trail.time = weapon.bulletSettings.bulletTrailLength.GetRandom();
                trail.Clear();
            }

            bullet.onDestroy += bulletObject.Despawn;
            
            if(!Instance.bullets.ContainsKey(weapon.trajectory)) Instance.bullets.Add(weapon.trajectory, new List<(Bullet, Action<Vector3>)>());

            void TransformUpdate(Vector3 position)
            {
                bulletObject.position = position;
            }
            Instance.bullets[weapon.trajectory].Add((bullet, TransformUpdate));
        }
    }
}