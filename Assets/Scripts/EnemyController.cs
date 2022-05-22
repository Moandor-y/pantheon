using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.IO;
using System;

namespace Pantheon
{
    public class EnemyController : NetworkBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        private EnemyListItem _enemyListItem;

        private Coroutine _castCoroutine;

        private int _id;

        public override void OnDestroy() {
            if (_enemyListItem != null) {
                Destroy(_enemyListItem.gameObject);
            }

            GlobalContext.Instance.UnregisterEnemy(_id);

            base.OnDestroy();
        }

        public void Cast(string name, float duration) {
            if (!IsServer) {
                throw new InvalidOperationException();
            }
            CastClientRpc(name, duration);
        }

        [ServerRpc]
        public void SetTexturePathServerRpc(string path)
        {
            SetTexturePathClientRpc(path);
        }

        [ServerRpc]
        public void SetNameServerRpc(string name) {
            SetNameClientRpc(name);
        }

        [ServerRpc]
        public void SetIdServerRpc(int id) {
            SetIdClientRpc(id);
        }

        [ClientRpc]
        private void SetTexturePathClientRpc(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture.LoadImage(File.ReadAllBytes(path));
            _renderer.material.mainTexture = texture;
        }

        [ClientRpc]
        private void SetNameClientRpc(string name) {
            if (_enemyListItem == null) {
                _enemyListItem = EnemyList.Instance.AddEnemy();
            }
            _enemyListItem.SetName(name);
        }

        [ClientRpc]
        private void SetIdClientRpc(int id) {
            _id = id;
            GlobalContext.Instance.RegisterEnemy(id, this);
        }

        [ClientRpc]
        private void CastClientRpc(string name, float duration) {
            if (_castCoroutine != null) {
                StopCoroutine(_castCoroutine);
            }

            _castCoroutine = StartCoroutine(_enemyListItem.Cast(name, duration));
        }
    }
}
