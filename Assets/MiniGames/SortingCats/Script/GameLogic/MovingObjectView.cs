using UnityEngine;
using UnityEngine.UI;
using GameData;
using UniRx;
using MOVING_OBJECT_TYPE = DataConfig.MovingObjectType;
using Cysharp.Threading.Tasks;

namespace Gameplay
{
    public class MovingObjectView : MonoBehaviour
    {
        public ReactiveProperty<MOVING_OBJECT_TYPE> objectType = new ReactiveProperty<MOVING_OBJECT_TYPE>();
        public Image colorImage;
        public Image border;
        void Awake()
        {
            objectType.DistinctUntilChanged().Subscribe(SetColorByType).AddTo(this);
        }
        public void SetData(MovingObjectData data)
        {
            objectType.Value = data.type;
            DeselectedObject();
        }

        async void SetColorByType(MOVING_OBJECT_TYPE type)
        {
            colorImage.sprite = await DataConfig.GetSpriteByType(type);
        }

        public void SelectedObject()
        {
            border.enabled = true;
        }

        public void DeselectedObject()
        {
            border.enabled = false;
        }
    }
}