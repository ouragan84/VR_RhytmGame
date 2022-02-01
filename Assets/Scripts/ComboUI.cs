using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboUI : MonoBehaviour
{
    //public float aplhaInit = 1f;
    //public float timeToFade = 1f;
    //private float timeStartedToFade = 0f;
    public Sprite[] Tiers;
    private Image image;
    private Animator anim;
    public TextMeshProUGUI multText;
    public TextMeshProUGUI comboText;
    public AudioClip ComboUpgradeSound;
    private AudioSource source;
    //private bool isShowing = false;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        image = GetComponent<Image>();
        anim = GetComponent<Animator>();
    }

    public void playUpgradeSound(){
        source.PlayOneShot(ComboUpgradeSound);
    }

    public void upgrade(float multiplier, bool playAnim = true){
        

        if(multiplier <= 1){
            image.sprite = Tiers[0];
            if(playAnim)
                anim.Play("Base Layer.ComboReset", -1);
        }else{
            if(playAnim)
                anim.Play("Base Layer.ComboUpgrade", -1);

            if(multiplier >= WallGenerator.maxComboMult){
                image.sprite = Tiers[3];
            }else if(multiplier >= 3){
                image.sprite = Tiers[2];
            }else if(multiplier >= 2){
                image.sprite = Tiers[1];
            }
        }

        multText.text =  "x" + multiplier.ToString("F1");
    }

    public void updateCombo(int combo){
        comboText.text = "" + combo;
    }
}
