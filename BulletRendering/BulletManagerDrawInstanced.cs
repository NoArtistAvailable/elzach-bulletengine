using System;
using System.Collections.Generic;
using System.Linq;
using elZach.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace elZach.BulletEngine
{
    public class BulletManagerDrawInstanced : BulletManager
    {
        private static BulletManagerDrawInstanced _instance;
        public new static BulletManagerDrawInstanced Instance => _instance.OrSet(ref _instance, FindObjectOfType<BulletManagerDrawInstanced>);
        
        
        private List<MatrixHandler> renderMatrices = new List<MatrixHandler>();

        class MatrixHandler
        {
            public Mesh mesh;
            public Material material;
            public Matrix4x4[] matrices = new Matrix4x4[1023];

            public int count = 0;
            public MatrixHandler(Mesh mesh, Material mat)
            {
                this.mesh = mesh;
                this.material = mat;
                matrices = new Matrix4x4[1023];
                count = 0;
            }

            private int currentRenderCount = 0;
            public void ResetCurrentRenderCount() => currentRenderCount = 0;

            public void AddCurrent(Vector3 position, Vector3 scale)
            {
                if (currentRenderCount >= matrices.Length)
                {
                    Debug.LogWarning($"wtf how? {currentRenderCount} / {matrices.Length}; count: {count}");
                    return;
                }
                matrices[currentRenderCount] = Matrix4x4.TRS(position, Quaternion.identity, scale);
                currentRenderCount++;
            }

            public void Draw()
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count);
            }
        }

        public override void ImplementAddBullet(Bullet bullet, Weapon weapon)
        {
            var scale = weapon.bulletSettings.bulletSize.GetRandom() * Vector3.one;

            var mesh = weapon.bulletSettings.mesh;
            var mat = weapon.bulletSettings.mat;
            
            if (!Instance.renderMatrices.Any(x=>x.mesh == mesh && x.material == mat && x.count < 1022))
                Instance.renderMatrices.Add(new MatrixHandler(mesh, mat));
            
            var renderMatrix = Instance.renderMatrices.FirstOrDefault(x => x.mesh == mesh && x.material == mat && x.count < 1022);
            
            renderMatrix.count++;
            
            void Matrix4x4Update(Vector3 position)
            {
                renderMatrix.AddCurrent(position, scale);
            }

            void Destroy()
            {
                renderMatrix.count--;
            }
            bullet.onDestroy += Destroy;

            if (!Instance.bullets.ContainsKey(weapon.trajectory))
                Instance.bullets.Add(weapon.trajectory, new List<(Bullet, Action<Vector3>)>());
            Instance.bullets[weapon.trajectory].Add((bullet, Matrix4x4Update));
        }

        protected override void Update()
        {
            foreach(var matr in renderMatrices) matr.ResetCurrentRenderCount();
            base.Update();
            foreach(var matr in renderMatrices) matr.Draw();
        }
    }
}