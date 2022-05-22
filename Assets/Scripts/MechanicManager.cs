using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.Netcode;

namespace Pantheon
{
    public class MechanicManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _visualObjectPrefab;

        [SerializeField]
        private GameObject _enemyPrefab;

        [SerializeField]
        private GameObject _aoeMarkerPrefab;

        private XivSimParser.MechanicData _mechanicData;

        private int _nextEnemyId = 1;
        private MersenneTwister _random;
        private List<NetworkPlayer> _players;

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

        private IEnumerator Execute(XivSimParser.MechanicData mechanicData)
        {
            if (_mechanicData != null || _players != null)
            {
                throw new InvalidOperationException();
            }
            _mechanicData = mechanicData;
            _players = new List<NetworkPlayer>(GlobalContext.Instance.Players);

            foreach (NetworkPlayer player in _players) {
                player.MaxHealth = Mathf.RoundToInt(_mechanicData.defaultHealth);
                player.Health = player.MaxHealth;
            }

            foreach (XivSimParser.MechanicEvent mechanicEvent in _mechanicData.mechanicEvents)
            {
                yield return Execute(mechanicEvent, new MechanicContext());
            }

            while (_coroutines.Count > 0) {
                Coroutine coroutine = _coroutines[_coroutines.Count - 1];
                _coroutines.RemoveAt(_coroutines.Count - 1);
                yield return coroutine;
            }
            _mechanicData = null;
            _random = null;
            _players = null;
        }

        private IEnumerator Execute(XivSimParser.MechanicEvent mechanicEvent, MechanicContext mechanicContext)
        {
            yield return Execute((dynamic)mechanicEvent, mechanicContext);
        }

        private IEnumerator Execute(XivSimParser.SpawnVisualObject spawnVisualObject, MechanicContext mechanicContext)
        {
            SpawnVisualObjectClientRpc(
                visible: mechanicContext.Visible,
                texturePath: spawnVisualObject.textureFilePath,
                position: spawnVisualObject.relativePosition,
                rotation: Quaternion.Euler(spawnVisualObject.eulerAngles),
                scale: spawnVisualObject.scale);
            yield break;
        }

        private IEnumerator Execute(XivSimParser.SpawnMechanicEvent spawnMechanicEvent, MechanicContext mechanicContext)
        {
            _coroutines.Add(StartCoroutine(Execute(_mechanicData.referenceMechanicProperties[spawnMechanicEvent.referenceMechanicName], new MechanicContext() {
                Parent = mechanicContext,
                Visible = mechanicContext.Visible,
                SourceId = mechanicContext.SourceId,
                Position = mechanicContext.Position + spawnMechanicEvent.position,
                Collision = mechanicContext.Collision,
            })));
            yield break;
        }

        private IEnumerator Execute(XivSimParser.MechanicProperties mechanicProperties, MechanicContext mechanicContext)
        {
            if (mechanicProperties.visible) {
                float radius;
                float angle;
                if (mechanicProperties.collisionShape == XivSimParser.CollisionShape.Round) {
                    radius = mechanicProperties.collisionShapeParams.x;
                    angle = mechanicProperties.collisionShapeParams.y;
                } else {
                    throw new NotImplementedException();
                }
                SpawnAoeMarkerClientRpc(
                    position: new Vector3(mechanicContext.Position.x, 0, mechanicContext.Position.y),
                    duration: GetDuration(mechanicProperties.mechanic),
                    radius: radius,
                    angle: angle);
            }

            ICollision collision = mechanicContext.Collision;
            if (mechanicProperties.collisionShape == XivSimParser.CollisionShape.Round) {
                collision = new RoundCollision(mechanicContext.Position, mechanicProperties.collisionShapeParams.x);
            }

            _coroutines.Add(StartCoroutine(Execute(mechanicProperties.mechanic, new MechanicContext() {
                Parent = mechanicContext,
                Visible = mechanicContext.Visible && mechanicProperties.visible,
                SourceId = mechanicContext.SourceId,
                Position = mechanicContext.Position,
                Collision = collision,
            })));

            yield break;
        }

        private IEnumerator Execute(XivSimParser.ExecuteMultipleEvents executeMultipleEvents, MechanicContext mechanicContext) {
            foreach (XivSimParser.MechanicEvent mechanicEvent in executeMultipleEvents.events) {
                yield return Execute(mechanicEvent, mechanicContext);
            }
        }

        private IEnumerator Execute(XivSimParser.ExecuteRandomEvents executeRandomEvents, MechanicContext mechanicContext) {
            List<XivSimParser.MechanicEvent> pool = _mechanicData.mechanicPools[executeRandomEvents.mechanicPoolName];
            for (int i = 0; i < executeRandomEvents.numberToSpawn; ++i) {
                _coroutines.Add(StartCoroutine(Execute(pool[_random.Range(0, pool.Count)], mechanicContext)));
            }
            yield break;
        }

        private IEnumerator Execute(XivSimParser.SpawnEnemy spawnEnemy, MechanicContext mechanicContext) {
            int enemyId = _nextEnemyId;
            ++_nextEnemyId;

            EnemyController enemy = Instantiate(_enemyPrefab).GetComponent<EnemyController>();
            enemy.GetComponent<NetworkObject>().Spawn(true);
            enemy.SetIdServerRpc(enemyId);
            enemy.transform.localScale = spawnEnemy.visualScale;
            enemy.SetTexturePathServerRpc(spawnEnemy.textureFilePath);
            enemy.SetNameServerRpc(spawnEnemy.enemyName);

            yield return Execute(_mechanicData.referenceMechanicProperties[spawnEnemy.referenceMechanicName], new MechanicContext() {
                Parent = mechanicContext,
                Visible = mechanicContext.Visible,
                SourceId = enemyId,
                Position = mechanicContext.Position,
            });
        }

        private IEnumerator Execute(XivSimParser.StartCastBar startCastBar, MechanicContext mechanicContext) {
            EnemyController source = GlobalContext.Instance.GetEnemyById(mechanicContext.SourceId);
            source.Cast(startCastBar.castName, startCastBar.duration);
            yield break;
        }

        private IEnumerator Execute(XivSimParser.WaitEvent waitEvent, MechanicContext mechanicContext)  {
            yield return new WaitForSeconds(waitEvent.timeToWait);
        }

        private IEnumerator Execute(XivSimParser.ReshufflePlayerIds reshufflePlayerIds, MechanicContext mechanicContext) {
            for (int i = _players.Count - 1; i > 1; --i) {
                int j = _random.UniformInt(0, i);
                NetworkPlayer temp = _players[i];
                _players[i] = _players[j];
                _players[j] = temp;
            }
            yield break;
        }

        private IEnumerator Execute(XivSimParser.SpawnTargetedEvents spawnTargetedEvents, MechanicContext mechanicContext) {
            List<NetworkPlayer> targets = TargetPlayers(spawnTargetedEvents.targetingScheme);
            foreach (NetworkPlayer target in targets) {
                _coroutines.Add(StartCoroutine(Execute(_mechanicData.referenceMechanicProperties[spawnTargetedEvents.referenceMechanicName], new MechanicContext() {
                    Parent = mechanicContext,
                    Visible = mechanicContext.Visible,
                    SourceId = mechanicContext.SourceId,
                    Position = new Vector2(target.transform.position.x, target.transform.position.z),
                    Collision = mechanicContext.Collision,
                })));
            }
            yield break;
        }

        private IEnumerator Execute(XivSimParser.ApplyEffectToPlayers applyEffectToPlayers, MechanicContext mechanicContext) {
            List<NetworkPlayer> hit = new List<NetworkPlayer>();
            foreach (NetworkPlayer player in _players) {
                if (mechanicContext.Collision.CollidesWith(new Vector2(player.transform.position.x, player.transform.position.z))) {
                    hit.Add(player);
                }
            }
            ApplyEffect(applyEffectToPlayers.effect, hit);
            yield break;
        }

        private void ApplyEffect(XivSimParser.MechanicEffect effect, List<NetworkPlayer> players) {
            if (effect is XivSimParser.DamageEffect) {
                var damageEffect = (XivSimParser.DamageEffect) effect;
                float damage = damageEffect.damageAmount / Mathf.Min(damageEffect.maxStackAmount, players.Count);
                foreach (NetworkPlayer player in players) {
                    player.Health -= Mathf.RoundToInt(damage);
                }
            }
        }

        private List<NetworkPlayer> TargetPlayers(XivSimParser.TargetingScheme targetingScheme) {
            List<NetworkPlayer> result = new List<NetworkPlayer>();
            if (targetingScheme is XivSimParser.TargetSpecificPlayerIds) {
                var targetSpecificPlayerIds = (XivSimParser.TargetSpecificPlayerIds) targetingScheme;
                foreach (int id in targetSpecificPlayerIds.targetIds) {
                    result.Add(_players[id % _players.Count]);
                }
            }
            return result;
        }

        private float GetDuration(XivSimParser.MechanicEvent mechanicEvent) {
            if (mechanicEvent is XivSimParser.WaitEvent) {
                return ((XivSimParser.WaitEvent) mechanicEvent).timeToWait;
            }
            if (mechanicEvent is XivSimParser.ExecuteMultipleEvents) {
                float duration = 0;
                foreach (XivSimParser.MechanicEvent subMechanicEvent in ((XivSimParser.ExecuteMultipleEvents) mechanicEvent).events) {
                    duration += GetDuration(subMechanicEvent);
                }
                return duration;
            }
            return 0;
        }

        [ClientRpc]
        private void SpawnVisualObjectClientRpc(bool visible, string texturePath, Vector3 position, Quaternion rotation, Vector3 scale) {
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
        private void SpawnAoeMarkerClientRpc(Vector3 position, float duration, float radius, float angle) {
            AoeMarker aoeMarker = Instantiate(_aoeMarkerPrefab).GetComponent<AoeMarker>();
            aoeMarker.transform.position = position;
            aoeMarker.Duration = duration;
            aoeMarker.Radius = radius;
            aoeMarker.Angle = angle;
        }

        private class MechanicContext
        {
            public MechanicContext Parent;
            public bool Visible = true;
            public int SourceId;
            public Vector2 Position;
            public ICollision Collision;
        }

        private interface ICollision {
            public bool CollidesWith(Vector2 position);
        }

        private class RoundCollision : ICollision {
            private Vector2 _position;
            private float _maxRange;

            public RoundCollision(Vector2 position, float maxRange) {
                _position = position;
                _maxRange = maxRange;
            }

            public bool CollidesWith(Vector2 position) {
                return Vector2.Distance(position, _position) < _maxRange;
            }
        }
    }
}
