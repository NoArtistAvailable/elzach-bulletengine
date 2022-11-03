using System;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;

namespace elZach.BulletEngine
{
    public class MeshEmitter : BaseEmitter
    {
        public new class Properties : BaseEmitter.Properties
        {
            public Mesh mesh;
            public Vector3 rotation;
            public float speedMult = 1f;

            public bool spawnAllAtOnce = true;
            [NonSerialized] public int currentIndex;
            
            private Vector3[] _verts = new Vector3[0];

            public Vector3[] verts
            {
                get
                {
                    //return mesh.vertices;
                    if (_verts.Length != mesh.vertexCount)
                    {
                        _verts = mesh.vertices;
                    }
                    return _verts;
                }
            }

        }

        public override Type GetPropertiesType() => typeof(Properties);

        public override void Emit(IBulletSource source, BaseEmitter.Properties properties, Action<RaycastHit> onHit, Action onDestroy)
        {
            var props = (Properties) properties;
            var baseRotation = Quaternion.Euler(props.rotation);
            if (!props.spawnAllAtOnce)
            {
                var dir = source.Muzzle.rotation * baseRotation * props.verts[props.currentIndex];
                
                var parameters = (BaseTrajectory.IHasSpeed) source.Weapon.GetTrajectoryValues(source);
                parameters.Speed = parameters.Speed * dir.magnitude * props.speedMult;
                
                props.currentIndex++;
                props.currentIndex %= props.verts.Length;
                
                BulletManager.AddBullet(
                    new Bullet(source, source.Muzzle.position,  dir.normalized, parameters, onHit, onDestroy),
                    source.Weapon
                );
                return;
            }
            foreach (var position in props.verts)
            {
                var dir = source.Muzzle.rotation * baseRotation * position;

                var parameters = (BaseTrajectory.IHasSpeed) source.Weapon.GetTrajectoryValues(source);
                parameters.Speed = parameters.Speed * dir.magnitude * props.speedMult;
                
                BulletManager.AddBullet(
                    new Bullet(source, source.Muzzle.position,  dir.normalized, parameters, onHit, onDestroy),
                    source.Weapon
                );
            }
        }
        
        public override IEnumerable<Bullet> PreviewBullets(IBulletSource source, BaseEmitter.Properties properties)
        {
            var props = (MeshEmitter.Properties) properties;
            var baseRotation = Quaternion.Euler(props.rotation);
            foreach (var vert in props.verts)
            {
                var dir = source.Muzzle.rotation * baseRotation * vert;

                var parameters = (BaseTrajectory.IHasSpeed) source.Weapon.GetTrajectoryValues(source);
                parameters.Speed = parameters.Speed * dir.magnitude * props.speedMult;
                yield return new Bullet(source, source.Muzzle.position, dir, parameters, null, null);
            }
        }
    }
}