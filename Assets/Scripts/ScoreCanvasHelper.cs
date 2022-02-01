using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCanvasHelper : MonoBehaviour
{
    public ComboUI comboUI;
    public ScoreOutput scoreOutput;
    public GameObject canvas;

    public void updateCombo(int combo){
        comboUI.updateCombo(combo);
    }

    public void upgrade(float comboMultiplier, bool playAnim = true){
        comboUI.upgrade(comboMultiplier, playAnim);
    }

    public void ShowHit(){
        scoreOutput.ShowHit();
    }

    public void ShowScore(int scoreIncrease){
        scoreOutput.ShowScore(scoreIncrease);
    }

    public void Activate(){
        canvas.SetActive(true);
    }

    public void Disactivate(){
        scoreOutput.resetTransarency();
        canvas.SetActive(false);
    }
}
