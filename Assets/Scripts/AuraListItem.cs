using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pantheon {

  public class AuraListItem : MonoBehaviour {
    public float RemainingTime {
      set { _remainingTime = value; }
    }

    [SerializeField]
    private TMP_Text _countdown;

    private float _remainingTime = -1;

    private void Update() {
      _remainingTime -= Time.deltaTime;
      if (_remainingTime > 0) {
        _countdown.text = Mathf.RoundToInt(_remainingTime).ToString();
      } else {
        _countdown.text = string.Empty;
      }
    }
  }

}
