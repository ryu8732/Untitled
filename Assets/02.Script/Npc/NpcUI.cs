using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcUI : MonoBehaviour
{
    public Quest currentQuest;

    public GameObject choiceUi;
    public GameObject talkUi;
    public GameObject storeUi;

    public GameObject talkChoice;
    public GameObject storeChioce;

    public Canvas canvas;
    public Transform cameraTr;
    public IEnumerator coroutine;

    public void OnEnable()
    {
        choiceUi.SetActive(false);
        talkUi.SetActive(false);
        storeUi.SetActive(false);
    }

    // ������ �� ��ȭ�ϱ�, ������ ������ ���� �ش� �������� Ȱ��/��Ȱ��ȭ �Ѵ�.
    public void SetChoiceUi(bool hasTalk, bool hasStore)
    {
        talkChoice.SetActive(hasTalk);
        storeChioce.SetActive(hasStore);

        choiceUi.SetActive(true);
    }


    public void OnClickStoreButton()
    {
        choiceUi.SetActive(false);
        storeUi.SetActive(true);
    }

    public void OnClickTalkButton()
    {
        TalkManager.instance.talkIndex = 0;

        currentQuest = null;
        TalkManager.instance.Talk(GameManager.instance.currentInteractId);

        choiceUi.SetActive(false);
        talkUi.SetActive(true);
    }

    public void OnClickQuestButton(QuestChoice questChoice)
    {
        TalkManager.instance.talkIndex = 0;

        currentQuest = questChoice.quest;
        TalkManager.instance.Talk(GameManager.instance.currentInteractId, currentQuest.questId, (int)currentQuest.questStatus);

        choiceUi.SetActive(false);
        talkUi.SetActive(true);
    }

    public void OnClickTalkBox()
    {
        if (currentQuest == null)
        {
            TalkManager.instance.Talk(GameManager.instance.currentInteractId);
        }
        else
        {
            TalkManager.instance.Talk(GameManager.instance.currentInteractId, currentQuest.questId, (int)currentQuest.questStatus);
        }
    }
}
