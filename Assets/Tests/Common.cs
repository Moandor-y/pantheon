using System.Collections.Generic;
using UnityEngine;
using static Pantheon.XivSimParser;

namespace Pantheon.Test {
  public static class Common {
    public const string MechanicVisible = "MechanicVisible";
    public const string MechanicNonTargeted = "MechanicNonTargeted";
    public const string SpawnBoss = "SpawnBoss";
    public const string BossMechanic = "BossMechanic";

    public static MechanicData BaseMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();
      mechanicData.referenceTetherProperties = new Dictionary<string, TetherProperties>();
      mechanicData.mechanicPools = new Dictionary<string, List<MechanicEvent>>();
      mechanicData.referenceStatusProperties = new Dictionary<string, StatusEffectData>();
      mechanicData.defaultHealth = 50000;
      mechanicData.referenceMechanicProperties["MechanicArenaBoundary"] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(100, 360, 25, 0),
        persistentTickInterval = 0.2f,
        persistentMechanic =
            new ApplyEffectToPlayers() {
              effect =
                  new DamageEffect() {
                    damageAmount = 9999999,
                  },
            },
      };
      mechanicData.mechanicEvents = new List<MechanicEvent>() {
        new SpawnVisualObject() {
          textureFilePath = "Mechanics/Resources/ArenaCircle.png",
          relativePosition = new Vector3(0, -0.001f, 0),
          eulerAngles = new Vector3(90, 0, 0),
          scale = new Vector3(50, 50, 1),
          visualDuration = float.PositiveInfinity,
        },
        new SpawnEnemy() {
          enemyName = "Boss",
          textureFilePath = "Mechanics/Resources/Thordan.png",
          colorHtml = "#000000",
          maxHp = 1000000,
          baseMoveSpeed = 10,
          hitboxSize = 3,
          isTargetable = true,
          visualPosition = new Vector3(0, 2, 0),
          visualScale = new Vector3(4, 4, 4),
          referenceMechanicName = BossMechanic,
          position = new Vector2(0, 0),
        },
      };
      mechanicData.referenceMechanicProperties[MechanicVisible] = new MechanicProperties() {
        visible = true,
      };
      mechanicData.referenceMechanicProperties[MechanicNonTargeted] = new MechanicProperties() {
        isTargeted = false,
      };
      return mechanicData;
    }
  }
}
