using System;
using System.Collections.Generic;
using elZach.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class BulletManagerParticleSystem : BulletManager
    {
        private static BulletManagerParticleSystem _instance;
        public new static BulletManagerParticleSystem Instance => _instance.OrSet(ref _instance, FindObjectOfType<BulletManagerParticleSystem>);
        
        public ParticleSystem system;
        private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[8000];
        // private Dictionary<Bullet, int> bulletParticlePair = new Dictionary<Bullet, int>();
        private List<int> blockedIndices = new List<int>();
        public override void ImplementAddBullet(Bullet bullet, Weapon weapon)
        {
            ParticleSystem.Particle particle = new ParticleSystem.Particle();
            particle.position = bullet.startPosition;
            particle.startLifetime = 100;
            particle.startSize3D = Vector3.one * weapon.bulletSettings.bulletSize.GetRandom();

            ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
            emit.position = bullet.startPosition;
            Instance.system.Emit(emit,1);
            int index = 0;
            while (Instance.blockedIndices.Contains(index)) index++;
            Instance.blockedIndices.Add(index);
            Instance.particles[index] = particle;

            void ParticleUpdate(Vector3 position) => Instance.particles[index].position = position;
            void ParticleDestroy()
            {
                Instance.particles[index].startLifetime = 0;
                Instance.blockedIndices.Remove(index);
            }

            bullet.onDestroy += ParticleDestroy;
            if (!Instance.bullets.ContainsKey(weapon.trajectory))
                Instance.bullets.Add(weapon.trajectory, new List<(Bullet, Action<Vector3>)>());
            Instance.bullets[weapon.trajectory].Add((bullet, ParticleUpdate));
        }

        protected override void Update()
        {
            base.Update();
            system.SetParticles(particles);
        }
    }
}