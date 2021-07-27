
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestManager : MonoBehaviour
{
    // �̱��� ���ٿ� ������Ƽ
    public static QuestManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<QuestManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static QuestManager m_instance; // �̱����� �Ҵ�� static ����

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
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
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

        // ���ǹ��� ���� UI�� ������ ����Ʈ�� �����Ѵ�.
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
        // ������ ����Ʈ�� �����ϰ� �� ����Ʈ�� ID�� �ش��ϴ� ������Ʈ�� ���� QuestChoice�� �����Ѵ�.
        // ����� ID�� �����ʿ� ��ϵ� Ŭ�� �̺�Ʈ�� �߻��� ��, �ش� ID�� �˸��� ����Ʈ�� �ҷ����� ������ �Ѵ�.
        GameObject questChoiceObj = ObjectPoolingManager.instance.GetQueue("questChoice");
        questChoiceObj.transform.SetParent(choiceUi.transform);
        questChoiceObj.transform.SetAsFirstSibling();
        questChoiceObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        questChoiceObj.GetComponent<QuestChoice>().quest = quest;
        questChoiceObj.GetComponentInChildren<Text>().text = quest.questName;

        // ���ٽ��� �̿��� onClick �̺�Ʈ ������ ���
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

        // linkedQuestId = 0 �� ����, ���� ����Ʈ�� �������� �ʴ� ����̴�.
        if (quest.linkedQuestId > 0)
        {
            questContainer.activeQuests[quest.linkedQuestId] = allQuests[quest.linkedQuestId];
            questChecker();
        }

        // ������ ������ ������ ��� ������ ���� & ��� ���� ����
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

        // ����Ʈ ��ǥ
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

        // ���� ������
        foreach (RewardItem item in quest.rewardItemList)
        {
            GameObject rewardObj = Instantiate(questRewardPrefab, questRewardLayout);
            rewardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DataManager.instance.itemDict[item.itemId].itemName;
            rewardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = " X " + item.amount;
            rewardObj.SetActive(true);
        }

        // ���� ���
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
