using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<TalkManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static TalkManager m_instance; // �̱����� �Ҵ�� static ����

    // ���� ���� ��, GameManager���� Resource ������ ����Ǿ� �ִ� ��ȭ ����Ʈ�� �ҷ��� �ش� ��ųʸ��� �����Ѵ�.
    // �ϳ��� Key�� string�� �ƴ� string �迭�� ���� ����
    public Dictionary<int, string[]> talkDataList = new Dictionary<int, string[]>();
    public GameObject npcUi;
    public TextMeshProUGUI talkText;

    [HideInInspector]
    public int talkIndex;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }


    // ��ȭ�� ���� �Լ��̴�. �Ű������� ����Ʈ ��ȣ�� �ƴ� 0���� ������ ��쿡�� �Ϲ� ��ȭ�ϱ��̸�, ����Ʈ ��ȣ�� �Ű������� ������ ��� �ش� ����Ʈ�� �˸��� ��ȭ�� ����Ѵ�.
    public void Talk(int npcId, int questId = 0, int questIndex = 0)
    {
        //�ش� NPC�� �˸��� ��ȭ ��ũ��Ʈ�� �����´�.
        string talkData = GetTalk(npcId + questId + questIndex, talkIndex);

        // talkData �� �ΰ��� ���� ������ ��ȭ���� ������ ����̴�.
        if (talkData == null)
        {
            // ��ũ�ε����� 0���� �ʱ�ȭ���ָ�, ����Ʈ ��ȭ���� ��� �ش� ����Ʈ�� Ŭ�����Ѵ�.
            talkIndex = 0;
            npcUi.SetActive(false);

            if (questId != 0)
            {
                QuestManager.instance.EndOfQuestTalk(questId);
            }

            return;
        }

        talkText.text = talkData;
        talkIndex++;
    }

    // �˸��� ��ȭ ��ũ��Ʈ�� return ���ش�. (null : ������ ��ȭ)
    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkDataList[id].Length)
        {
            return null;
        }
        else
        {
            return talkDataList[id][talkIndex];
        }
    }
}
