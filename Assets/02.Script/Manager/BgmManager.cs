using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static BgmManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<BgmManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static BgmManager m_instance; // 싱글톤이 할당될 static 변수

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
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
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
