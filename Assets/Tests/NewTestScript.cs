using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.IO;
using Newtonsoft.Json;

namespace Pantheon {

  public class NewTestScript {
    [SetUp]
    public void SetUp() {
      Time.captureDeltaTime = 1.0f / 10;
      SceneManager.LoadScene("SampleScene");
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses() {
      File.WriteAllText("Mechanics/Tests/DelayedProteanHit.json",
                        JsonConvert.SerializeObject(
                            Test.Mechanics.DelayedProteanHit.GetMechanicData(), Formatting.Indented,
                            new JsonSerializerSettings() {
                              SerializationBinder = new XivSimParser.TypeBinder(),
                              TypeNameHandling = TypeNameHandling.Auto,
                              DefaultValueHandling = DefaultValueHandling.Ignore,
                            }));
      GlobalContext.Instance.MechanicPath = "Mechanics/Tests/DelayedProteanHit.json";
      NetworkManager.Singleton.StartHost();
      NetworkManager.Singleton.SceneManager.LoadScene("Scenes/Arena", LoadSceneMode.Single);
      bool finished = false;
      NetworkManager.Singleton.SceneManager.OnLoadEventCompleted +=
          (string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
           List<ulong> clientsTimedOut) => { finished = true; };
      while (!finished) {
        yield return null;
      }
      Object.FindObjectOfType<MechanicManager>().StartMechanic();
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, 50000);
      yield return new WaitForSeconds(Test.Mechanics.DelayedProteanHit.Duration);
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, -50000);
    }
  }

}
