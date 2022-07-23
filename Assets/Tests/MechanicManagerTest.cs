using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Pantheon.Test {

  public class MechanicManagerTest {
    private NetworkPlayer _localPlayer;
    private AutoPilot _localAutoPilot;

    [UnitySetUp]
    public IEnumerator SetUp() {
      Time.captureDeltaTime = 1.0f / 10;

      SceneManager.LoadScene("SampleScene");

      yield return null;

      NetworkManager.Singleton.StartHost();

      NetworkManager.Singleton.SceneManager.LoadScene("Scenes/Arena", LoadSceneMode.Single);
      bool finished = false;
      NetworkManager.Singleton.SceneManager.OnLoadEventCompleted +=
          (string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
           List<ulong> clientsTimedOut) => { finished = true; };
      while (!finished) {
        yield return null;
      }

      _localPlayer = GlobalContext.Instance.LocalPlayer;
      _localAutoPilot = _localPlayer.gameObject.AddComponent<AutoPilot>();
    }

    [UnityTearDown]
    public IEnumerator TearDown() {
      NetworkManager.Singleton.Shutdown();
      yield break;
    }

    [UnityTest]
    public IEnumerator DelayedProteanHit() {
      StartMechanic("DelayedProteanHit");
      Assert.AreEqual(_localPlayer.Health, 50000);
      _localAutoPilot.Go(new Vector2(0, -3));
      yield return new WaitForSeconds(Test.Mechanics.DelayedProteanHit.Duration);
      Assert.AreEqual(_localPlayer.Health, -50000);
    }

    [UnityTest]
    public IEnumerator DelayedProteanMiss() {
      StartMechanic("DelayedProteanMiss");
      Assert.AreEqual(_localPlayer.Health, 50000);
      _localAutoPilot.Go(new Vector2(0, -3));
      yield return new WaitForSeconds(1);
      _localAutoPilot.Go(new Vector2(3, 0));
      yield return new WaitForSeconds(2);
      Assert.AreEqual(_localPlayer.Health, 50000);
    }

    [UnityTest]
    public IEnumerator TargetTanks() {
      _localPlayer.PlayerClass = NetworkPlayer.Class.Healer;
      for (int i = 0; i < 7; ++i) {
        float rad = Mathf.Deg2Rad * 45 * i;
        GlobalContext.Instance.SpawnAiPlayer().GetComponent<AutoPilot>().Go(
            new Vector2(5 * Mathf.Cos(rad), 5 * Mathf.Sin(rad)));
      }
      yield return null;
      StartMechanic("TargetTanks");
      yield return new WaitForSeconds(2);
      Assert.AreEqual(GlobalContext.Instance.Players.Count(player => player.Health == 49999), 7);
      Assert.AreEqual(_localPlayer.Health, 50000);
    }

    [UnityTest]
    public IEnumerator TargetTanksNotEnoughTanks() {
      _localPlayer.PlayerClass = NetworkPlayer.Class.Healer;
      for (int i = 0; i < 6; ++i) {
        float rad = Mathf.Deg2Rad * 45 * i;
        GlobalContext.Instance.SpawnAiPlayer().GetComponent<AutoPilot>().Go(
            new Vector2(5 * Mathf.Cos(rad), 5 * Mathf.Sin(rad)));
      }
      yield return null;
      StartMechanic("TargetTanks");
      yield return new WaitForSeconds(2);
      Assert.AreEqual(7, GlobalContext.Instance.Players.Count(player => player.Health == 49999));
    }

    [UnityTest]
    public IEnumerator TargetTanksNotEnoughPlayers() {
      _localPlayer.PlayerClass = NetworkPlayer.Class.Tank;
      for (int i = 0; i < 5; ++i) {
        float rad = Mathf.Deg2Rad * 45 * i;
        GlobalContext.Instance.SpawnAiPlayer().GetComponent<AutoPilot>().Go(
            new Vector2(5 * Mathf.Cos(rad), 5 * Mathf.Sin(rad)));
      }
      yield return null;
      StartMechanic("TargetTanks");
      yield return new WaitForSeconds(2);
      Assert.AreEqual(5, GlobalContext.Instance.Players.Count(player => player.Health == 49999));
      Assert.AreEqual(1, GlobalContext.Instance.Players.Count(player => player.Health == -50001));
    }

    [UnityTest]
    public IEnumerator EnemyDirectChildMechanicsFollowMovement() {
      _localPlayer.PlayerClass = NetworkPlayer.Class.Healer;
      var tank = GlobalContext.Instance.SpawnAiPlayer();
      tank.PlayerClass = NetworkPlayer.Class.Tank;
      var tankAutoPilot = tank.GetComponent<AutoPilot>();
      yield return null;
      StartMechanic("EnemyDirectChildMechanicsFollowMovement");
      tankAutoPilot.Go(new Vector2(0, 20));
      yield return new WaitForSeconds(5);
      tankAutoPilot.Go(new Vector2(20, 20));
      yield return new WaitForSeconds(7);
      Assert.AreEqual(50000, _localPlayer.Health);
      Assert.AreEqual(49999, tank.Health);
    }

    [UnityTest]
    public IEnumerator EnemyIndirectChildMechanicsDoNotFollowMovement() {
      _localPlayer.PlayerClass = NetworkPlayer.Class.Healer;
      var tank = GlobalContext.Instance.SpawnAiPlayer();
      tank.PlayerClass = NetworkPlayer.Class.Tank;
      var tankAutoPilot = tank.GetComponent<AutoPilot>();
      yield return null;
      StartMechanic("EnemyIndirectChildMechanicsDoNotFollowMovement");
      tankAutoPilot.Go(new Vector2(0, 20));
      yield return new WaitForSeconds(5);
      tankAutoPilot.Go(new Vector2(20, 20));
      yield return new WaitForSeconds(7);
      Assert.AreEqual(49998, _localPlayer.Health);
      Assert.AreEqual(50000, tank.Health);
    }

    [UnityTest]
    public IEnumerator EnemyDoesNotMoveWhileCasting() {
      StartMechanic("EnemyDoesNotMoveWhileCasting");
      _localAutoPilot.Go(new Vector2(0, -20));
      yield return new WaitForSeconds(7);
      Assert.AreEqual(50000, _localPlayer.Health);
    }

    [UnityTest]
    public IEnumerator MovesEnemy() {
      StartMechanic("MovesEnemy");
      _localAutoPilot.Go(new Vector2(0, -20));
      yield return new WaitForSeconds(7);
      Assert.AreEqual(49999, _localPlayer.Health);
    }

    private void StartMechanic(string name) {
      File.WriteAllText(
          $"Mechanics/Tests/{name}.json",
          JsonConvert.SerializeObject(Type.GetType($"Pantheon.Test.Mechanics.{name}")
                                          .GetMethod("GetMechanicData")
                                          .Invoke(null, null),
                                      Formatting.Indented,
                                      new JsonSerializerSettings() {
                                        SerializationBinder = new XivSimParser.TypeBinder(),
                                        TypeNameHandling = TypeNameHandling.Auto,
                                        DefaultValueHandling = DefaultValueHandling.Ignore,
                                      }));
      GlobalContext.Instance.MechanicPath = $"Mechanics/Tests/{name}.json";
      UnityEngine.Object.FindObjectOfType<MechanicManager>().StartMechanic();
    }
  }

}
