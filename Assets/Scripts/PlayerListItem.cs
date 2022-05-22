using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pantheon
{
    public class PlayerListItem : MonoBehaviour {
        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private ProgressBar _healthBar;

        [SerializeField]
        private ProgressBar _manaBar;

        [SerializeField]
        private TMP_Text _healthNumber;

        [SerializeField]
        private TMP_Text _manaNumber;

        private NetworkPlayer _player;

        public void SetPlayer(NetworkPlayer player) {
            _player = player;
            _player.OnNameUpdated += OnNameUpdated;
            _player.OnMaxHealthUpdated += OnMaxHealthUpdated;
            _player.OnHealthUpdated += OnHealthUpdated;
        }

        private void OnNameUpdated(string name) {
            _name.text = name;
        }

        private void OnMaxHealthUpdated(int maxHealth) {
            _healthBar.SetProgress(((float) _player.Health) / maxHealth);
        }

        private void OnHealthUpdated(int health) {
            _healthNumber.text = health.ToString();
            _healthBar.SetProgress(((float) health) / _player.MaxHealth);
        }
    }
}
