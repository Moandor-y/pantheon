using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Mechanics {
  public static class DsrP2 {
    private const string _mechanicVisible = "MechanicVisible";
    private const string _mechanicNonTargeted = "MechanicNonTargeted";

    private const string _spawnThordan = "SpawnThordan";
    private const string _thordanMechanics = "ThordanMechanics";

    private const string _ascalonsMercyConcealed = "AscalonsMercyConcealed";
    private const string _ascalonsMercyConcealedTargeted = "AscalonsMercyConcealedTargeted";

    private const float _ascalonsMercyConcealedCastDuration = 2.6833333333333333333333333333333f;
    private const float _ascalonsMercyConcealedDamageDelay = 1.8833333333333333333333333333333f;

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();

      mechanicData.referenceMechanicProperties[_mechanicVisible] = new MechanicProperties() {
        visible = true,
      };
      mechanicData.referenceMechanicProperties[_mechanicNonTargeted] = new MechanicProperties() {
        isTargeted = false,
      };

      mechanicData.referenceMechanicProperties[_spawnThordan] = new MechanicProperties() {
        visible = false,
        mechanic =
            new SpawnEnemy() {
              enemyName = "King Thordan",
              textureFilePath = "Mechanics/Resources/Thordan.png",
              colorHtml = "#000000",
              maxHp = 1000000,
              baseMoveSpeed = 2,
              hitboxSize = 3,
              isTargetable = true,
              visualPosition = new Vector3(0, 2, 0),
              visualScale = new Vector3(4, 4, 4),
              referenceMechanicName = _thordanMechanics,
              position = new Vector2(0, 0),
            },
      };
      mechanicData.referenceMechanicProperties[_thordanMechanics] = new MechanicProperties() {
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    // new WaitEvent() {
                    //   timeToWait = 8.4333333333333333333333333333333f,
                    // },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _ascalonsMercyConcealed,
                    },
                    new WaitEvent() {
                      timeToWait = float.PositiveInfinity,
                    },
                  },
            },
      };

      mechanicData.referenceMechanicProperties[_ascalonsMercyConcealed] = new MechanicProperties() {
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new StartCastBar() {
                      castName = "Ascalon's Mercy Concealed",
                      duration = _ascalonsMercyConcealedCastDuration,
                    },
                    new SpawnTargetedEvents() {
                      referenceMechanicName = _ascalonsMercyConcealedTargeted,
                      targetingScheme = new TargetAllPlayers(),
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties[_ascalonsMercyConcealedTargeted] =
          new MechanicProperties() {
            visible = false,
            isTargeted = true,
            followSpeed = 0,
            collisionShape = CollisionShape.Round,
            collisionShapeParams = new Vector4(100, 24, 0, 0),
            colorHtml = "#ff0000",
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = _ascalonsMercyConcealedCastDuration,
                        },
                        new ModifyMechanicEvent() {
                          referenceMechanicName = _mechanicNonTargeted,
                        },
                        new WaitEvent() {
                          timeToWait = _ascalonsMercyConcealedDamageDelay,
                        },
                        new ModifyMechanicEvent() {
                          referenceMechanicName = _mechanicVisible,
                        },
                        new ApplyEffectToPlayers() {
                          effect =
                              new DamageEffect() {
                                damageAmount = 100000,
                              },
                        },
                        new WaitEvent() {
                          timeToWait = 0.8f,
                        },
                      },
                },
          };

      mechanicData.mechanicEvents = new List<MechanicEvent>() {
        new SpawnVisualObject() {
          textureFilePath = "Mechanics/Resources/ArenaCircle.png",
          relativePosition = new Vector3(0, -0.001f, 0),
          eulerAngles = new Vector3(90, 0, 0),
          scale = new Vector3(46.83f, 46.83f, 1),
          visualDuration = float.PositiveInfinity,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = _spawnThordan,
        },
      };
      return mechanicData;
    }
  }
}
