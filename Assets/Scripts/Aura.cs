using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Assertions;

namespace Pantheon {

  public class Aura : NetworkBehaviour {
    public double ExpiresAt {
      set { _expiresAt.Value = value; }
    }

    private NetworkPlayer _player;
    private AuraListItem _auraListItem;

    [SerializeField]
    private GameObject _auraListItemPrefab;

    private NetworkVariable<double> _expiresAt = new NetworkVariable<double>();

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
      if (_auraListItemPrefab != null) {
        Destroy(_auraListItemPrefab.gameObject);
      }
    }

    private void OnExpiresAtChanged(double previous, double current) {
      if (_auraListItem != null) {
        _auraListItem.RemainingTime = (float)(current - NetworkManager.ServerTime.Time);
      }
    }
  }

}
