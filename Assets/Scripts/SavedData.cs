using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData
{
    public int EnvironementID;
    //add everything related to high scores there

    public void ResetData(){
        EnvironementID = 0;
    }
}
