using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Dialog
{
    
    // Returns dialog from NPCs or objects

    [SerializeField] List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}
