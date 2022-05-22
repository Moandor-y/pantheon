using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pantheon
{
    class EnemyList : Singleton<EnemyList> {
        [SerializeField]
        private GameObject _enemyListItemPrefab;

        public EnemyListItem AddEnemy() {
            return Instantiate(_enemyListItemPrefab, transform).GetComponent<EnemyListItem>();
        }
    }
}
