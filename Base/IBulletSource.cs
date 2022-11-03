using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace elZach.BulletEngine
{
    public interface IBulletSource
    {
        public LayerMask CollisionLayer { get; }
        public Weapon Weapon { get; }
        public Transform Muzzle { get; }
        public Collider Collider { get; }
    }
}
