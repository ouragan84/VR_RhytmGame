using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreOutput : MonoBehaviour
{

    public Sprite[] Tiers;
    private Image image;
    private Animator anim;
    public TextMeshProUGUI scoreText;
    public AudioClip[] TiersSounds;
    private AudioSource source;
    private Vector3 initialPos;

    void Start()
    {
        source = GetComponent<AudioSource>();
        initialPos = GetComponent<RectTransform>().position;
        image = GetComponent<Image>();
        anim = GetComponent<Animator>();
        resetTransarency();
    }

    public void ShowHit(){
        image.sprite = Tiers[4];
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        scoreText.text =  "";
        anim.Play("Base Layer.ScoreOutputAnim", -1);
        source.PlayOneShot(TiersSounds[1]);
    }

    public void resetTransarency(){
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        scoreText.color = new Color32(scoreText.faceColor.r, scoreText.faceColor.g, scoreText.faceColor.b, 0);
    }

    public void ShowScore(int score){
        if(score >= WallGenerator.perfectScore){
            image.sprite = Tiers[0];
            source.PlayOneShot(TiersSounds[0]);
        }else if(score >= 250){   //arbitrary values here
            image.sprite = Tiers[1];
            source.PlayOneShot(TiersSounds[0]);
        }else if(score >= WallGenerator.minScore){   
            image.sprite = Tiers[2]; 
            source.PlayOneShot(TiersSounds[0]);
        }else{
            image.sprite = Tiers[3];
            source.PlayOneShot(TiersSounds[1]);
        }

        scoreText.text =  "+" + score;

        GetComponent<RectTransform>().position = initialPos;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        anim.Play("Base Layer.ScoreOutputAnim", -1);
        scoreText.color = new Color32(scoreText.faceColor.r, scoreText.faceColor.g, scoreText.faceColor.b, (byte)(255));
    }
}
