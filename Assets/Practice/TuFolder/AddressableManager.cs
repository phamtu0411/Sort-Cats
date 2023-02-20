using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] List<Image> images;
    ScriptableData data;

    void Start()
    {
        LoadAddressable("dataColor");
    }

    void LoadAddressable(string path)
    {
        Addressables.LoadAssetAsync<ScriptableData>(path).Completed += AddressableCompleted;
    }

    void SetColor()
    {
        for (int i = 0; i < 7; i++)
        {
            images[i].color = data.colors[i];
        }
    }

    public void AddressableCompleted(AsyncOperationHandle<ScriptableData> handle)
    {
        data = handle.Result;
        SetColor();
    }
}
