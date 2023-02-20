using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ScriptPractice3 : MonoBehaviour
{
    ScriptableData data;

    async void Start()
    {
        data = await Addressables.LoadAssetAsync<ScriptableData>("dataColor");
        for(int i = 0; i < data.selectedColorIndex.Count; i++)
        {
            int x = data.selectedColorIndex[i];
            var obj = await Addressables.InstantiateAsync("img", transform);
            var img = obj.GetComponent<Image>();
            img.color = data.colors[x];
        }
    }
}
