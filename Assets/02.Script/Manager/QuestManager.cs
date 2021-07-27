
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static QuestManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<QuestManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static QuestManager m_instance; // 싱글톤이 할당될 static 변수

    public Dictionary<int, Quest> allQuests;

    public QuestContainer questContainer;

    public GameObject choiceUi;
    public GameObject questUi;
    public Transform questSlotContainer;

    [Header("Quest Infomation Panel")]
    public GameObject questInfoPanel;
    public TextMeshProUGUI questNameText;
    public VerticalLayoutGroup questInfoLayout;
    public Transform questTaskLayout;
    public GameObject questTaskPrefab;
    public Transform questRewardLayout;
    public GameObject questRewardPrefab;
    public TextMeshProUGUI questInfoDescription;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        allQuests = new Dictionary<int, Quest>();
    }

    public void questChecker()
    {
        if (GameManager.instance.npcList != null)
        {
            Dictionary<int, GameObject> npcDic = GameManager.instance.npcList;

            foreach (Quest quest in questContainer.activeQuests.Values)
            {
                if (quest.questStatus == Quest.QuestStatus.Ready)
                {
                    if (npcDic.ContainsKey(quest.startNpcId))
                    {
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(true);
                        npcDic[quest.startNpcId].GetComponent<NpcInfo>().questionMark.SetActive(false);
                    }
                }

                else if (quest.questStatus == Quest.QuestStatus.Proceeding)
                {

                    bool isGoal = true;
                    foreach (Task task in quest.tasks)
                    {
                        if (task.taskType == Task.TaskType.Item)
                        {
                            Debug.Log("item");
                            task.currentCount = InventoryManager.instance.inventory.GetItemAmount(task.targetId);
                        }

                        if (task.goalCount > task.currentCount)
                        {
                            isGoal = false;
                            break;
                        }
                        if (npcDic.ContainsKey(quest.endNpcId))
                        {
                            if (isGoal)
                            {
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().questionMark.SetActive(true);
                            }
                            else
                            {
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                                npcDic[quest.endNpcId].GetComponent<NpcInfo>().questionMark.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }


    public void ShowNpcQuestList(int npcId)
    {
        for (int i = 0; i < choiceUi.transform.childCount - 2; i++)
        {
            ObjectPoolingManager.instance.InsertQueue(choiceUi.transform.GetChild(i).gameObject, "questChoice");
        }

        // 조건문을 통해 UI상에 보여줄 퀘스트를 결정한다.
        foreach (Quest quest in questContainer.activeQuests.Values)
        {
            switch (quest.questStatus)
            {
                case Quest.QuestStatus.Ready:
                    if (quest.startNpcId == npcId)
                    {
                        AddQuestChoiceSlot(quest);
                    }
                    break;
                case Quest.QuestStatus.Proceeding:
                    if (quest.endNpcId == npcId)
                    {
                        bool isGoal = true;
                        foreach (Task task in quest.tasks)
                        {
                            if (task.taskType == Task.TaskType.Item)
                            {
                                task.currentCount = InventoryManager.instance.inventory.GetItemAmount(task.targetId);
                            }

                            if (task.goalCount > task.currentCount)
                            {
                                isGoal = false;
                                break;
                            }
                        }

                        if (isGoal)
                        {
                            AddQuestChoiceSlot(quest);
                        }
                    }
                    break;
            }
        }
    }

    public void AddQuestChoiceSlot(Quest quest)
    {
        // 결정된 퀘스트를 생성하고 각 퀘스트의 ID를 해당하는 오브젝트가 지닌 QuestChoice에 저장한다.
        // 저장된 ID는 리스너에 등록된 클릭 이벤트가 발생할 때, 해당 ID에 알맞은 퀘스트를 불러오는 역할을 한다.
        GameObject questChoiceObj = ObjectPoolingManager.instance.GetQueue("questChoice");
        questChoiceObj.transform.SetParent(choiceUi.transform);
        questChoiceObj.transform.SetAsFirstSibling();
        questChoiceObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        questChoiceObj.GetComponent<QuestChoice>().quest = quest;
        questChoiceObj.GetComponentInChildren<Text>().text = quest.questName;

        // 람다식을 이용해 onClick 이벤트 리스너 등록
        questChoiceObj.GetComponent<Button>().onClick.AddListener(() => { choiceUi.transform.parent.GetComponent<NpcUI>().OnClickQuestButton(questChoiceObj.GetComponent<QuestChoice>()); });
    }

    public void QuestClear(int questId)
    {
        Quest quest = questContainer.activeQuests[questId];

        foreach (Task task in quest.tasks)
        {
            if (task.taskType == Task.TaskType.Item)
            {
                InventoryManager.instance.inventory.ReduceItem(task.targetId, task.goalCount);
            }
        }

        questContainer.completedQuests[questId] = quest;
        questContainer.activeQuests.Remove(questId);

        // linkedQuestId = 0 인 경우는, 연계 퀘스트가 존재하지 않는 경우이다.
        if (quest.linkedQuestId > 0)
        {
            questContainer.activeQuests[quest.linkedQuestId] = allQuests[quest.linkedQuestId];
            questChecker();
        }

        // 아이템 보상이 존재할 경우 아이템 지급 & 골드 보상 지급
        if (quest.rewardItemList.Length != 0)
        {
            foreach (RewardItem rewardItem in quest.rewardItemList)
            {
                GameManager.instance.playerStatement.inventory.AddItem(DataManager.instance.itemDict[rewardItem.itemId], rewardItem.amount);
            }
        }

        GameManager.instance.playerStatement.inventory.AddGold(quest.rewardGold);
    }

    public void EndOfQuestTalk(int questId)
    {
        Quest quest = questContainer.activeQuests[questId];

        switch (quest.questStatus)
        {
            case Quest.QuestStatus.Ready:
                GameManager.instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                GameManager.instance.npcList[quest.startNpcId].GetComponent<NpcInfo>().questionMark.SetActive(false);

                quest.questStatus = Quest.QuestStatus.Proceeding;
                break;
            case Quest.QuestStatus.Proceeding:
                GameManager.instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().exclamationMark.SetActive(false);
                GameManager.instance.npcList[quest.endNpcId].GetComponent<NpcInfo>().questionMark.SetActive(false);

                quest.questStatus = Quest.QuestStatus.Completed;
                QuestClear(questId);
                break;
        }

        questChecker();
    }

    public void TagetEnemyKilled(int targetId)
    {
        foreach (Quest quest in questContainer.activeQuests.Values)
        {
            foreach (Task task in quest.tasks)
            {
                if (task.taskType == Task.TaskType.Hunt && task.targetId == targetId)
                {
                    task.currentCount++;
                    questChecker();
                }
            }
        }
    }

    public void ShowProgressQuest()
    {
        while (questSlotContainer.childCount > 0)
        {
            ObjectPoolingManager.instance.InsertQueue(questSlotContainer.GetChild(0).gameObject, "questSlot");
        }

        foreach (Quest quest in questContainer.activeQuests.Values)
        {
            if (quest.questStatus == Quest.QuestStatus.Proceeding)
            {
                GameObject slotObj = ObjectPoolingManager.instance.GetQueue("questSlot");
                slotObj.transform.SetParent(questSlotContainer);
                slotObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                EventTrigger eventTrigger = slotObj.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((data) => { OnSlotPointerDown((PointerEventData)data); });
                eventTrigger.triggers.Add(pointerDown);

                slotObj.GetComponent<QuestSlot>().questId = quest.questId;
                slotObj.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;
            }
        }
        questUi.SetActive(true);
    }

    private void OnSlotPointerDown(PointerEventData data)
    {
        int questId = data.pointerEnter.GetComponentInParent<QuestSlot>().questId;
        Quest quest = questContainer.activeQuests[questId];


        for (int i = 0; i < questTaskLayout.childCount; i++)
        {
            Destroy(questTaskLayout.GetChild(i).gameObject);
        }

        for (int i = 0; i < questRewardLayout.childCount; i++)
        {
            Destroy(questRewardLayout.GetChild(i).gameObject);
        }


        questInfoPanel.SetActive(true);
        questNameText.text = quest.questName;

        // 퀘스트 목표
        foreach (Task task in quest.tasks)
        {
            GameObject taskObj = Instantiate(questTaskPrefab, questTaskLayout);
            taskObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = task.goalText;

            if (task.goalCount > 0)
            {
                taskObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Mathf.Clamp(task.currentCount, 0, task.goalCount) + "/" + task.goalCount;
                taskObj.transform.GetChild(1).gameObject.SetActive(true);
            }

            taskObj.SetActive(true);
        }

        // 보상 아이템
        foreach (RewardItem item in quest.rewardItemList)
        {
            GameObject rewardObj = Instantiate(questRewardPrefab, questRewardLayout);
            rewardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DataManager.instance.itemDict[item.itemId].itemName;
            rewardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " X " + item.amount;
            rewardObj.SetActive(true);
        }

        // 보상 골드
        if (quest.rewardGold != 0)
        {
            GameObject rewardObj = Instantiate(questRewardPrefab, questRewardLayout);
            rewardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Gold";
            rewardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " X " + quest.rewardGold;
            rewardObj.SetActive(true);
        }

        questInfoDescription.text = quest.description;

        Canvas.ForceUpdateCanvases();
        questInfoLayout.SetLayoutVertical();
    }
}
