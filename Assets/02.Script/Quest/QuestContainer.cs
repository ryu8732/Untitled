using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestContainer
{
    public Dictionary<int, Quest> completedQuests = new Dictionary<int, Quest>();
    public Dictionary<int, Quest> activeQuests = new Dictionary<int, Quest>();

    public List<Quest> completedQuestList = new List<Quest>();
    public List<Quest> activeQuestList = new List<Quest>();

    public void DictionaryToList()
    {
        completedQuestList.Clear();
        activeQuestList.Clear();

        foreach (Quest quest in completedQuests.Values)
        {
            completedQuestList.Add(quest);
        }

        foreach (Quest quest in activeQuests.Values)
        {
            activeQuestList.Add(quest);
        }
    }

    public void ListToDictionary()
    {
        foreach (Quest quest in completedQuestList)
        {
            completedQuests[quest.questId] = quest;
        }

        foreach (Quest quest in activeQuestList)
        {
            activeQuests[quest.questId] = quest;
        }
    }
}
