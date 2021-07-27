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
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<TalkManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static TalkManager m_instance; // 싱글톤이 할당될 static 변수

    // 게임 시작 시, GameManager에서 Resource 폴더에 저장되어 있는 대화 리스트를 불러와 해당 딕셔너리에 저장한다.
    // 하나의 Key당 string이 아닌 string 배열이 들어감에 주의
    public Dictionary<int, string[]> talkDataList = new Dictionary<int, string[]>();
    public GameObject npcUi;
    public TextMeshProUGUI talkText;

    [HideInInspector]
    public int talkIndex;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }


    // 대화를 위한 함수이다. 매개변수가 퀘스트 번호가 아닌 0으로 들어왔을 경우에는 일반 대화하기이며, 퀘스트 번호가 매개변수로 들어왔을 경우 해당 퀘스트에 알맞은 대화를 출력한다.
    public void Talk(int npcId, int questId = 0, int questIndex = 0)
    {
        //해당 NPC의 알맞은 대화 스크립트를 가져온다.
        string talkData = GetTalk(npcId + questId + questIndex, talkIndex);

        // talkData 가 널값인 경우는 마지막 대화까지 마쳤을 경우이다.
        if (talkData == null)
        {
            // 토크인덱스를 0으로 초기화해주며, 퀘스트 대화였을 경우 해당 퀘스트를 클리어한다.
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

    // 알맞은 대화 스크립트를 return 해준다. (null : 마지막 대화)
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
