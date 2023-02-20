using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using GameData;
using System.Linq;
using System;
using DG.Tweening;

namespace Gameplay
{
    public class FloorView : MonoBehaviour
    {
        [SerializeField] Button floorButton;
        [SerializeField] GameObject objectContainer;
        [SerializeField] Image floorImage;
        private List<MovingObjectView> movingsOnFloor = new List<MovingObjectView>();
        public Action<FloorView> clickedFloor;
        public List<Transform> transformByIndex;

        public async void SetData(FloorData data) //create object in container
        {
            floorImage.sprite = data.floorSprite;
            Debug.Log(floorImage.sprite);
            foreach (var movingObj in data.objects)
            {
                var tmpObj = await Addressables.InstantiateAsync("MovingObject", objectContainer.transform);
                var movingObjectTmp = tmpObj.GetComponent<MovingObjectView>();
                movingObjectTmp.SetData(movingObj);
                movingsOnFloor.Add(movingObjectTmp);
            }
            floorButton.onClick.AddListener(ClickedFloor);
        }

        void ClickedFloor()
        {
            clickedFloor?.Invoke(this);
        }

        public Transform GetContainer() //get vao container can move den
        {
            return objectContainer.transform;
        }

        public int GetCurrentObjectNumber() //count object / floor
        {
            return objectContainer.GetComponentsInChildren<MovingObjectView>().Length;//obj/tang
        }

        public List<MovingObjectView> GetMovingObjects() //lay obj can move
        {
            List<MovingObjectView> currentMovingObjs = new List<MovingObjectView>();
            List<MovingObjectView> result = new List<MovingObjectView>();
            MovingObjectView lastView = null; //obj cuoi

            currentMovingObjs = objectContainer.GetComponentsInChildren<MovingObjectView>().ToList();
            if (currentMovingObjs.Count > 0)
            {
                lastView = currentMovingObjs[currentMovingObjs.Count - 1];
                result.Add(lastView);
            }
            for (int i = currentMovingObjs.Count - 2; i >= 0; i--)
            {
                if (currentMovingObjs[i].objectType.Value == lastView.objectType.Value)
                {
                    result.Add(currentMovingObjs[i]);
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        async UniTask RemoveAllObjects(List<MovingObjectView> floorObjs) //destroy obj
        {
            GridLayoutGroup gridGrp = transform.GetComponentInChildren<GridLayoutGroup>();
            gridGrp.enabled = false;
            foreach (MovingObjectView view in floorObjs)
            {
                await view.transform.DOMove(view.transform.position + new Vector3(0, 0.2f, 0), 0.1f).SetEase(Ease.Linear);
                Addressables.ReleaseInstance(view.gameObject);
                await UniTask.Delay(2);
            }
            if (gridGrp != null)
                gridGrp.enabled = true;
        }

        public async UniTask RemoveAllObjects()
        {
            var movingObjs = objectContainer.GetComponentsInChildren<MovingObjectView>().ToList();
            await RemoveAllObjects(movingObjs);
        }

        public void EnableBorder() //boder obj selected
        {
            List<MovingObjectView> currentMovingObjs = new List<MovingObjectView>();
            currentMovingObjs = GetMovingObjects();
            foreach (var movingObject in currentMovingObjs)
            {
                movingObject.SelectedObject();
            }
        }

        public bool isAllSameColor(MovingObjectView input)
        {
            var checkElement = objectContainer.GetComponentsInChildren<MovingObjectView>().ToList();

            foreach (var obj in checkElement)
            {
                if (obj.objectType.Value != input.objectType.Value)
                    return false;
            }
            return true;
        }

        public Vector3 GetPositionOfObjectByIndex(int index)
        {
            if (index > 3 || index < 0) return Vector3.zero;
            return transformByIndex[index].position;
        }
    }
}