using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NewTestScript {
  [OneTimeSetUp]
  public void SetUp() {
    SceneManager.LoadScene("SampleScene");
  }

  [UnityTest]
  public IEnumerator NewTestScriptWithEnumeratorPasses() {
    NetworkManager.Singleton.StartHost();
    NetworkManager.Singleton.SceneManager.LoadScene("Scenes/Arena", LoadSceneMode.Single);
    Assert.AreEqual(Pantheon.GlobalContext.Instance.Players.Count, 1);
    yield return null;
  }
}
