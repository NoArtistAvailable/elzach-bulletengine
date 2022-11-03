using System;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class EvenEmitter : RandomEmitter
    {
        public new class Properties : BaseEmitter.Properties
        {
            public float spread = 5f;
            public int multiShot = 1;
        }

        public override Type GetPropertiesType() => typeof(Properties);

        public override void Emit(IBulletSource source, BaseEmitter.Properties properties, Action<RaycastHit> onHit, Action onDestroy)
        {
            var props = (Properties) properties;
            Quaternion rot;
            for (int i = 0; i < props.multiShot; i++)
            {
                rot = source.Muzzle.rotation * Quaternion.Euler(0f, ((float) i / (float) props.multiShot).Remap(-props.spread, props.spread), 0f);
                FireBullet(source, rot, onHit, onDestroy);
            }
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