using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    // �̱��� ���ٿ� ������Ƽ
    public static BgmManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<BgmManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static BgmManager m_instance; // �̱����� �Ҵ�� static ����

    public AudioSource audioSource;
    public Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    public BgmData[] bgmData;

    [System.Serializable]
    public struct BgmData
    {
        public string name;
        public AudioClip clip;
    }

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for(int i = 0; i < bgmData.Length; i++)
        {
            bgmDictionary[bgmData[i].name] = bgmData[i].clip;
        }
    }

    public void PlayBgm(string name)
    {
        audioSource.clip = bgmDictionary[name];
        audioSource.Play();
    }

    public void StopBgm()
    {
        audioSource.Stop();
    }
}
