using System.Collections.Generic;
using UnityEngine;
using GameData;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Gameplay
{
    public class GameplayView : MonoBehaviour
    {
        [SerializeField] GameObject leftFloorContainer;
        [SerializeField] GameObject rightFloorContainer;
        [SerializeField] GameObject showCompletePanel;

        private const string LEFT_FLOOR_ADDRESS = "left";
        private const string RIGHT_FLOOR_ADDRESS = "right";
        private const string LAST_LEVEL = "Level 10";

        private const int OBJECTED_NUMBER_PER_FLOOR = 4;
        int clearTimeToComplete = 0;
        [SerializeField] TMP_Text currentLevelNameTxt;
        [SerializeField] Button nextLevelBtn;
        [SerializeField] Button replayGameBtn;
        [SerializeField] Button restartLevelBtn;
        [SerializeField] TMP_Text levelCurrentTxt;

        string currentLevelNameString;
        LevelDataInstance currentLevelData;

        public TMP_Text timerTxt;
        public TMP_Text timerCompletedTxt;
        [SerializeField] GameObject showStartPanel;
        [SerializeField] Button startGameBtn;
        [SerializeField] TMP_Text result;
        [SerializeField] Transform contentTrans;

        [SerializeField] AudioSource bgm;
        [SerializeField] AudioSource effM;

        async void Start()
        {
            await LoadSound();
            PlayBGMSound(SoundClipByName.SoundType.BGM);
            ShowStartOn();
            startGameBtn.onClick.AddListener(ShowStartOff);
            nextLevelBtn.onClick.AddListener(GoToNextLevel);
            replayGameBtn.onClick.AddListener(ReplayGame);
            restartLevelBtn.onClick.AddListener(RestartCurrentLevel);
        }

        async void ShowStartOn()
        {
            showStartPanel.SetActive(true);
            stopTimer = true;
            await UniTask.Delay(2000);
            startGameBtn.interactable = true;
        }

        async void ShowStartOff()
        {
            PlayEffSound(SoundClipByName.SoundType.ClickBtn);
            startGameBtn.interactable = false;
            await showStartPanel.GetComponentInChildren<CanvasGroup>().DOFade(0, 1);
            showStartPanel.SetActive(false);
            stopTimer = false;
            GenerateMap("Level 1");
        }

        private List<FloorView> floors = new List<FloorView>();

        public async void GenerateMap(string levelName)
        {
            currentLevelData = await Addressables.LoadAssetAsync<LevelDataInstance>(levelName);
            currentLevelNameString = levelName;
            levelCurrentTxt.text = $"{currentLevelNameString}";
            foreach (var floor in currentLevelData.floors)
            {
                var floorTmp = await Addressables.InstantiateAsync(floor.isLeftFloor ? LEFT_FLOOR_ADDRESS : RIGHT_FLOOR_ADDRESS, floor.isLeftFloor ? leftFloorContainer.transform : rightFloorContainer.transform);
                var floorView = floorTmp.GetComponent<FloorView>();
                if (floorView != null)
                {
                    floorView.SetData(floor);
                    floors.Add(floorView);
                    floorView.clickedFloor = HandleFloorClicked;
                }
            }
            clearTimeToComplete = GetTotalObjs() / OBJECTED_NUMBER_PER_FLOOR;
        }

        FloorView selectedFloor; //check floor selected ?
        //floorview = floor clicked
        void HandleFloorClicked(FloorView floorView) //xu li dieu kien floor dc click
        {
            if (selectedFloor == floorView) //click 2 lan
                return;
            //B1
            if (selectedFloor == null) //chua chon   
            {
                PlayEffSound(SoundClipByName.SoundType.Select);
                floorView.EnableBorder();//chon object di
                selectedFloor = floorView; //luu lai floor da chon
            }
            //B2
            else //da chon
            {
                bool canMove = CanMove(floorView); // Khi nao co thi move
                if (canMove)
                {
                    MoveObjs(floorView);
                }
                else
                {
                    ChangeSelected(floorView);
                }
            }
        }

        async UniTask MoveSingleObject(MovingObjectView needMoveObject, FloorView destinationFloor, int floorObjNum)
        {
            PlayEffSound(SoundClipByName.SoundType.Move);
            //needMoveObject.transform.SetParent(timerTxt.transform);
            needMoveObject.transform.DORotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            await needMoveObject.transform.DOMove(destinationFloor.GetPositionOfObjectByIndex(floorObjNum), 0.2f).SetEase(Ease.OutExpo);
            needMoveObject.transform.SetParent(destinationFloor.GetContainer());
        }
        int clearFloorTime = 0;
        async void MoveObjs(FloorView destinationFloor)
        {

            List<MovingObjectView> input = selectedFloor.GetMovingObjects();
            if (input.Count <= 0) return;
            int floorObjNum = destinationFloor.GetCurrentObjectNumber();
            bool isCompleteFloor = (floorObjNum + input.Count >= OBJECTED_NUMBER_PER_FLOOR) && destinationFloor.isAllSameColor(input[0]);
            selectedFloor = null;
            foreach (var obj in input)
            {
                obj.DeselectedObject();
            }
            for (int i = 0; i < input.Count; i++)
            {
                int currentFloorIndex = floorObjNum + i + 1;
                if (currentFloorIndex > OBJECTED_NUMBER_PER_FLOOR)
                    return;

                var needMoveObject = input[i];
                await MoveSingleObject(needMoveObject, destinationFloor, currentFloorIndex - 1);
                // needMoveObject.DeselectedObject();
            }
            await UniTask.DelayFrame(2);
            if (isCompleteFloor)
            {
                clearFloorTime++;
                PlayEffSound(SoundClipByName.SoundType.Destroy);
                await destinationFloor.RemoveAllObjects();
                if (clearFloorTime == clearTimeToComplete)
                {
                    Debug.Log("Complete");
                    ShowCompleteOn();
                }
                else
                {
                    Debug.Log("Not Complete");
                }
            }
        }

        // k move dc -> doi select sang floor vua click
        void ChangeSelected(FloorView destinationFloor)
        {
            List<MovingObjectView> objSelected = selectedFloor.GetMovingObjects();
            List<MovingObjectView> objDesstination = destinationFloor.GetMovingObjects();
            foreach (MovingObjectView obj in objSelected)
            {
                obj.DeselectedObject();
            }
            foreach (MovingObjectView obj in objDesstination)
            {
                PlayEffSound(SoundClipByName.SoundType.Select);
                obj.SelectedObject();
            }
            selectedFloor = destinationFloor;
        }

        bool CanMove(FloorView destinationFloor)
        {
            //tang di chuuyen den co meo o ngoai cung mau voi tang dang selected
            //so obj tang di chuyen toi < OBJECTED_NUMBER_PER_FLOOR
            List<MovingObjectView> selected = new List<MovingObjectView>();
            List<MovingObjectView> destination = new List<MovingObjectView>();
            selected = selectedFloor.GetMovingObjects();
            destination = destinationFloor.GetMovingObjects();

            //neu floor den rong
            if (destination.Count == 0)
            {
                return true;
            }
            //neu obj giua 2 floor cung mau va obj floor den < 4
            {
                if (selected.Count > 0 && selected[0].objectType.Value == destination[0].objectType.Value && destinationFloor.GetCurrentObjectNumber() < OBJECTED_NUMBER_PER_FLOOR)
                    return true;
            }
            return false;
        }

        int GetTotalObjs()
        {
            List<FloorData> floorDatas = new List<FloorData>();
            floorDatas = currentLevelData.floors;
            int total = 0;
            for (int i = 0; i < floorDatas.Count; i++)
            {
                int x = floorDatas[i].objects.Count;
                total += x;
            }
            return total;
        }

        void ShowCompleteOn()
        {
            showCompletePanel.GetComponentInChildren<CanvasGroup>().DOFade(1, 1);
            //showCompletePanel.transform.localScale = new Vector3(0.5f, 0.5f);
            //showCompletePanel.transform.DOScale(new Vector3(1, 1), 0.5f).SetEase(Ease.InQuad); //incubic, incirc, insine, inquad, inflash
            showCompletePanel.SetActive(true);
            //Khi het man choi hien ra thong bao: BAN DA PHA DAO
            //Khong hien nut next level m� hien nut replay. An vao se choi lai tu level 1
            SaveResultTimer(playTime);

            if (currentLevelNameString == LAST_LEVEL)
            {
                PlayEffSound(SoundClipByName.SoundType.ComleteGame);
                string minutes = ((int)playTime / 60).ToString();
                string seconds = (playTime % 60).ToString("f1");
                timerCompletedTxt.text = $"Total Time: {minutes + ":" + seconds}";
                //timerCompletedTxt.text = $"Total Time: {playTime:F2}";
                currentLevelNameTxt.text = $"Completed game";
                timerCompletedTxt.gameObject.SetActive(true);
                replayGameBtn.gameObject.SetActive(true);
                contentTrans.parent.parent.gameObject.SetActive(true);
                nextLevelBtn.gameObject.SetActive(false);
                ShowResultTime();
            }
            else
            {
                PlayEffSound(SoundClipByName.SoundType.Complete);
                currentLevelNameTxt.text = $"Completed {currentLevelNameString}";
                nextLevelBtn.gameObject.SetActive(true);
                timerCompletedTxt.gameObject.SetActive(false);
                replayGameBtn.gameObject.SetActive(false);
                contentTrans.parent.parent.gameObject.SetActive(false);
            }
            stopTimer = true;
        }

        void ShowCompleteOff()
        {
            showCompletePanel.SetActive(false);
            stopTimer = false;

        }
        async void GoToNextLevel()
        {
            PlayEffSound(SoundClipByName.SoundType.ClickBtn);
            nextLevelBtn.interactable = false;
            //clear man truoc
            foreach (Transform floor in leftFloorContainer.transform)
            {
                Addressables.ReleaseInstance(floor.gameObject);
            }
            foreach (Transform floor in rightFloorContainer.transform)
            {
                Addressables.ReleaseInstance(floor.gameObject);
            }
            foreach (Transform obj in timerTxt.transform)
            {
                Addressables.ReleaseInstance(obj.gameObject);
            }
            showCompletePanel.transform.localScale = new Vector3(1, 1);
            await showCompletePanel.GetComponentInChildren<CanvasGroup>().DOFade(0, 1);
            ShowCompleteOff();
            //thay doi "Level" -> " " va gan = 1 bien int  => currentLevelNameString = {0,1,2,3,4,5}
            //generate map tiep
            int curentLevelInt = int.Parse(currentLevelNameString.Replace("Level", "").Trim());
            string nextLevelName = $"Level {curentLevelInt + 1}";
            clearFloorTime = 0;
            await UniTask.DelayFrame(2);
            GenerateMap(nextLevelName);
            nextLevelBtn.interactable = true;

        }

        void ReplayGame()
        {
            PlayEffSound(SoundClipByName.SoundType.ClickBtn);
            currentLevelNameString = "Level 0";
            playTime = 0;
            GoToNextLevel();
            timeResultList.Clear();
            foreach (Transform trans in contentTrans)
            {
                Destroy(trans.gameObject);
            }
        }

        async void RestartCurrentLevel()
        {
            PlayEffSound(SoundClipByName.SoundType.ClickBtn);
            restartLevelBtn.interactable = false;
            int curentLevelInt = int.Parse(currentLevelNameString.Replace("Level", "").Trim());
            currentLevelNameString = $"Level {curentLevelInt - 1}";
            GoToNextLevel();
            await UniTask.Delay(1000);
            restartLevelBtn.interactable = true;
        }

        bool stopTimer;
        float playTime = 0;
        void Update()
        {
            if (stopTimer == false)
            {
                playTime += Time.deltaTime;
                string minutes = ((int)playTime / 60).ToString();
                string seconds = (playTime % 60).ToString("f1");
                timerTxt.text = minutes + ":" + seconds;
            }
        }

        List<float> timeResultList = new List<float>();
        void SaveResultTimer(float stopTime)
        {
            timeResultList.Add(stopTime);
        }

        void ShowResultTime()
        {
            //Debug.Log(timeResultList[0]);
            TMP_Text view1 = Instantiate(result, contentTrans);
            view1.text = "Level 1: " + timeResultList[0].ToString("f1");

            for (int i = 0; i < timeResultList.Count - 1; i++)
            {
                float x = timeResultList[i + 1] - timeResultList[i];
                //lv1
                TMP_Text view = Instantiate(result, contentTrans); //x = lv2++
                view.text = $"Level {i + 2}: " + x.ToString("f1");
            }
        }

        DataSound dataSounds;
        async UniTask LoadSound()
        {
            dataSounds = await Addressables.LoadAssetAsync<DataSound>("sound");
        }

        void PlayBGMSound(SoundClipByName.SoundType bmgType)
        {
            bgm.clip = dataSounds.GetClipByType(bmgType);
            bgm.loop = true;
            bgm.Play();
        }

        void PlayEffSound(SoundClipByName.SoundType effType)
        {
            AudioClip clip = dataSounds.GetClipByType(effType);
            effM.PlayOneShot(clip);
        }
    }
}