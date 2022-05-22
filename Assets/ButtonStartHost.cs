using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

namespace Pantheon {
  public class ButtonStartHost : MonoBehaviour {
    public TMP_InputField mechanicPath;

    void Start() {
      mechanicPath.text = PlayerPrefs.GetString("mechanic_path", string.Empty);
    }

    public void OnClick() {
      NetworkManager.Singleton.StartHost();
      NetworkManager.Singleton.SceneManager.LoadScene("Scenes/Arena", LoadSceneMode.Single);
      GlobalContext.Instance.MechanicPath = mechanicPath.text;
      PlayerPrefs.SetString("mechanic_path", GlobalContext.Instance.MechanicPath);
    }
  }
}
