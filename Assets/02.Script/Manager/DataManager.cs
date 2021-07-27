using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static DataManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<DataManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static DataManager m_instance; // 싱글톤이 할당될 static 변수

    [Serializable]
    public class Serialization<TKey, TValue>
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> GetDictionary() { return target; }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }
    }

    string jsonPath;

    public PlayerData playerData;
    //public QuestDataList questDataList;
    public TalkDataList talkDataList;

    public Dictionary<int, Item> itemDict;
    public Dictionary<int, PlayerStat> playerStatDict;
    public Dictionary<int, EnemyData> enemyDict;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        jsonPath = Application.persistentDataPath;

        playerData = new PlayerData();
        //questDataList = new QuestDataList();
        talkDataList = new TalkDataList();

        itemDict = new Dictionary<int, Item>();
        enemyDict = new Dictionary<int, EnemyData>();
        playerStatDict = new Dictionary<int, PlayerStat>();
    }

    private void Start()
    {
        // Json 파일을 읽어 모든 퀘스트, 대화 리스트를 로드한다.
        LoadItemListFromJson();
        LoadQuestListFromJson();
        LoadTalkListFromJson();
        LoadExpListFromJson();
        LoadEnemyListFromJson();

        LoadPlayerDataFromJson();
    }

    // 현재 플레이어 데이터를 Json 파일에 저장한다.
    public void SavePlayerDataToJson(bool isExist = true)
    {
        if (isExist)
        {
            playerData.level = GameManager.instance.playerStatement.level;
            playerData.health = GameManager.instance.playerStatement.health;
            playerData.baseMaxHealth = GameManager.instance.playerStatement.baseMaxHealth;
            playerData.mana = GameManager.instance.playerStatement.mana;
            playerData.baseMaxMana = GameManager.instance.playerStatement.baseMaxMana;
            playerData.baseManaRegeneration = GameManager.instance.playerStatement.baseManaRegeneration;
            playerData.baseDamage = GameManager.instance.playerStatement.baseDamage;
            playerData.baseCriticalChance = GameManager.instance.playerStatement.baseCriticalChance;

            playerData.exp = GameManager.instance.playerStatement.exp;
            playerData.gold = GameManager.instance.playerStatement.inventory.gold;

            playerData.hasItemList = GameManager.instance.playerStatement.inventory.GetItemList();
            playerData.equipItemList = GameManager.instance.playerStatement.equipItemList.Values.ToList();

            playerData.questContainer = QuestManager.instance.questContainer;
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = GameManager.instance.currentSceneName;
            playerData.currentPosition = GameManager.instance.player.transform.position;
            playerData.currentRotation = GameManager.instance.player.transform.rotation;
        }

        else
        {
            GameManager.instance.isRespawn = true;
            playerData.level = 1;
            playerData.baseMaxHealth = playerStatDict[1].health;
            playerData.health = playerData.baseMaxHealth;
            playerData.baseMaxMana = playerStatDict[1].mana;
            playerData.mana = playerData.baseMaxMana;
            playerData.baseManaRegeneration = playerStatDict[1].manaRegeneration;
            playerData.baseDamage = playerStatDict[1].damage;
            playerData.baseCriticalChance = 0f;
            playerData.questContainer.activeQuests[10] = QuestManager.instance.allQuests[10];
            playerData.questContainer.DictionaryToList();

            playerData.currentSceneName = "Town";
        }

        string fileName = "playerData.json";
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(jsonPath + "/" + fileName, jsonData);
    }

    // Json 파일의 플레이어 데이터로부터 게임상의 플레이어 데이터에 불러온다.
    public void LoadPlayerDataFromJson()
    {
        string fileName = "playerData.json";

        if (!File.Exists(jsonPath + "/" + fileName))
        {
            SavePlayerDataToJson(false);
        }

        string jsonData = File.ReadAllText(jsonPath + "/" + fileName);
        playerData = JsonUtility.FromJson<PlayerData>(jsonData);

        GameManager.instance.playerStatement.level = playerData.level;
        GameManager.instance.playerStatement.health = playerData.health;
        GameManager.instance.playerStatement.mana = playerData.mana;

        GameManager.instance.playerStatement.baseMaxHealth = playerData.baseMaxHealth;
        GameManager.instance.playerStatement.baseMaxMana = playerData.baseMaxMana;
        GameManager.instance.playerStatement.baseManaRegeneration = playerData.baseManaRegeneration;
        GameManager.instance.playerStatement.baseDamage = playerData.baseDamage;
        GameManager.instance.playerStatement.baseCriticalChance = playerData.baseCriticalChance;

        GameManager.instance.playerStatement.inventory.itemList = playerData.hasItemList;

        QuestManager.instance.questContainer = playerData.questContainer;
        QuestManager.instance.questContainer.ListToDictionary();

        for (int i = 0; i < playerData.equipItemList.Count; i++)
        {
            GameManager.instance.playerStatement.EquipItem(playerData.equipItemList[i]);
        }

        GameManager.instance.playerStatement.ApplyItems();

        GameManager.instance.playerStatement.healthSlider.value = GameManager.instance.playerStatement.health;
        GameManager.instance.playerStatement.manaSlider.value = GameManager.instance.playerStatement.mana;

        GameManager.instance.playerStatement.exp = playerData.exp;
        GameManager.instance.playerStatement.inventory.gold = playerData.gold;

        GameManager.instance.currentSceneName = playerData.currentSceneName;
        GameManager.instance.currentPosition = playerData.currentPosition;
        GameManager.instance.currentRotation = playerData.currentRotation;
    }

    // Json 파일의 아이템 리스트에서 모든 아이템 데이터를 불러와 allItemList에 저장한다. 이미 아이템 자체에 itemNo이 존재하지만, 검색의 용이성을 위해 itemNo을 키값으로 하는 딕셔너리로 저장하였다.
    public void LoadItemListFromJson()
    {
        string fileName = "ItemList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        ItemList temp = JsonUtility.FromJson<ItemList>(jsonData.ToString());


        for (int i = 0; i < temp.itemList.Count; i++)
        {
            itemDict[temp.itemList[i].itemNo] = temp.itemList[i];
        }
    }

    // Json 파일의 퀘스트 리스트에서 모든 퀘스트 데이터를 불러와 저장한다.
    public void LoadQuestListFromJson()
    {
        string fileName = "QuestList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        QuestList questList = JsonUtility.FromJson<QuestList>(jsonData.ToString());

        foreach (Quest quest in questList.quests)
        {
            QuestManager.instance.allQuests[quest.questId] = quest;
        }
    }

    // Json 파일의 대화 리스트에서 모든 대화 데이터를 불러와 talkDataList에 저장한다. talkManager에서 해당 대화를 딕셔너리 형태로 활용하기 때문에 talkManager의 talkData에도 퀘스트 데이터를 딕셔너리 형태로 저장해준다.
    public void LoadTalkListFromJson()
    {
        string fileName = "TalkList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        talkDataList = JsonUtility.FromJson<TalkDataList>(jsonData.ToString());

        foreach (TalkData talkData in talkDataList.talkDatas)
        {
            TalkManager.instance.talkDataList.Add(talkData.id, talkData.scripts.ToArray());
        }
    }

    public void LoadExpListFromJson()
    {
        string fileName = "PlayerStatTable";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        PlayerStatTable temp = JsonUtility.FromJson<PlayerStatTable>(jsonData.ToString());

        for (int i = 0; i < temp.playerStatTable.Count; i++)
        {
            playerStatDict[i + 1] = temp.playerStatTable[i];
        }
    }

    public void LoadEnemyListFromJson()
    {
        string fileName = "EnemyList";

        TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
        EnemyDataList temp = JsonUtility.FromJson<EnemyDataList>(jsonData.ToString());

        for (int i = 0; i < temp.enemyList.Count; i++)
        {
            enemyDict[temp.enemyList[i].enemyId] = temp.enemyList[i];
        }
    }
}
