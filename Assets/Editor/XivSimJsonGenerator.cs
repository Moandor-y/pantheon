using UnityEditor;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;

namespace Pantheon.Editor {
  public class XivSimJsonGenerator {
    private const string _generateMechanicFiles = "Pantheon/Generate mechanic files";

    [MenuItem(_generateMechanicFiles)]
    private static void GenerateMechanicFiles() {
      File.WriteAllText("Mechanics/AlmightyJudgement.json",
                        JsonConvert.SerializeObject(
                            Mechanics.AlmightyJudgement.GetMechanicData(), Formatting.Indented,
                            new JsonSerializerSettings() {
                              SerializationBinder = new XivSimParser.TypeBinder(),
                              TypeNameHandling = TypeNameHandling.Auto,
                              DefaultValueHandling = DefaultValueHandling.Ignore,
                            }));
      File.WriteAllText(
          "Mechanics/DsrP2.json",
          JsonConvert.SerializeObject(Mechanics.DsrP2.GetMechanicData(), Formatting.Indented,
                                      new JsonSerializerSettings() {
                                        SerializationBinder = new XivSimParser.TypeBinder(),
                                        TypeNameHandling = TypeNameHandling.Auto,
                                        DefaultValueHandling = DefaultValueHandling.Ignore,
                                      }));
    }
  }
}
