using System;
using System.Threading.Tasks;
using UnityEngine;

namespace elZach.BulletEngine
{
    public class Bullet
    {
        public Vector3 startPosition;
        public Vector3 startDirection;
        public Vector3 currentPosition;
        public float dynamicStartTime;
        public float physicsStartTime;
        
        public IBulletSource source;    //this can probably be moved to BulletManger / Or have it's dependencies resolved by the source itself
        public object parameters;
        
        public event Action<RaycastHit> onHit;  //this also probably doesn't need to be managed by the bullet
        public event Action onDestroy;          //if we remove these events we can make bullets structs

        public Bullet(IBulletSource source, Vector3 startPosition, Vector3 startDirection, object parameters, Action<RaycastHit> onHit, Action onDestroy)
        {
            this.source = source;
            this.startPosition = startPosition;
            this.startDirection = startDirection;
            currentPosition = startPosition;
            dynamicStartTime = Time.time;
            physicsStartTime = Time.fixedTime;
            this.parameters = parameters;
            this.onHit = onHit;
            this.onDestroy = onDestroy;
        }

        public void Hit(RaycastHit hit)
        {
            onHit?.Invoke(hit);
        }
        
        public async void Destroy()
        {
            await Task.Yield();
            onDestroy?.Invoke();
        }
    }
}