using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class WallGenerator : MonoBehaviour
{
    public Vector3 speed;
    public GameObject[] Walls;
    public Transform VRHead;
    public Transform VRLeft;
    public Transform VRRight;
    public AudioClip Music;
    public TextMeshProUGUI scoreTotal;
    public ScoreOutput scoreOutput;
    private int combo = 0;
    private float comboMultiplier = 0;
    private float nextActionTime = 0.0f;
    public float period = 1f;
    private AudioSource source;
    private int Score = 0;
    public XRController RightController;
    public XRController LeftController;
    public ComboUI comboUI;
    public bool followCurve = true;
    public AnimationCurve speedMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        WallDance.generator = this;
        Obstacle.generator = this;
        source.PlayOneShot(Music);
        
        WallDance.VRRight = VRRight;
        WallDance.VRLeft = VRLeft;
        WallDance.VRHead = VRHead;
        WallDance.speed = speed;
        WallDance.speedMultiplier = speedMultiplier;
        WallDance.followCurve = followCurve;

        Obstacle.speed = speed;
        Obstacle.VRRight = VRRight;
        Obstacle.VRLeft = VRLeft;
        Obstacle.VRHead = VRHead;
        Obstacle.speedMultiplier = speedMultiplier;
        Obstacle.followCurve = followCurve;
    }

    
    
    void Update () {
        if (Time.time >= nextActionTime ) {
           nextActionTime = Time.time + period;
           GameObject obj = Instantiate(Walls[(int)Random.Range(0,Walls.Length)], transform.position, transform.rotation);
        }
    }

    public void addScore(float headDis, float rightDis, float leftDis){

        float totalDis = (new Vector3(headDis, rightDis, leftDis)).magnitude;
        int scoreIncrease = DistanceToScore(totalDis);
        addCombo(scoreIncrease);
        Score += Mathf.FloorToInt(scoreIncrease * comboMultiplier);

        comboUI.updateCombo(combo);

        scoreFeedback(rightDis, leftDis);

        scoreTotal.text = "SCORE:\n" + Score;
        scoreOutput.ShowScore(scoreIncrease);
    }

    int DistanceToScore(float dis){
        return (dis <= 8.0f? 1000 : (dis > 35.0f? 0 : Mathf.FloorToInt(vertexPointToQuad(8.0f, 500, 35.0f, 50, dis)))); 
    }

    float vertexPointToQuad(float vx, float vy, float x, float y, float t){
        return (y-vy)/Mathf.Pow(x-vx, 2) * Mathf.Pow(t-vx, 2) + vy;
    }

    void addCombo(int s){
        if(s <= 0){
            combo = 0;
            comboMultiplier = 1;
            comboUI.upgrade(comboMultiplier);
        }else{
            combo += 1;
            if(comboMultiplier < 5f && Mathf.Floor(combo/4) + 1 != comboMultiplier){
                comboMultiplier = Mathf.Floor(combo/4) + 1;
                comboUI.upgrade(comboMultiplier);
            }
        }
    }


    public void restart(){
        foreach(WallDance w in FindObjectsOfType<WallDance>()){
            Destroy(w.gameObject);
        }
        foreach(Obstacle o in FindObjectsOfType<Obstacle>()){
            Destroy(o.gameObject);
        }
        nextActionTime = Time.time;
        source.Stop();
        source.PlayOneShot(Music);
        Score = 0;
        scoreTotal.text = "SCORE:\n" + Score;
        
    }

    void scoreFeedback(float rightDis, float leftDis){
        if(rightDis >= 15f){
            hapticFeedback(true, rightDis > 30f ? .5f : lineTwoPoints(15f, .1f, 30f, .5f, rightDis), 0.2f);
        }

        if(leftDis >= 15f){
            hapticFeedback(false, leftDis > 30f ? .5f : lineTwoPoints(15f, .1f, 30f, .5f, leftDis), 0.2f);
        }
    }

    static float lineTwoPoints(float x1, float y1, float x2, float y2, float t){
        return (y2-y1)/(x2-x1) * (t-x1) + y1;
    }

    public void hapticFeedback(bool isRightController, float intensity, float duration){
        if(isRightController){
            RightController.SendHapticImpulse(intensity, duration);
        }else{
            LeftController.SendHapticImpulse(intensity, duration);
        }
    }

    public void TakeDamage(bool head, bool right, bool left){
        hapticFeedback(true, 0.4f, 0.3f);
        hapticFeedback(false, 0.4f, 0.3f);
        addCombo(0);
        comboUI.updateCombo(combo);
        scoreOutput.ShowHit();
    }
}
