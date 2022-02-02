using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelSelectButton : MonoBehaviour
{
    public static LevelSelector menuRef;
    private int id;

    public void setId(int newID){
        id = newID;
    }

    public void onHoverEnter(){
        menuRef.playPreview(id);
    }

    public void onHoverExit(){
        menuRef.stopPreview(id);
    }

    public void onClick(){
        menuRef.selectLevel(id);
    }
}
