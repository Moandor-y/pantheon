using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public class PlayerList : MonoBehaviour {
        [SerializeField]
        private GameObject _playerListItemPrefab;

        private List<Tuple<NetworkPlayer, PlayerListItem>> _players = new List<Tuple<NetworkPlayer, PlayerListItem>>();

        private void Start() {
            foreach (var player in GlobalContext.Instance.Players) {
                OnPlayerAdded(player);
            }
        }

        private void OnEnable() {
            GlobalContext.Instance.OnPlayerAdded += OnPlayerAdded;
            GlobalContext.Instance.OnPlayerRemoved += OnPlayerRemoved;
        }

        private void OnDisable() {
            GlobalContext.Instance.OnPlayerAdded -= OnPlayerAdded;
            GlobalContext.Instance.OnPlayerRemoved -= OnPlayerRemoved;
        }

        private void OnPlayerAdded(NetworkPlayer player) {
            if (_players.Find((Tuple<NetworkPlayer, PlayerListItem> x) => x.Item1 == player) != null) {
                return;
            }
            PlayerListItem item = Instantiate(_playerListItemPrefab, transform).GetComponent<PlayerListItem>();
            item.SetPlayer(player);
            _players.Add(Tuple.Create(player, item));
        }

        private void OnPlayerRemoved(NetworkPlayer player) {
            var item = _players.Find((Tuple<NetworkPlayer, PlayerListItem> x) => x.Item1 == player);
            Destroy(item.Item2.gameObject);
            _players.Remove(item);
        }
    }
}
