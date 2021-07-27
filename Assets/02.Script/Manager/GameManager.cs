using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


// 점수와 게임 오버 여부를 관리하는 게임 매니저
public class GameManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    [Header("Main Canvas")]
    public GameObject mainCanvas;
    public GameObject quitPanel;
    public GameObject diePanel;
    public GameObject loadingPanel;
    public Slider loadingProgressBar;
    public TextMeshProUGUI loadingValueText;
    public GameObject npcUi;

    [Header("Joystick Controller")]
    public JoystickController[] joysticks;

    [Header("Camera")]
    public FollowCam followCam;

    [Header("Interact elements")]
    public int currentInteractId;

    [Header("Player & Character Requirements")]
    public Transform playerSpawnPos;
    public Transform characterList;

    [HideInInspector]
    public GameObject player;
    private string selectedCharacter;

    [Header("Player Datas"), HideInInspector]
    public PlayerStatement playerStatement;

    public Dictionary<int, GameObject> npcList;


    [Header("All Npcs Parent Obj")]
    public GameObject npcObjs;

    [Header("LevelUp UI")]
    public GameObject levelUpObj;
    public TextMeshProUGUI levelText;

    [HideInInspector]
    public bool isRespawn = false;
    public string prevSceneName;
    public string currentSceneName;
    public Vector3 currentPosition;
    public Quaternion currentRotation;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        npcList = new Dictionary<int, GameObject>();
        selectedCharacter = "Knight";

        PlayerInit();
        mainCanvas.SetActive(false);
    }

    private void Start()
    {
        BgmManager.instance.PlayBgm("Title");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void PlayerInit()
    {
        player = characterList.Find(selectedCharacter).gameObject;

        playerStatement = player.GetComponent<PlayerStatement>();
        playerStatement.inventory = new Inventory();
        InventoryManager.instance.SetInventory(playerStatement.inventory);
        playerStatement.equipItemList = new Dictionary<Item.ItemType, Item>();

        playerStatement.SetComponents();
        playerStatement.PotionTextRefresh();
    }

    public void LoadScene(string nextSceneName)
    {
        loadingProgressBar.value = 0f;
        loadingValueText.text = "Loading... 0%";

        BgmManager.instance.StopBgm();
        loadingPanel.SetActive(true);
        mainCanvas.SetActive(true);

        prevSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneCoroutine(nextSceneName));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        BgmManager.instance.PlayBgm(currentSceneName);

        if(isRespawn)
        {
            playerSpawnPos = GameObject.Find("Start").transform;
            isRespawn = false;
        }

        else
        {
            if (prevSceneName == "Title")
            {
                playerSpawnPos = new GameObject().transform;
                playerSpawnPos.position = currentPosition;
                playerSpawnPos.rotation = currentRotation;
            }
            else
            {
                playerSpawnPos = GameObject.Find(prevSceneName).transform;
            }
        }

        player.transform.position = playerSpawnPos.position;
        player.transform.rotation = playerSpawnPos.rotation;

        followCam.SetPosToPlayerPos();
        player.SetActive(true);

        npcList.Clear();
        if ((npcObjs = GameObject.Find("NPCs")) != null)
        {
            for (int i = 0; i < npcObjs.transform.childCount; i++)
            {
                GameObject npcObj = npcObjs.transform.GetChild(i).gameObject;
                npcList[npcObj.GetComponent<NpcInfo>().npcId] = npcObj;
            }

            QuestManager.instance.questChecker();
        }

        foreach (JoystickController joystick in joysticks)
        {
            joystick.DragEnd();
        }

        loadingPanel.SetActive(false);
    }

    IEnumerator LoadSceneCoroutine(string nextSceneName)
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName); 
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while (!op.isDone) { 
            yield return null; 
            timer += Time.deltaTime; 
            if (op.progress < 0.9f) {
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, op.progress, timer);
                if (loadingProgressBar.value >= op.progress)
                {
                    timer = 0f;
                }
            } 
            else {
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, 1f, timer);
                if (loadingProgressBar.value == 1.0f)
                {
                    op.allowSceneActivation = true;
                }
            }
            loadingValueText.text = "Loading..." + ((int)(loadingProgressBar.value * 100)).ToString() + "%";
        }
    }

    public void GetExp(int exp)
    {
        playerStatement.exp += exp;

        int count = 0;

        while(playerStatement.level + count < DataManager.instance.playerStatDict.Count && DataManager.instance.playerStatDict[playerStatement.level + count].exp <= playerStatement.exp)
        {
            count++;
        }

        if (count != 0)
        {
            LevelUp(count);
        }
    }

    private void LevelUp(int count)
    {
        playerStatement.level += count;
        playerStatement.baseMaxHealth = DataManager.instance.playerStatDict[playerStatement.level].health;
        playerStatement.baseMaxMana = DataManager.instance.playerStatDict[playerStatement.level].mana;
        playerStatement.baseManaRegeneration = DataManager.instance.playerStatDict[playerStatement.level].manaRegeneration;
        playerStatement.baseDamage = DataManager.instance.playerStatDict[playerStatement.level].damage;

        playerStatement.ApplyItems();

        levelText.text = playerStatement.level.ToString();
        levelUpObj.SetActive(true);

        StartCoroutine(DisappearLevelUpObj());
        Debug.Log("Level Up !! : " + playerStatement.level);
    }

    private IEnumerator DisappearLevelUpObj()
    {
        yield return new WaitForSeconds(2.0f);

        levelUpObj.SetActive(false);
    }


    public void DieEnable()
    {
        diePanel.SetActive(true);
        Debug.Log("EndGame");
    }

    public void OnRespawnButtonClicked()
    {
        playerStatement.ResetPlayerState();

        diePanel.SetActive(false);
        isRespawn = true;
        LoadScene("Town");
    }

    public void OnQuitYesButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}