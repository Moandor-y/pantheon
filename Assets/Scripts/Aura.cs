using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

namespace Pantheon {

  public class Aura : NetworkBehaviour {
    public abstract class Effect {
      public virtual float ApplyDamage(float rawDamage, string damageType) {
        return rawDamage;
      }
    }

    public class DamageModifierEffect : Effect {
      private string _damageType;
      private float _multiplier;

      public DamageModifierEffect(string damageType, float multiplier) {
        _damageType = damageType;
        _multiplier = multiplier;
      }

      public override float ApplyDamage(float rawDamage, string damageType) {
        if (damageType == _damageType) {
          rawDamage *= _multiplier;
        }
        return rawDamage;
      }
    }

    public double ExpiresAt {
      set { _expiresAt.Value = value; }
    }

    public ReadOnlyCollection<Effect> Effects => _effects.AsReadOnly();

    private NetworkPlayer _player;
    private AuraListItem _auraListItem;

    [SerializeField]
    private GameObject _auraListItemPrefab;

    private NetworkVariable<double> _expiresAt = new NetworkVariable<double>();

    private Coroutine _destroyTimer;

    private List<Effect> _effects = new List<Effect>();

    public override void OnNetworkSpawn() {
      base.OnNetworkDespawn();

      _expiresAt.OnValueChanged += OnExpiresAtChanged;
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {
      base.OnNetworkObjectParentChanged(parentNetworkObject);

      Assert.IsNull(_player);
      _player = parentNetworkObject.GetComponent<NetworkPlayer>();
      Assert.IsNotNull(_player);

      _player.AddAura(this);

      if (_player.IsLocalPlayer) {
        _auraListItem = Instantiate(_auraListItemPrefab, DebuffList.Instance.gameObject.transform)
                            .GetComponent<AuraListItem>();
        _auraListItem.RemainingTime = (float)(_expiresAt.Value - NetworkManager.ServerTime.Time);
      }
    }

    public override void OnNetworkDespawn() {
      base.OnNetworkDespawn();

      if (_player != null) {
        _player.RemoveAura(this);
      }

      if (_auraListItem != null) {
        Destroy(_auraListItem.gameObject);
      }

      if (_destroyTimer != null) {
        StopCoroutine(_destroyTimer);
        _destroyTimer = null;
      }
    }

    public void AddEffect(Effect effect) {
      _effects.Add(effect);
    }

    private void OnExpiresAtChanged(double previous, double current) {
      if (_auraListItem != null) {
        _auraListItem.RemainingTime = (float)(current - NetworkManager.ServerTime.Time);
      }

      if (IsServer) {
        if (_destroyTimer != null) {
          StopCoroutine(_destroyTimer);
        }
        _destroyTimer = StartCoroutine(DestroyIn(current - NetworkManager.ServerTime.Time));
      }
    }

    private IEnumerator DestroyIn(double seconds) {
      yield return new WaitForSeconds((float)seconds);
      GetComponent<NetworkObject>().Despawn();
      _destroyTimer = null;
    }
  }

}
