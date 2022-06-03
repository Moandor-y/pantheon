using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.Netcode;

namespace Pantheon {
  public class MechanicManager : NetworkBehaviour {
    [SerializeField]
    private GameObject _visualObjectPrefab;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _aoeMarkerPrefab;

    private XivSimParser.MechanicData _mechanicData;

    private int _nextEnemyId = 1;
    private MersenneTwister _random;
    private List<NetworkPlayer> _players = new List<NetworkPlayer>();
    private Dictionary<Guid, AoeMarker> _aoeMarkers = new Dictionary<Guid, AoeMarker>();

    private List<Coroutine> _coroutines = new List<Coroutine>();

    public void StartMechanic() {
      StartMechanicServerRpc();
    }

    [ServerRpc]
    private void StartMechanicServerRpc() {
      if (_random != null) {
        throw new InvalidOperationException();
      }
      string json = File.ReadAllText(GlobalContext.Instance.MechanicPath);
      _random = new MersenneTwister(MersenneTwister.NewSeed());
      StartCoroutine(Execute(XivSimParser.Parse(json)));
    }

    private IEnumerator Execute(XivSimParser.MechanicData mechanicData) {
      if (_mechanicData != null || _players.Count != 0 || _aoeMarkers.Count != 0) {
        throw new InvalidOperationException();
      }
      _mechanicData = mechanicData;
      _players.AddRange(GlobalContext.Instance.Players);

      foreach (NetworkPlayer player in _players) {
        player.MaxHealth = Mathf.RoundToInt(_mechanicData.defaultHealth);
        player.Health = player.MaxHealth;
      }

      foreach (XivSimParser.MechanicEvent mechanicEvent in _mechanicData.mechanicEvents) {
        yield return Execute(mechanicEvent, new MechanicContext());
      }

      while (_coroutines.Count > 0) {
        Coroutine coroutine = _coroutines[_coroutines.Count - 1];
        _coroutines.RemoveAt(_coroutines.Count - 1);
        yield return coroutine;
      }
      _mechanicData = null;
      _random = null;
      _players.Clear();
      _aoeMarkers.Clear();
    }

    private IEnumerator Execute(XivSimParser.MechanicEvent mechanicEvent,
                                MechanicContext mechanicContext) {
      yield return Execute((dynamic)mechanicEvent, mechanicContext);
    }

    private IEnumerator Execute(XivSimParser.SpawnVisualObject spawnVisualObject,
                                MechanicContext mechanicContext) {
      SpawnVisualObjectClientRpc(visible: mechanicContext.Visible,
                                 texturePath: spawnVisualObject.textureFilePath,
                                 position: spawnVisualObject.relativePosition,
                                 rotation: Quaternion.Euler(spawnVisualObject.eulerAngles),
                                 scale: spawnVisualObject.scale);
      yield break;
    }

    private IEnumerator Execute(XivSimParser.SpawnMechanicEvent spawnMechanicEvent,
                                MechanicContext mechanicContext) {
      MechanicContext childContext = InheritContext(mechanicContext);
      childContext.Position += spawnMechanicEvent.position;
      _coroutines.Add(StartCoroutine(Execute(
          _mechanicData.referenceMechanicProperties[spawnMechanicEvent.referenceMechanicName],
          childContext)));
      yield break;
    }

    private IEnumerator Execute(XivSimParser.MechanicProperties mechanicProperties,
                                MechanicContext mechanicContext) {
      float duration = GetDuration(mechanicProperties.mechanic);
      Guid aoeMarkerId = Guid.NewGuid();

      bool visible = mechanicProperties.visible.GetValueOrDefault();

      float radius = 0;
      float angle = 0;
      float innerRadius = 0;
      if (mechanicProperties.collisionShape == XivSimParser.CollisionShape.Round) {
        Vector4 shapeParams = mechanicProperties.collisionShapeParams.GetValueOrDefault();
        radius = shapeParams.x;
        angle = shapeParams.y;
        innerRadius = shapeParams.z;
      }
      SpawnAoeMarkerClientRpc(
          new Vector3(mechanicContext.Position.x, 0, mechanicContext.Position.y), duration, radius,
          innerRadius, angle, visible, aoeMarkerId.ToByteArray());

      MechanicContext childContext = InheritContext(mechanicContext);
      childContext.Visible = visible;
      childContext.IsTargeted = mechanicProperties.isTargeted.GetValueOrDefault();
      UpdateCollision(childContext, mechanicProperties);

      _coroutines.Add(StartCoroutine(Execute(mechanicProperties.mechanic, childContext)));

      float endTime = Time.time + duration;
      while (Time.time < endTime) {
        if (childContext.IsTargeted) {
          Vector2 targetPosition = new Vector2(childContext.Target.transform.position.x,
                                               childContext.Target.transform.position.z);
          Vector2 diff = targetPosition - childContext.Position;

          float movement = Mathf.Min(
              diff.magnitude, mechanicProperties.followSpeed.GetValueOrDefault() * Time.deltaTime);
          childContext.Position += diff.normalized * movement;

          if (diff == Vector2.zero) {
            childContext.Forward = Vector2.up;
          } else {
            childContext.Forward = diff.normalized;
          }
        }

        UpdateAoeMarkerClientRpc(
            new Vector3(childContext.Position.x, 0, childContext.Position.y),
            Quaternion.LookRotation(new Vector3(childContext.Forward.x, 0, childContext.Forward.y)),
            childContext.Visible, aoeMarkerId.ToByteArray());

        UpdateCollision(childContext, mechanicProperties);

        yield return null;
      }
    }

    private IEnumerator Execute(XivSimParser.ExecuteMultipleEvents executeMultipleEvents,
                                MechanicContext mechanicContext) {
      foreach (XivSimParser.MechanicEvent mechanicEvent in executeMultipleEvents.events) {
        yield return Execute(mechanicEvent, mechanicContext);
      }
    }

    private IEnumerator Execute(XivSimParser.ExecuteRandomEvents executeRandomEvents,
                                MechanicContext mechanicContext) {
      List<XivSimParser.MechanicEvent> pool =
          _mechanicData.mechanicPools[executeRandomEvents.mechanicPoolName];
      for (int i = 0; i < executeRandomEvents.numberToSpawn; ++i) {
        _coroutines.Add(
            StartCoroutine(Execute(pool[_random.Range(0, pool.Count)], mechanicContext)));
      }
      yield break;
    }

    private IEnumerator Execute(XivSimParser.SpawnEnemy spawnEnemy,
                                MechanicContext mechanicContext) {
      int enemyId = _nextEnemyId;
      ++_nextEnemyId;

      EnemyController enemy = Instantiate(_enemyPrefab).GetComponent<EnemyController>();
      enemy.GetComponent<NetworkObject>().Spawn(true);
      enemy.SetIdServerRpc(enemyId);
      enemy.transform.localScale = spawnEnemy.visualScale;
      enemy.SetTexturePathServerRpc(spawnEnemy.textureFilePath);
      enemy.SetNameServerRpc(spawnEnemy.enemyName);

      MechanicContext childContext = InheritContext(mechanicContext);
      childContext.Source = enemy;
      yield return Execute(
          _mechanicData.referenceMechanicProperties[spawnEnemy.referenceMechanicName],
          childContext);
    }

    private IEnumerator Execute(XivSimParser.StartCastBar startCastBar,
                                MechanicContext mechanicContext) {
      mechanicContext.Source.Cast(startCastBar.castName, startCastBar.duration);
      yield break;
    }

    private IEnumerator Execute(XivSimParser.WaitEvent waitEvent, MechanicContext mechanicContext) {
      yield return new WaitForSeconds(waitEvent.timeToWait);
    }

    private IEnumerator Execute(XivSimParser.ReshufflePlayerIds reshufflePlayerIds,
                                MechanicContext mechanicContext) {
      for (int i = _players.Count - 1; i > 1; --i) {
        int j = _random.UniformInt(0, i);
        NetworkPlayer temp = _players[i];
        _players[i] = _players[j];
        _players[j] = temp;
      }
      yield break;
    }

    private IEnumerator Execute(XivSimParser.SpawnTargetedEvents spawnTargetedEvents,
                                MechanicContext mechanicContext) {
      List<NetworkPlayer> targets = TargetPlayers(spawnTargetedEvents.targetingScheme);
      foreach (NetworkPlayer target in targets) {
        MechanicContext childContext = InheritContext(mechanicContext);
        childContext.Target = target;
        _coroutines.Add(StartCoroutine(Execute(
            _mechanicData.referenceMechanicProperties[spawnTargetedEvents.referenceMechanicName],
            childContext)));
      }
      yield break;
    }

    private IEnumerator Execute(XivSimParser.ApplyEffectToPlayers applyEffectToPlayers,
                                MechanicContext mechanicContext) {
      List<NetworkPlayer> hit = new List<NetworkPlayer>();
      foreach (NetworkPlayer player in _players) {
        if (mechanicContext.Collision.CollidesWith(
                new Vector2(player.transform.position.x, player.transform.position.z))) {
          hit.Add(player);
        }
      }
      ApplyEffect(applyEffectToPlayers.effect, hit);
      yield break;
    }

    private IEnumerator Execute(XivSimParser.ModifyMechanicEvent modifyMechanicEvent,
                                MechanicContext mechanicContext) {
      XivSimParser.MechanicProperties mechanicProperties =
          _mechanicData.referenceMechanicProperties[modifyMechanicEvent.referenceMechanicName];
      if (mechanicProperties.isTargeted.HasValue) {
        mechanicContext.IsTargeted = mechanicProperties.isTargeted.Value;
      }
      if (mechanicProperties.visible.HasValue) {
        mechanicContext.Visible = mechanicProperties.visible.Value;
      }
      yield break;
    }

    private void ApplyEffect(XivSimParser.MechanicEffect effect, List<NetworkPlayer> players) {
      if (effect is XivSimParser.DamageEffect) {
        var damageEffect = (XivSimParser.DamageEffect)effect;
        float damage =
            damageEffect.damageAmount / Mathf.Min(damageEffect.maxStackAmount, players.Count);
        foreach (NetworkPlayer player in players) {
          player.Health -= Mathf.RoundToInt(damage);
        }
      }
    }

    private List<NetworkPlayer> TargetPlayers(XivSimParser.TargetingScheme targetingScheme) {
      List<NetworkPlayer> result = new List<NetworkPlayer>();
      if (targetingScheme is XivSimParser.TargetSpecificPlayerIds) {
        var targetSpecificPlayerIds = (XivSimParser.TargetSpecificPlayerIds)targetingScheme;
        foreach (int id in targetSpecificPlayerIds.targetIds) {
          result.Add(_players[id % _players.Count]);
        }
      } else if (targetingScheme is XivSimParser.TargetAllPlayers) {
        result.AddRange(_players);
      } else {
        throw new NotImplementedException();
      }
      return result;
    }

    private float GetDuration(XivSimParser.MechanicEvent mechanicEvent) {
      if (mechanicEvent is XivSimParser.WaitEvent) {
        return ((XivSimParser.WaitEvent)mechanicEvent).timeToWait;
      }
      if (mechanicEvent is XivSimParser.ExecuteMultipleEvents) {
        float duration = 0;
        foreach (XivSimParser
                     .MechanicEvent subMechanicEvent in (
                         (XivSimParser.ExecuteMultipleEvents)mechanicEvent)
                     .events) {
          duration += GetDuration(subMechanicEvent);
        }
        return duration;
      }
      return 0;
    }

    [ClientRpc]
    private void SpawnVisualObjectClientRpc(bool visible, string texturePath, Vector3 position,
                                            Quaternion rotation, Vector3 scale) {
      GameObject spawned = Instantiate(_visualObjectPrefab);
      spawned.GetComponent<MeshRenderer>().enabled = visible;
      Texture2D texture = new Texture2D(2, 2);
      texture.LoadImage(File.ReadAllBytes(texturePath));
      spawned.GetComponent<MeshRenderer>().material.mainTexture = texture;
      spawned.transform.position = position;
      spawned.transform.rotation = rotation;
      spawned.transform.localScale = scale;
    }

    [ClientRpc]
    private void SpawnAoeMarkerClientRpc(Vector3 position, float duration, float radius,
                                         float innerRadius, float angle, bool visible, byte[] id) {
      AoeMarker aoeMarker = Instantiate(_aoeMarkerPrefab).GetComponent<AoeMarker>();
      aoeMarker.SetRound(radius, innerRadius, angle);
      aoeMarker.transform.position = position;
      aoeMarker.Duration = duration;
      aoeMarker.Visible = visible;
      _aoeMarkers[new Guid(id)] = aoeMarker;
    }

    [ClientRpc]
    private void UpdateAoeMarkerClientRpc(Vector3 position, Quaternion rotation, bool visible,
                                          byte[] id) {
      var aoeMarker = _aoeMarkers[new Guid(id)];
      aoeMarker.transform.position = position;
      aoeMarker.transform.rotation = rotation;
      aoeMarker.Visible = visible;
    }

    private void UpdateCollision(MechanicContext mechanicContext,
                                 XivSimParser.MechanicProperties mechanicProperties) {
      if (mechanicProperties.collisionShape == XivSimParser.CollisionShape.Round) {
        Vector4 shapeParams = mechanicProperties.collisionShapeParams.GetValueOrDefault();
        mechanicContext.Collision = new RoundCollision(
            mechanicContext.Position, direction: mechanicContext.Forward, radius: shapeParams.x,
            innerRadius: shapeParams.z, angle: shapeParams.y);
      }
    }

    private static MechanicContext InheritContext(MechanicContext parent) {
      return new MechanicContext() {
        Parent = parent,        Visible = false,
        Source = parent.Source, Position = new Vector2(0, 0),
        Collision = null,       Target = parent.Target,
        IsTargeted = false,     Forward = Vector2.up,
      };
    }

    private class MechanicContext {
      public MechanicContext Parent;
      public bool Visible = true;
      public EnemyController Source;
      public Vector2 Position;
      public ICollision Collision;
      public NetworkPlayer Target;
      public bool IsTargeted;
      public Vector2 Forward = Vector2.up;
    }

    private interface ICollision {
      public bool CollidesWith(Vector2 position);
    }

    private class RoundCollision : ICollision {
      private Vector2 _position;
      private Vector2 _direction;
      private float _radius;
      private float _innerRadius;
      private float _angle;

      public RoundCollision(Vector2 position, Vector2 direction, float radius, float innerRadius,
                            float angle) {
        _position = position;
        _direction = direction;
        _radius = radius;
        _innerRadius = innerRadius;
        _angle = angle;
      }

      public bool CollidesWith(Vector2 position) {
        Vector2 diff = position - _position;
        bool distance = _innerRadius <= diff.magnitude && diff.magnitude <= _radius;
        bool angle = (diff == Vector2.zero) || Vector2.Angle(diff, _direction) <= _angle / 2;
        return distance && angle;
      }
    }
  }
}
