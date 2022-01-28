using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

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
    private float nextActionTime = 1.0f;
    public float period = 1f;
    private AudioSource source;
    private int Score = 0;
    public XRController RightController;
    public XRController LeftController;
    public ComboUI comboUI;
    public bool followCurve = true;
    public AnimationCurve speedMultiplier;
    public InputHelpers.Button pauseButton;
    private bool wasPausedButtonPressedLastUpdate = false;
    public GameObject PauseMenu;
    

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(false);

        source = GetComponent<AudioSource>();
        ObstacleInterface.generator = this;
        source.PlayOneShot(Music);
        
        ObstacleInterface.VRRight = VRRight;
        ObstacleInterface.VRLeft = VRLeft;
        ObstacleInterface.VRHead = VRHead;
        ObstacleInterface.speed = speed;
        ObstacleInterface.speedMultiplier = speedMultiplier;
        ObstacleInterface.followCurve = followCurve;
    }

    
    
    void Update () {
        if (Time.time >= nextActionTime ) {
           nextActionTime = Time.time + period;
           GameObject obj = Instantiate(Walls[(int)Random.Range(0,Walls.Length)], transform.position, transform.rotation);
        }

        checkForPause();
    }

    void FixedUpdate(){
        if(Obstacle.isPaused){
            Obstacle.totalTimePaused += Time.fixedDeltaTime;
            nextActionTime += Time.fixedDeltaTime;
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

    static int DistanceToScore(float dis){
        return (dis <= 8.0f? 1000 : (dis > 35.0f? 0 : Mathf.FloorToInt(vertexPointToQuad(35.0f, 50, 8.0f, 500, dis)))); 
    }

    static float vertexPointToQuad(float vx, float vy, float x, float y, float t){
        return (y-vy)/Mathf.Pow(x-vx, 2) * Mathf.Pow(t-vx, 2) + vy;
    }

    void addCombo(int s, bool playAnim = true){
        if(s <= 0){
            combo = 0;
            comboMultiplier = 1;
            comboUI.upgrade(comboMultiplier, playAnim);
        }else{
            combo += 1;
            if(comboMultiplier < 5f && Mathf.Floor(combo/4) + 1 != comboMultiplier){
                comboMultiplier = Mathf.Floor(combo/4) + 1;
                comboUI.upgrade(comboMultiplier, playAnim);
            }
        }
    }


    public void restart(){
        foreach(ObstacleInterface w in FindObjectsOfType<ObstacleInterface>()){
            Destroy(w.gameObject);
        }
        nextActionTime = Time.time;
        source.Stop();
        source.PlayOneShot(Music);
        Score = 0;
        scoreTotal.text = "SCORE:\n" + Score;
        addCombo(0, false);
        comboUI.updateCombo(combo);
        ResumeGame();
    }

    void checkForPause(){
        //InputDevices.GetDeviceAtXRNode(RightController.controllerNode).TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuPressedRight);
        InputHelpers.IsPressed(LeftController.inputDevice, pauseButton, out bool isMenuPressedLeft, .1f);
        InputHelpers.IsPressed(LeftController.inputDevice, pauseButton, out bool isMenuPressedRight, .1f);

        if(!wasPausedButtonPressedLastUpdate && (isMenuPressedLeft || isMenuPressedRight)){
            wasPausedButtonPressedLastUpdate = true;
            if(!ObstacleInterface.isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        wasPausedButtonPressedLastUpdate =  (isMenuPressedLeft || isMenuPressedRight);
    }

    public void PauseGame(){
        //Time.timeScale = 0.0f;
        Obstacle.isPaused = true;
        source.Pause();
        PauseMenu.SetActive(true);
    }

    public void ResumeGame(){
        //Time.timeScale = 1.0f;
        Obstacle.isPaused = false;
        source.UnPause();
        PauseMenu.SetActive(false);
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
