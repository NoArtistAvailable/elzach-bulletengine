using System;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class RandomEmitter : BaseEmitter
    {
        public new class Properties : BaseEmitter.Properties
        {
            public float spread = 5f;
            public float multiShot = 1f;
        }
        public override Type GetPropertiesType() => typeof(Properties);
        public override void Emit(IBulletSource source, BaseEmitter.Properties properties, Action<RaycastHit> onHit, Action onDestroy)
        {
            var props = (Properties) properties;
            int count = Mathf.FloorToInt(props.multiShot) + (Random.value < (props.multiShot % 1f) ? 1 : 0);
            Quaternion rot;
            for (int i = 0; i < count; i++)
            {
                rot = source.Muzzle.rotation * Quaternion.Euler(0f, Random.Range(-props.spread, props.spread), 0f);
                FireBullet(source, rot, onHit, onDestroy);
            }
        }
        
        protected void FireBullet(IBulletSource source, Quaternion rotation, Action<RaycastHit> onHit, Action onDestroy)
        {
            var bullet = new Bullet(source, source.Muzzle.position, rotation * Vector3.forward,
                source.Weapon.GetTrajectoryValues(source), 
                onHit, 
                onDestroy);
            BulletManager.AddBullet(bullet, source.Weapon);
        }
        
        public override IEnumerable<Bullet> PreviewBullets(IBulletSource source, BaseEmitter.Properties properties)
        {
            var props = (Properties) properties;
            for (int i = 0; i < props.multiShot; i++)
            {
                var rot = source.Muzzle.rotation * Quaternion.Euler(0f, ((float) i / (float) props.multiShot).Remap(-props.spread, props.spread), 0f);
                yield return new Bullet(source, source.Muzzle.position, rot * source.Muzzle.forward,
                    source.Weapon.GetTrajectoryValues(source), null, null);
            }
        }
    }
}