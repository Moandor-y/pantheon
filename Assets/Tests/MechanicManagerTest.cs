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

  public class MechanicManagerTest {
    [SetUp]
    public void SetUp() {
      Time.captureDeltaTime = 1.0f / 10;
      SceneManager.LoadScene("SampleScene");
    }

    [TearDown]
    public void TearDown() {
      NetworkManager.Singleton.Shutdown();
    }

    [UnityTest]
    public IEnumerator DelayedProteanHit() {
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
      var autoPilot = GlobalContext.Instance.Players[0].gameObject.AddComponent<AutoPilot>();
      Object.FindObjectOfType<MechanicManager>().StartMechanic();
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, 50000);
      autoPilot.Go(new Vector2(0, -3));
      yield return new WaitForSeconds(Test.Mechanics.DelayedProteanHit.Duration);
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, -50000);
    }

    [UnityTest]
    public IEnumerator DelayedProteanMiss() {
      File.WriteAllText("Mechanics/Tests/DelayedProteanMiss.json",
                        JsonConvert.SerializeObject(
                            Test.Mechanics.DelayedProteanHit.GetMechanicData(), Formatting.Indented,
                            new JsonSerializerSettings() {
                              SerializationBinder = new XivSimParser.TypeBinder(),
                              TypeNameHandling = TypeNameHandling.Auto,
                              DefaultValueHandling = DefaultValueHandling.Ignore,
                            }));
      GlobalContext.Instance.MechanicPath = "Mechanics/Tests/DelayedProteanMiss.json";
      NetworkManager.Singleton.StartHost();
      NetworkManager.Singleton.SceneManager.LoadScene("Scenes/Arena", LoadSceneMode.Single);
      bool finished = false;
      NetworkManager.Singleton.SceneManager.OnLoadEventCompleted +=
          (string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
           List<ulong> clientsTimedOut) => { finished = true; };
      while (!finished) {
        yield return null;
      }
      var autoPilot = GlobalContext.Instance.Players[0].gameObject.AddComponent<AutoPilot>();
      Object.FindObjectOfType<MechanicManager>().StartMechanic();
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, 50000);
      autoPilot.Go(new Vector2(0, -3));
      yield return new WaitForSeconds(1);
      autoPilot.Go(new Vector2(3, 0));
      yield return new WaitForSeconds(2);
      Assert.AreEqual(GlobalContext.Instance.Players[0].Health, 50000);
    }
  }

}
