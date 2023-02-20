using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class DataConfig
{
    public enum MovingObjectType
    {
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        GREY,
        PURPLE,
        WHITE
    }

    public async static UniTask<Sprite> GetSpriteByType(MovingObjectType type)
    {
        switch (type)
        {
            case MovingObjectType.RED:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_27]");
            case MovingObjectType.YELLOW:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_0]");
            case MovingObjectType.BLUE:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_62]");
            case MovingObjectType.GREEN:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_43]");
            case MovingObjectType.PURPLE:
                return await Addressables.LoadAssetAsync<Sprite>("Assets/Hisa Cube Animal/Others/Catverse Spritesheet 1.1.png[Catverse Spritesheet 1.1_8]");
            case MovingObjectType.GREY:
                return await Addressables.LoadAssetAsync<Sprite>("Assets/Hisa Cube Animal/Others/Catverse Spritesheet 1.1.png[Catverse Spritesheet 1.1_28]");
            case MovingObjectType.WHITE:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_16]");
            default:
                return await Addressables.LoadAssetAsync<Sprite>("Cat[Catverse Spritesheet 1.0_29]");
        }
    }
}