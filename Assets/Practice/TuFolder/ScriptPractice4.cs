using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ScriptPractice4 : MonoBehaviour
{
    [SerializeField] List<Image> images;
    ScriptableData data;

    void GetColor()
    {
        data.colors.Clear();
        for (int i = 0; i < images.Count; i++)
        {
            var currentImage =  images[i];
            data.colors.Add(currentImage.color);
        }

    }
    async void Start()
    {
        data = await Addressables.LoadAssetAsync<ScriptableData>("dataColorScene4");
        GetColor();
        for (int i = 0; i < data.selectedColorIndex.Count; i++)
        {
            int x = data.selectedColorIndex[i];
            var obj = await Addressables.InstantiateAsync("img", transform);
            var img = obj.GetComponent<Image>();
            img.color = data.colors[x];
        }
    }
}
