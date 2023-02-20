using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ScriptPractice2 : MonoBehaviour
{
   async void Start()
    {
        ScriptableData data;
        data = await Addressables.LoadAssetAsync<ScriptableData>("dataColor") ;
        for (int i = 0; i < 7; i++)
        {
            var image = await Addressables.InstantiateAsync("img", transform);
            var img = image.GetComponent<Image>();
            img.color = data.colors[i];
        }
    } 
}
