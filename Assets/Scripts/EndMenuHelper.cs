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
    private Animator anim;

    void Start(){
        anim = GetComponent<Animator>();
    }

    void Update(){
        scoreText.text = Mathf.FloorToInt(scoreMult * score ).ToString();
    }

    public void Activate(int scoreSet){
        this.score = scoreSet;
        canvas.SetActive(true);
        anim.Play("Base Layer.EndMenuStartAnim", -1);
    }

    public void Disactivate(){
        canvas.SetActive(false);
    }
}
