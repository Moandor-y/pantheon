using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

namespace Pantheon
{
public class ButtonStartClient : MonoBehaviour
{
    public TMP_InputField TargetHost;
    public TMP_InputField MechanicPath;

    void Start() {
        MechanicPath.text = PlayerPrefs.GetString("mechanic_path", string.Empty);
    }

    public void OnClick() {
        var networkManager = NetworkManager.Singleton;
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = TargetHost.text;
        networkManager.StartClient();
        GlobalContext.Instance.MechanicPath = MechanicPath.text;
        PlayerPrefs.SetString("mechanic_path", GlobalContext.Instance.MechanicPath);
    }
}
}
