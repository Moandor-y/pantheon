using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.IO;
using System;
using UnityEngine.Assertions;

namespace Pantheon {
  public class EnemyController : NetworkBehaviour {
    public NetworkPlayer Aggro { set; private get; }

    public float HitboxSize { set; private get; }

    public float Speed {
      set { _autoPilot.Speed = value; }
    }

    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private Renderer _rendererBack;

    [SerializeField]
    private AutoPilot _autoPilot;

    private EnemyListItem _enemyListItem;

    private Coroutine _castClientCoroutine;
    private Coroutine _castServerCoroutine;

    private int _id;
    private bool _isCasting;

    public override void OnDestroy() {
      if (_enemyListItem != null) {
        Destroy(_enemyListItem.gameObject);
      }

      GlobalContext.Instance.UnregisterEnemy(_id);

      base.OnDestroy();
    }

    public void Cast(string name, float duration) {
      Assert.IsTrue(IsServer);

      if (_castServerCoroutine != null) {
        StopCoroutine(_castServerCoroutine);
      }
      _isCasting = true;
      _castServerCoroutine = StartCoroutine(StopCastInSeconds(duration));

      CastClientRpc(name, duration);
    }

    [ServerRpc]
    public void SetTexturePathServerRpc(string path) {
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
    private void SetTexturePathClientRpc(string path) {
      Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
      texture.LoadImage(File.ReadAllBytes(path));
      _renderer.material.mainTexture = texture;
      _rendererBack.material.mainTexture = texture;
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
      if (_castClientCoroutine != null) {
        StopCoroutine(_castClientCoroutine);
      }

      _castClientCoroutine = StartCoroutine(_enemyListItem.Cast(name, duration));
    }

    private void Update() {
      if (Aggro != null && !_isCasting) {
        _autoPilot.TargetDistance = HitboxSize;
        _autoPilot.Go(new Vector2(Aggro.transform.position.x, Aggro.transform.position.z));
        Vector3 look = Aggro.transform.position - transform.position;
        _autoPilot.Face(new Vector2(look.x, look.z));
      }
    }

    private IEnumerator StopCastInSeconds(float seconds) {
      yield return new WaitForSeconds(seconds);
      _isCasting = false;
    }
  }
}
