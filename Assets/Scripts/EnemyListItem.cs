using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pantheon
{
    class EnemyListItem : MonoBehaviour {
        [SerializeField]
        private TMP_Text _enemyName;

        [SerializeField]
        private TMP_Text _castName;

        [SerializeField]
        private ProgressBar _healthBar;

        [SerializeField]
        private ProgressBar _castBar;

        private void Start() {
            _castName.gameObject.SetActive(false);
            _castBar.gameObject.SetActive(false);
        }

        public void SetName(string name) {
            _enemyName.text = name;
        }

        public IEnumerator Cast(string name, float duration) {
            _castName.gameObject.SetActive(true);
            _castBar.gameObject.SetActive(true);
            _castName.text = name;
            float start = Time.time;
            while (Time.time < start + duration) {
                _castBar.SetProgress((Time.time - start) / duration);
                yield return null;
            }
            _castName.gameObject.SetActive(false);
            _castBar.gameObject.SetActive(false);
        }
    }
}
