using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public float requireMana;
    public float skillCooltime;
    public GameObject skillArea;
    public string coroutineName;


    public Skill(string skillName, float requireMana, float skillCooltime, GameObject skillArea, string coroutineName)
    {
        this.skillName = skillName;
        this.requireMana = requireMana;
        this.skillCooltime = skillCooltime;
        this.skillArea = skillArea;
        this.coroutineName = coroutineName;
    }
}
