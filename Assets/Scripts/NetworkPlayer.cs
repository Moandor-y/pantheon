using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace Pantheon
{
    [DisallowMultipleComponent]
    public class NetworkPlayer : NetworkBehaviour
    {
        public int Health {
            get {
                return _health.Value;
            }

            set {
                _health.Value = value;
            }
        }

        public int MaxHealth {
            get {
                return _maxHealth.Value;
            }

            set {
                _maxHealth.Value = value;
            }
        }

        public event Action<int> OnMaxHealthUpdated;
        public event Action<int> OnHealthUpdated;
        public event Action<int> OnManaUpdated;
        public event Action<string> OnNameUpdated;

        private NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString32Bytes>> _name = new NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString32Bytes>>();
        private NetworkVariable<int> _maxHealth = new NetworkVariable<int>();
        private NetworkVariable<int> _health = new NetworkVariable<int>();
        private NetworkVariable<int> _mana = new NetworkVariable<int>();

        [SerializeField]
        private PlayerMovementController _playerMovementController;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                _playerMovementController.enabled = true;
            }

            _maxHealth.OnValueChanged += OnMaxHealthChanged;
            _health.OnValueChanged += OnHealthChanged;
            _mana.OnValueChanged += OnManaChanged;
            _name.OnValueChanged += OnNameChanged;

            GlobalContext.Instance.RegisterPlayer(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            GlobalContext.Instance.UnregisterPlayer(this);
        }

        private void OnMaxHealthChanged(int previous, int current) {
            OnMaxHealthUpdated?.Invoke(current);
        }

        private void OnHealthChanged(int previous, int current) {
            OnHealthUpdated?.Invoke(current);
        }

        private void OnManaChanged(int previous, int current) {
            OnManaUpdated?.Invoke(current);
        }

        private void OnNameChanged(ForceNetworkSerializeByMemcpy<FixedString32Bytes> previous, ForceNetworkSerializeByMemcpy<FixedString32Bytes> current) {
            OnNameUpdated?.Invoke(current.ToString());
        }
    }
}
