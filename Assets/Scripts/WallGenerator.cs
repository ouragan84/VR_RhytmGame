using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using System.IO;

public class WallGenerator : MonoBehaviour
{
    public Vector3 speed;
    public GameObject[] Walls;
    public Transform VRHead;
    public Transform VRLeft;
    public Transform VRRight;
    public TextMeshProUGUI scoreTotal;
    public ScoreOutput scoreOutput;
    public XRController RightController;
    public XRController LeftController;
    public ComboUI comboUI;
    public bool followCurve = true;
    public AnimationCurve speedMultiplier;
    public InputHelpers.Button pauseButton;
    public GameObject PauseMenu;
    public float additionalHeight = 0.10f;
    public float initialHeightOfPanels = 1.75f;
    public InputHelpers.Button resetHeightButton;
    public TextAsset levelFile;
    public bool isRandom = false;

    private bool wasPausedButtonPressedLastUpdate = false;
    private int combo = 0;
    private float comboMultiplier = 0;
    private float songTimeElapsed = -1.0f;
    private AudioSource source;
    private int Score = 0;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private short currentLevel;
    private Levels levels;
    private AudioClip currentSong;
    private LevelStructure levelStructure;
    private int lastIndexBuilt = -1;
    

    // Start is called before the first frame update
    void Start()
    {
        loadLevels();
        loadAssetsFromLevel(0, "easy");

        PauseMenu.SetActive(false);

        source = GetComponent<AudioSource>();
        ObstacleInterface.generator = this;
        source.PlayOneShot(currentSong);
        
        ObstacleInterface.VRRight = VRRight;
        ObstacleInterface.VRLeft = VRLeft;
        ObstacleInterface.VRHead = VRHead;
        ObstacleInterface.speed = speed;
        ObstacleInterface.speedMultiplier = speedMultiplier;
        ObstacleInterface.followCurve = followCurve;

        initialScale = transform.localScale;
        initialPosition = transform.position;

        //readFileLevel(levelFilePaths[currentLevel]);
    }
    
    void Update () {

        if(Obstacle.isPaused){
            Obstacle.totalTimePaused += Time.deltaTime;
        }else{
            songTimeElapsed += Time.deltaTime;
        }

        if (Mathf.FloorToInt(songTimeElapsed / levelStructure.interval) > lastIndexBuilt ) {
            lastIndexBuilt = Mathf.FloorToInt(songTimeElapsed / levelStructure.interval);

            GameObject obj;
            if(isRandom)
                obj = Instantiate(Walls[(int)Random.Range(0,Walls.Length)], initialPosition, transform.rotation);
            else{
                int id = levelStructure.data[lastIndexBuilt] - 1;
                if(id >= 0)
                    obj = Instantiate(Walls[id], initialPosition, transform.rotation);
            }
                
        }

        checkForInputs();
    }

    public void loadLevels(){
        levels = JsonUtility.FromJson<Levels>(levelFile.text);
    }

    public void loadAssetsFromLevel(short lvl, string difficulty){
        currentLevel = lvl;
        currentSong = Resources.Load<AudioClip>(levels.levels[currentLevel].song_path);
        Debug.Log(levels.levels[currentLevel].song_path);

        foreach (LevelStructure x in levels.levels[currentLevel].level_structure)
        {
            if (x.difficulty.Equals(difficulty))
            {
                levelStructure = x;
                break;
            }
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
        songTimeElapsed = -1.0f;
        lastIndexBuilt = -1;
        source.Stop();
        source.PlayOneShot(currentSong);
        Score = 0;
        scoreTotal.text = "SCORE:\n" + Score;
        addCombo(0, false);
        comboUI.updateCombo(combo);
        ResumeGame();
    }

    void checkForInputs(){
        //InputDevices.GetDeviceAtXRNode(RightController.controllerNode).TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuPressedRight);
        InputHelpers.IsPressed(LeftController.inputDevice, pauseButton, out bool isMenuPressedLeft, .1f);
        InputHelpers.IsPressed(RightController.inputDevice, pauseButton, out bool isMenuPressedRight, .1f);

        if(!wasPausedButtonPressedLastUpdate && (isMenuPressedLeft || isMenuPressedRight)){
            wasPausedButtonPressedLastUpdate = true;
            if(!ObstacleInterface.isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        InputHelpers.IsPressed(LeftController.inputDevice, resetHeightButton, out bool isResetPressedLeft, .1f);
        InputHelpers.IsPressed(RightController.inputDevice, resetHeightButton, out bool isResetPressedRight, .1f);

        if(!wasPausedButtonPressedLastUpdate && (isResetPressedLeft && isResetPressedRight)){
            wasPausedButtonPressedLastUpdate = true;
            ResetHeight();
        }

        wasPausedButtonPressedLastUpdate =  (isMenuPressedLeft || isMenuPressedRight) || (isResetPressedLeft && isResetPressedRight);
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

    public void ResetHeight(){
        float playerHeight = FindObjectOfType<XROrigin>().CameraInOriginSpaceHeight + additionalHeight;
        if(playerHeight > 2.5f) playerHeight = 2.5f;
        if(playerHeight < 1.0f) playerHeight = 1.2f;
        
        transform.localScale = initialScale * (playerHeight / initialHeightOfPanels);
        transform.position = new Vector3(initialPosition.x, initialPosition.y*(transform.localScale.y/initialScale.y), initialPosition.z);
    }

    public void QuitLevel(){
        Application.Quit();
    }
}
