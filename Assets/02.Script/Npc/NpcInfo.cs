using UnityEngine;

public class NpcInfo : MonoBehaviour
{
    [Header ("Npc Datas")]
    public int npcId;
    public string npcName;

    [Header("Npc Elements")]
    public bool hasTalk, hasStore;

    [Header("Npc Detect Player Area")]
    public GameObject interactArea;

    [Header("Quest Marks")]
    public GameObject questionMark;
    public GameObject exclamationMark;

    private void OnEnable()
    {
        // ��ȣ�ۿ� �������� �����ص� ������Ʈ�� �̺�Ʈ�� ����Ѵ�. (OnCollisionEnter / Exit �̺�Ʈ�� ���� ����)
        interactArea.GetComponent<InteractArea>().CollisionEnterEvent += InteractAreaEnter;
        interactArea.GetComponent<InteractArea>().CollisionExitEvent += InteractAreaExit;
    }

    private void InteractAreaEnter(Collider collider)
    {
        GameManager.instance.npcUi.SetActive(true);

        GameManager.instance.currentInteractId = npcId;
        QuestManager.instance.ShowNpcQuestList(npcId);
        GameManager.instance.npcUi.GetComponent<NpcUI>().SetChoiceUi(hasTalk, hasStore);
    }

    private void InteractAreaExit(Collider collider)
    {
        GameManager.instance.npcUi.SetActive(false);
    }
}
