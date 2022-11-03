using System;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.BulletEngine
{
    public abstract class BaseEmitter : ScriptableObject
    {
        [System.Serializable]
        public class Properties{}
        public abstract Type GetPropertiesType();

        public abstract void Emit(IBulletSource source, Properties properties, Action<RaycastHit> onHit, Action onDestroy);

        public abstract IEnumerable<Bullet> PreviewBullets(IBulletSource source, Properties properties);
    }
}