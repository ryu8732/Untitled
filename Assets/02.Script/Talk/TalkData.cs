using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TalkData
{
    public int id;
    public List<string> scripts = new List<string>();

    public TalkData(int id, List<string> scripts)
    {
        this.id = id;
        this.scripts = scripts;
    }
}
