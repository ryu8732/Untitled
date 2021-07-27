using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardItem
{
    public int itemId;
    public int amount;
}

[System.Serializable]
public class Quest
{
    public enum QuestStatus
    {
        Ready = 0,
        Proceeding = 1,
        Completed = 2,
        InActive = 3
    }

    public int startNpcId;
    public int endNpcId;
    public int questId;
    public string questName;

    // {{id, amount}, {id, amount}...}
    public RewardItem[] rewardItemList;

    public int rewardGold;
    public int linkedQuestId;
    public string description;

    public QuestStatus questStatus;

    public Task[] tasks;

    public Quest(int startNpcId, int endNpcId, int questId, string questName, Task[] tasks, int linkedQuestId = 0)
    {
        this.startNpcId = startNpcId;
        this.endNpcId = endNpcId;
        this.questId = questId;
        this.questName = questName;

        this.tasks = tasks;
        this.linkedQuestId = linkedQuestId;
    }
}
