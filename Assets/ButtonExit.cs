using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ButtonExit : MonoBehaviour {
  public void OnClick() {
    NetworkManager.Singleton.Shutdown();
    SceneManager.LoadSceneAsync("Scenes/SampleScene");
  }
}
