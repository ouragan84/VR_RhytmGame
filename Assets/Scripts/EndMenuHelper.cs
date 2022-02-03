using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndMenuHelper : MonoBehaviour
{
    public GameObject canvas;
    public float scoreMult = 1;
    private int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI GradeText;
    public GameObject newHigh;
    public GameObject notNewHigh;
    private Animator anim;
    private int maxTotalScore;
    private LevelSelector selector;
    private int levelID;
    private short difficulty;

    void Start(){
        anim = GetComponent<Animator>();
    }

    void Update(){
        scoreText.text = LevelSelector.formatScore(Mathf.FloorToInt(scoreMult * score));
    }

    public void showGrade(){
        GradeText.text = "" + calculateGrade();
    }

    public void showHighScore(){
        if(score > selector.getHighScore(levelID, difficulty)){
            newHigh.SetActive(true);
            notNewHigh.SetActive(false);
            selector.setNewHighScore(score, calculateGrade(), levelID, difficulty);
        }else{
            newHigh.SetActive(false);
            notNewHigh.SetActive(true);
            notNewHigh.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = LevelSelector.formatScore(selector.getHighScore(levelID, difficulty));
            notNewHigh.transform.Find("Grade").GetComponent<TextMeshProUGUI>().text = selector.getHighGrade(levelID, difficulty);
        }
        
        // show highscore
    }

    public string calculateGrade(){
        float percentage = 100 * score/maxTotalScore;
        if(percentage >= 110.0f) return "SS";
        if(percentage >= 100.0f) return "S";
        if(percentage >= 90.0f) return "A+";
        if(percentage >= 80.0f) return "A";
        if(percentage >= 70.0f) return "B+";
        if(percentage >= 60.0f) return "B";
        if(percentage >= 50.0f) return "C+";
        if(percentage >= 40.0f) return "C";
        if(percentage >= 30.0f) return "D+";
        if(percentage >= 20.0f) return "D";
        return "F";
    }

    public void setGradeKey(bool[] key, LevelStructure lvl){
        int obstacleCount = 0;
        foreach(short s in lvl.data){
            if(key[s])
                obstacleCount ++;
        }
        maxTotalScore = WallGenerator.maxScore * pointsTotalWithCombo(WallGenerator.maxComboMult, WallGenerator.comboStep, obstacleCount);
    }

    int pointsTotalWithCombo(float m, float s, float x){
        return floor( (x < s*m) ? s*floor(x/s)*(x/s - .5f*(1 + floor(x/s))) + x : (s*floor(m)*(m - .5f*(1 + floor(m))) + m*s) + m*x - s*m*m );
    }

    int floor(float x){
        return Mathf.FloorToInt(x);
    }

    public void Activate(int scoreSet, LevelSelector selector, int id, short diff){
        this.levelID = id;
        this.difficulty = diff;
        this.score = scoreSet;
        this.selector = selector;
        canvas.SetActive(true);
        newHigh.SetActive(false);
        notNewHigh.SetActive(false);
        anim.Play("Base Layer.EndMenuStartAnim", -1);
    }

    public void Disactivate(){
        canvas.SetActive(false);
    }
}
