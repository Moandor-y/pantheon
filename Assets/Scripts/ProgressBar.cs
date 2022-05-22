using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pantheon {
  class ProgressBar : MonoBehaviour {
    [SerializeField]
    private RectTransform _innerBar;

    [SerializeField]
    private RectTransform _outerBar;

    public void SetProgress(float value) {
      _innerBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                          _outerBar.rect.width * value);
    }
  }
}
