using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Mechanics {
  public static class DsrP2 {
    private const string _mechanicVisible = "MechanicVisible";
    private const string _mechanicNonTargeted = "MechanicNonTargeted";
    private const string _mechanicArenaBoundary = "MechanicArenaBoundary";

    private const float _arenaRadius = 46.83f / 2;
    private const float _bossBaseSpeed = 10;

    private const string _statusLightningStormVuln = "LightningStormVuln";
    private const string _statusLightningStormVulnName = "Lightning Resistance Down II";

    private const string _statusSkywardLeapVuln = "SkywardLeapVuln";
    private const string _statusSkywardLeapVulnName = "Physical Vulnerability Up";

    private const string _damageTypePhysical = "Physical";
    private const string _damageTypeLightning = "Lightning";

    private const float _damageMultiplier = 100000;

    private const string _spawnThordan = "SpawnThordan";
    private const string _thordanMechanics = "ThordanMechanics";

    private const string _ascalonsMercyConcealed = "AscalonsMercyConcealed";
    private const string _ascalonsMercyConcealedTargeted = "AscalonsMercyConcealedTargeted";
    private const float _ascalonsMercyConcealedCastDuration = 2.6833333333333333333333333333333f;
    private const float _ascalonsMercyConcealedDamageDelay = 1.8833333333333333333333333333333f;

    private const string _strengthOfTheWard = "StrengthOfTheWard";
    private const float _strengthOfTheWardCastDuration = 3.6833333333333333333333333333333f;

    private const string _strengthOfTheWardHeavyImpact = "StrengthOfTheWardHeavyImpact";
    private const string _strengthOfTheWardHeavyImpactDamage = "StrengthOfTheWardHeavyImpactDamage";
    private const string _strengthOfTheWardHeavyImpactPool = "StrengthOfTheWardHeavyImpactPool";
    private const float _strengthOfTheWardHeavyImpactRadius = _arenaRadius / 6.5f;
    private const float _strengthOfTheWardHeavyImpactPosition =
        _strengthOfTheWardHeavyImpactRadius * 1.1f;
    private const float _strengthOfTheWardHeavyImpactBeforeStart =
        6.9666666666666666666666666666667f;
    private const float _strengthOfTheWardHeavyImpactCastDuration = 5.7f;
    private const float _strengthOfTheWardHeavyImpactDamageInterval =
        1.8166666666666666666666666666667f;

    private const string _strengthOfTheWardSpiralThrust = "StrengthOfTheWardSpiralThrust";
    private const string _strengthOfTheWardSpiralThrustPool = "StrengthOfTheWardSpiralThrustPool";

    private const string _strengthOfTheWardLightningStorm = "StrengthOfTheWardLightningStorm";
    private const string _strengthOfTheWardLightningStormSingle =
        "StrengthOfTheWardLightningStormSingle";
    private const float _strengthOfTheWardLightningStormEffectDuration = 1;
    private const float _strengthOfTheWardLightningStormRadius = 7;

    private const float _strengthOfTheWardAscalonsMercyConcealedBeforeStart =
        8.0666666666666666666666666666667f;

    private const string _strengthOfTheWardDimensionalCollapse =
        "StrengthOfTheWardDimensionalCollapse";
    private const string _strengthOfTheWardDimensionalCollapseSingle =
        "StrengthOfTheWardDimensionalCollapseSingle";
    private const string _strengthOfTheWardDimensionalCollapseSize =
        "StrengthOfTheWardDimensionalCollapseSize";
    private const float _strengthOfTheWardDimensionalCollapseBeforeStart =
        5.0333333333333333333333333333333f;
    private const float _strengthOfTheWardDimensionalCollapseCastDuration = 8.7f;
    private const float _strengthOfTheWardDimensionalCollapseSizeInitialDelay =
        2.9333333333333333333333333333333f;
    private const float _strengthOfTheWardDimensionalCollapseSizeInterval = 1.025f;

    private const string _strengthOfTheWardTheDragonsRage = "StrengthOfTheWardTheDragonsRage";
    private const string _strengthOfTheWardTheDragonsRageSkywardLeap =
        "StrengthOfTheWardTheDragonsRageSkywardLeap";
    private const float _strengthOfTheWardTheDragonsRageSkywardLeapRadius = 10;
    private const float _strengthOfTheWardTheDragonsRageSkywardLeapEffectDuration = 0.5f;

    private const string _strengthOfTheWardHolyShieldBash = "StrengthOfTheWardHolyShieldBash";
    private const string _strengthOfTheWardHolyShieldBashPool =
        "StrengthOfTheWardHolyShieldBashPool";
    private const string _strengthOfTheWardHolyShieldBashKnightMechanics =
        "StrengthOfTheWardHolyShieldBashPoolKnightMechanics";
    private const string _strengthOfTheWardHolyShieldBashTether =
        "StrengthOfTheWardHolyShieldBashTether";
    private const float _strengthOfTheWardHolyShieldBashCloseKnightsCoordinate = 7;

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();

      mechanicData.referenceMechanicProperties[_mechanicVisible] = new MechanicProperties() {
        visible = true,
      };
      mechanicData.referenceMechanicProperties[_mechanicNonTargeted] = new MechanicProperties() {
        isTargeted = false,
      };

      mechanicData.referenceMechanicProperties[_mechanicArenaBoundary] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(100, 360, _arenaRadius, 0),
        persistentTickInterval = 0.2f,
        persistentMechanic =
            new ApplyEffectToPlayers() {
              effect =
                  new DamageEffect() {
                    damageAmount = 9999999,
                  },
            },
      };
      mechanicData.referenceMechanicProperties[_spawnThordan] = new MechanicProperties() {
        visible = false,
        mechanic =
            new SpawnEnemy() {
              enemyName = "King Thordan",
              textureFilePath = "Mechanics/Resources/Thordan.png",
              colorHtml = "#000000",
              maxHp = 1000000,
              baseMoveSpeed = _bossBaseSpeed,
              hitboxSize = 3,
              isTargetable = true,
              visualPosition = new Vector3(0, 2, 0),
              visualScale = new Vector3(4, 4, 4),
              referenceMechanicName = _thordanMechanics,
              position = new Vector2(0, 0),
            },
      };
      mechanicData.referenceMechanicProperties[_thordanMechanics] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(1, 360, 0, 0),
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SetEnemyAggro() {
                      targetingScheme =
                          new TargetSpecificPlayerIds {
                            targetIds = new List<int>() { 0 },
                          },
                    },
                    // new WaitEvent() {
                    //   timeToWait = 8.4333333333333333333333333333333f,
                    // },
                    // new SpawnMechanicEvent() {
                    //   referenceMechanicName = _ascalonsMercyConcealed,
                    //   isPositionRelative = true,
                    // },
                    // new WaitEvent() {
                    //   timeToWait = _ascalonsMercyConcealedCastDuration,
                    // },
                    // new SetEnemyBaseSpeed() {
                    //   baseMoveSpeed = 0,
                    // },
                    // new WaitEvent() {
                    //   timeToWait = _ascalonsMercyConcealedDamageDelay,
                    // },
                    // new SetEnemyBaseSpeed() {
                    //   baseMoveSpeed = _bossBaseSpeed,
                    // },
                    // new WaitEvent() {
                    //   timeToWait = 16.5f - _ascalonsMercyConcealedCastDuration -
                    //   _ascalonsMercyConcealedDamageDelay,
                    // },
                    new SetEnemyMovement() {
                      position = new Vector2(0, 0),
                      movementTime = 0.1f,
                    },
                    new SetEnemyBaseSpeed() {
                      baseMoveSpeed = 0,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _strengthOfTheWard,
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
                      isPositionRelative = true,
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

      mechanicData.referenceMechanicProperties[_strengthOfTheWard] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    // new StartCastBar() {
                    //   castName = "Strength of the Ward",
                    //   duration = _strengthOfTheWardCastDuration,
                    // },
                    // new WaitEvent() {
                    //   timeToWait =
                    //       _strengthOfTheWardCastDuration +
                    //       _strengthOfTheWardHeavyImpactBeforeStart,
                    // },
                    // new ExecuteRandomEvents() {
                    //   mechanicPoolName = _strengthOfTheWardHeavyImpactPool,
                    // },
                    // new ExecuteRandomEvents() {
                    //   mechanicPoolName = _strengthOfTheWardSpiralThrustPool,
                    //   numberToSpawn = 3,
                    // },
                    // new SpawnMechanicEvent() {
                    //   referenceMechanicName = _strengthOfTheWardLightningStorm,
                    // },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _strengthOfTheWardHolyShieldBash,
                      rotation = 90,
                    },
                    new WaitEvent() {
                      timeToWait = _strengthOfTheWardAscalonsMercyConcealedBeforeStart,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _ascalonsMercyConcealed,
                    },
                    new WaitEvent() {
                      timeToWait = _strengthOfTheWardDimensionalCollapseBeforeStart,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _strengthOfTheWardDimensionalCollapse,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = _strengthOfTheWardTheDragonsRage,
                    },
                  },
            },
      };
      List<string> strengthOfTheWardHeavyImpactDamageNames = new List<string>();
      for (int i = 0; i < 5; ++i) {
        strengthOfTheWardHeavyImpactDamageNames.Add($"{_strengthOfTheWardHeavyImpactDamage}{i}");
      }
      mechanicData.referenceMechanicProperties[_strengthOfTheWardHeavyImpact] =
          new MechanicProperties() {
            visible = true,
            collisionShape = CollisionShape.Round,
            collisionShapeParams = new Vector4(_strengthOfTheWardHeavyImpactRadius, 360, 0, 0),
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardHeavyImpactCastDuration,
                        },
                        new SpawnMechanicEvent() {
                          referenceMechanicName = strengthOfTheWardHeavyImpactDamageNames[0],
                          isPositionRelative = true,
                        },
                      },
                },
          };
      for (int i = 0; i < strengthOfTheWardHeavyImpactDamageNames.Count; ++i) {
        List<MechanicEvent> events = new List<MechanicEvent>() {
          new ApplyEffectToPlayers() {
            effect =
                new DamageEffect() {
                  damageAmount = 100000,
                },
          },
          new WaitEvent() {
            timeToWait = _strengthOfTheWardHeavyImpactDamageInterval,
          },
        };
        if (i + 1 < strengthOfTheWardHeavyImpactDamageNames.Count) {
          events.Add(new SpawnMechanicEvent() {
            referenceMechanicName = strengthOfTheWardHeavyImpactDamageNames[i + 1],
            isPositionRelative = true,
          });
        }
        mechanicData.referenceMechanicProperties[strengthOfTheWardHeavyImpactDamageNames[i]] =
            new MechanicProperties() {
              visible = true,
              collisionShape = CollisionShape.Round,
              collisionShapeParams = new Vector4(_strengthOfTheWardHeavyImpactRadius * (i + 1), 360,
                                                 _strengthOfTheWardHeavyImpactRadius * i),
              mechanic =
                  new ExecuteMultipleEvents() {
                    events = events,
                  },
            };
      }

      mechanicData.referenceMechanicProperties[_strengthOfTheWardSpiralThrust] =
          new MechanicProperties() {
            visible = true,
            collisionShape = CollisionShape.Rectangle,
            collisionShapeParams = new Vector4(
                2 * _arenaRadius, 2 * _arenaRadius * Mathf.Sin(45.0f / 2 * Mathf.Deg2Rad), 0, 0),
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardHeavyImpactCastDuration,
                        },
                        new ApplyEffectToPlayers() {
                          effect =
                              new DamageEffect() {
                                damageAmount = 100000,
                              },
                        },
                      },
                },
          };

      mechanicData.referenceMechanicProperties[_strengthOfTheWardLightningStorm] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardHeavyImpactCastDuration,
                        },
                        new SpawnTargetedEvents() {
                          referenceMechanicName = _strengthOfTheWardLightningStormSingle,
                          targetingScheme = new TargetAllPlayers(),
                          isPositionRelative = true,
                          spawnOnTarget = true,
                        },
                      },
                },
          };

      mechanicData.referenceMechanicProperties[_strengthOfTheWardLightningStormSingle] =
          new MechanicProperties() {
            visible = true,
            isTargeted = true,
            collisionShape = CollisionShape.Round,
            collisionShapeParams = new Vector4(_strengthOfTheWardHeavyImpactRadius, 360, 0, 0),
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new ApplyEffectToPlayers() {
                          effects =
                              new List<MechanicEffect>() {
                                new DamageEffect() {
                                  damageAmount = 1,
                                  damageType = _damageTypeLightning,
                                },
                                new ApplyStatusEffect() {
                                  referenceStatusName = _statusLightningStormVuln,
                                },
                              },
                        },
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardLightningStormEffectDuration,
                        },
                      },
                },
          };

      {
        List<MechanicEvent> events = new List<MechanicEvent>();
        foreach (Vector2 position in new[] {
                   new Vector2(0, 10),
                   new Vector2(0, -10),
                   new Vector2(10, 0),
                   new Vector2(-10, 0),
                   new Vector2(14, 14),
                   new Vector2(14, -14),
                   new Vector2(-14, 14),
                   new Vector2(-14, -14),
                 }) {
          events.Add(new SpawnMechanicEvent() {
            referenceMechanicName = _strengthOfTheWardDimensionalCollapseSingle,
            position = position,
          });
        }
        mechanicData.referenceMechanicProperties[_strengthOfTheWardDimensionalCollapse] =
            new MechanicProperties() {
              visible = false,
              mechanic =
                  new ExecuteMultipleEvents() {
                    events = events,
                  },
            };
      }

      {
        List<MechanicEvent> events = new List<MechanicEvent>() {
          new WaitEvent() {
            timeToWait = _strengthOfTheWardDimensionalCollapseSizeInitialDelay,
          },
        };
        for (int i = 0; i < 6; ++i) {
          string name = $"{_strengthOfTheWardDimensionalCollapseSize}{i}";
          mechanicData.referenceMechanicProperties[name] = new MechanicProperties() {
            collisionShapeParams = new Vector4(3 + (i + 1) * 1.5f, 360, 0, 0),
          };
          events.AddRange(new MechanicEvent[] {
            new ModifyMechanicEvent() {
              referenceMechanicName = name,
            },
            new WaitEvent() {
              timeToWait = _strengthOfTheWardDimensionalCollapseSizeInterval,
            },
          });
        }
        mechanicData.referenceMechanicProperties[_strengthOfTheWardDimensionalCollapseSingle] =
            new MechanicProperties() {
              visible = true,
              collisionShape = CollisionShape.Round,
              collisionShapeParams = new Vector4(3, 360, 0, 0),
              mechanic =
                  new ExecuteMultipleEvents() {
                    events = events,
                  },
            };
      }

      mechanicData.referenceMechanicProperties[_strengthOfTheWardTheDragonsRage] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new ReshufflePlayerIds(),
                        new SpawnTargetedEvents() {
                          targetingScheme =
                              new TargetSpecificPlayerIdsByClass() {
                                classType = PlayerClassType.Tank,
                                invertCheck = true,
                                targetIds = new List<int>() { 0, 1, 2 },
                              },
                          referenceMechanicName = _strengthOfTheWardTheDragonsRageSkywardLeap,
                          spawnOnTarget = true,
                        },
                      },
                },
          };

      mechanicData.referenceMechanicProperties[_strengthOfTheWardTheDragonsRageSkywardLeap] =
          new MechanicProperties() {
            visible = false,
            isTargeted = true,
            followSpeed = 100,
            collisionShape = CollisionShape.Round,
            collisionShapeParams =
                new Vector4(_strengthOfTheWardTheDragonsRageSkywardLeapRadius, 360, 0, 0),
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new SpawnVisualObject() {
                          textureFilePath = "Mechanics/Resources/DefamationMarker.png",
                          relativePosition = new Vector3(0, 1, 0),
                          scale = new Vector3(1, 1, 1),
                          colorHtml = "#0000ff",
                          spawnOnPlayer = true,
                          isBillboard = true,
                          visualDuration = _strengthOfTheWardDimensionalCollapseCastDuration,
                        },
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardDimensionalCollapseCastDuration,
                        },
                        new ModifyMechanicEvent() {
                          referenceMechanicName = _mechanicVisible,
                        },
                        new ApplyEffectToPlayers() {
                          effects =
                              new List<MechanicEffect>() {
                                new DamageEffect() {
                                  damageAmount = 1,
                                  damageType = _damageTypePhysical,
                                },
                                new ApplyStatusEffect() {
                                  referenceStatusName = _statusSkywardLeapVuln,
                                },
                              },
                        },
                        new WaitEvent() {
                          timeToWait = _strengthOfTheWardTheDragonsRageSkywardLeapEffectDuration,
                        },
                      },
                },
          };

      mechanicData.referenceMechanicProperties[_strengthOfTheWardHolyShieldBash] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new SpawnEnemy() {
                          enemyName = "Ser Adelphel",
                          textureFilePath = "Mechanics/Resources/Knight.png",
                          maxHp = 1,
                          colorHtml = "#000000",
                          isTargetable = false,
                          isVisible = false,
                          visualPosition = new Vector3(0, 2, 0),
                          visualScale = new Vector3(4, 4, 4),
                          position =
                              new Vector2(_strengthOfTheWardHolyShieldBashCloseKnightsCoordinate,
                                          -_strengthOfTheWardHolyShieldBashCloseKnightsCoordinate),
                          referenceMechanicName = _strengthOfTheWardHolyShieldBashKnightMechanics,
                          isPositionRelative = true,
                          isRotationRelative = true,
                        },
                        new SpawnEnemy() {
                          enemyName = "Ser Janlenoux",
                          textureFilePath = "Mechanics/Resources/Knight.png",
                          maxHp = 1,
                          colorHtml = "#000000",
                          isTargetable = false,
                          isVisible = false,
                          visualPosition = new Vector3(0, 2, 0),
                          visualScale = new Vector3(4, 4, 4),
                          position =
                              new Vector2(-_strengthOfTheWardHolyShieldBashCloseKnightsCoordinate,
                                          -_strengthOfTheWardHolyShieldBashCloseKnightsCoordinate),
                          referenceMechanicName = _strengthOfTheWardHolyShieldBashKnightMechanics,
                          isPositionRelative = true,
                          isRotationRelative = true,
                        },
                        new WaitEvent() {
                          timeToWait = float.PositiveInfinity,
                        },
                      },
                },
          };

      mechanicData.referenceMechanicProperties[_strengthOfTheWardHolyShieldBashKnightMechanics] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = 2,
                        },
                        new SpawnTethersToPlayers() {
                          targetingScheme =
                              new TargetSpecificPlayerIds() {
                                targetIds =
                                    new List<int>() {
                                      0,
                                    },
                              },
                          referenceTetherName = _strengthOfTheWardHolyShieldBashTether,
                          tetherOffset = new Vector3(0, 1, 0),
                        },
                        new WaitEvent() {
                          timeToWait = float.PositiveInfinity,
                        },
                      },
                },
          };

      mechanicData.mechanicEvents = new List<MechanicEvent>() {
        new SpawnVisualObject() {
          textureFilePath = "Mechanics/Resources/ArenaCircle.png",
          relativePosition = new Vector3(0, -0.001f, 0),
          eulerAngles = new Vector3(90, 0, 0),
          scale = new Vector3(_arenaRadius * 2, _arenaRadius * 2, 1),
          visualDuration = float.PositiveInfinity,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = _spawnThordan,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = _mechanicArenaBoundary,
        },
      };

      mechanicData.mechanicPools = new Dictionary<string, List<MechanicEvent>> {
        { _strengthOfTheWardHeavyImpactPool, new List<MechanicEvent>() },
        { _strengthOfTheWardSpiralThrustPool, new List<MechanicEvent>() },
      };
      for (int i = 0; i < 8; ++i) {
        int degrees = i * 45;
        mechanicData.mechanicPools[_strengthOfTheWardHeavyImpactPool].Add(new SpawnMechanicEvent() {
          referenceMechanicName = _strengthOfTheWardHeavyImpact,
          position = new Vector2(
              Mathf.Cos(Mathf.Deg2Rad * degrees) * _strengthOfTheWardHeavyImpactPosition,
              Mathf.Sin(Mathf.Deg2Rad * degrees) * _strengthOfTheWardHeavyImpactPosition),
        });
      }
      for (int i = 0; i < 4; ++i) {
        int degrees = i * 45;
        mechanicData.mechanicPools[_strengthOfTheWardSpiralThrustPool].Add(
            new SpawnMechanicEvent() {
              referenceMechanicName = _strengthOfTheWardSpiralThrust,
              position = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * degrees) * _arenaRadius,
                                     -Mathf.Cos(Mathf.Deg2Rad * degrees) * _arenaRadius),
              rotation = degrees,
            });
      }

      mechanicData.referenceStatusProperties = new Dictionary<string, StatusEffectData> {
        { _statusLightningStormVuln,
          new DamageModifier() {
            damageMultiplier = _damageMultiplier,
            damageType = _damageTypeLightning,
            statusName = _statusLightningStormVuln,
            statusDescription = _statusLightningStormVulnName,
            duration = 3,
            statusIconPath = "Mechanics/Resources/LightningVuln.png",
          } },
        { _statusSkywardLeapVuln,
          new DamageModifier() {
            damageMultiplier = _damageMultiplier,
            damageType = _damageTypePhysical,
            statusName = _statusSkywardLeapVuln,
            statusDescription = _statusLightningStormVulnName,
            duration = 6,
            statusIconPath = "Mechanics/Resources/PhysicalVuln.png",
          } },
      };

      mechanicData.referenceTetherProperties = new Dictionary<string, TetherProperties>();

      mechanicData.referenceTetherProperties[_strengthOfTheWardHolyShieldBashTether] =
          new TetherProperties() {
            tetherDuration = float.PositiveInfinity,
            retargetRandomPlayerOnDeath = true,
            oneTetherPerPlayer = true,
            interceptMechanic = new SwitchTetheredPlayer(),
            colorHtml = "#00ff60",
          };

      return mechanicData;
    }
  }
}
