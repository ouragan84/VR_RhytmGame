using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using System.IO;
using UnityEngine.UI;

public class WallGenerator : MonoBehaviour
{
    public Vector3 speed;
    public GameObject[] Walls;
    public Transform VRHead;
    public Transform VRLeft;
    public Transform VRRight;
    public TextMeshProUGUI scoreTotal;
    public ScoreCanvasHelper scoreOut;
    public XRController RightController;
    public XRController LeftController;
    public bool followCurve = true;
    public AnimationCurve speedMultiplier;
    public InputHelpers.Button pauseButton;
    public GameObject PauseMenu;
    public float additionalHeight = 0.10f;
    public float initialHeightOfPanels = 1.75f;
    public InputHelpers.Button resetHeightButton;
    public bool isRandom = false;
    public Image[] SongCover;
    public TextMeshProUGUI[] SongTitle;
    public TextMeshProUGUI[] AuthorName;
    public EndMenuHelper EndMenu;
    public AudioClip endAudio;

    public static int comboStep = 4;
    public static float maxComboMult = 5f;
    public static float lowDis = 10.0f;
    public static float highDis = 40.0f;
    public static int minScore = 50;
    public static int maxScore = 500;
    public static int perfectScore = 1000;
    public static Levels levels;

    private bool wasPausedButtonPressedLastUpdate = false;
    private int combo = 0;
    private float comboMultiplier = 0;
    private float songTimeElapsed = -1.0f;
    private AudioSource source;
    private int Score = 0;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private short currentLevel;
    private AudioClip currentSong;
    private Sprite currentSongIcon;
    private LevelStructure levelStructure;
    private int lastIndexBuilt = -1; 
    private bool hasLevelStarted = false;
    

    // Start is called before the first frame update
    void Start(){
        ObstacleInterface.generator = this;
        ObstacleInterface.VRRight = VRRight;
        ObstacleInterface.VRLeft = VRLeft;
        ObstacleInterface.VRHead = VRHead;
        ObstacleInterface.speed = speed;
        ObstacleInterface.speedMultiplier = speedMultiplier;
        ObstacleInterface.followCurve = followCurve;

        source = GetComponent<AudioSource>();

        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    public void StartLevel(short lvl, string difficulty){
        loadAssetsFromLevel(lvl, difficulty); //arbitrary values here

        PauseMenu.SetActive(false);
        EndMenu.Disactivate();
        scoreOut.Activate();

        source.PlayOneShot(currentSong);

        foreach(Image i in SongCover){
            i.sprite = currentSongIcon;
        }
        foreach(TextMeshProUGUI t in SongTitle){
            t.text = levels.levels[currentLevel].name;
        }
        foreach(TextMeshProUGUI t in AuthorName){
            t.text = levels.levels[currentLevel].author;
        }

        hasLevelStarted = true;
    }
    
    void Update(){
        if(hasLevelStarted){
            if(Obstacle.isPaused){
                Obstacle.totalTimePaused += Time.deltaTime;
            }else{
                songTimeElapsed += Time.deltaTime;
            }

            if (lastIndexBuilt < levelStructure.data_length-1 && Mathf.FloorToInt(songTimeElapsed / levelStructure.interval) > lastIndexBuilt) {
                lastIndexBuilt = Mathf.FloorToInt(songTimeElapsed / levelStructure.interval);
                GameObject obj = null;

                if(isRandom)
                    obj = Instantiate(Walls[(int)Random.Range(0,Walls.Length)], initialPosition, transform.rotation);
                else{
                    int id = levelStructure.data[lastIndexBuilt] - 1;
                    if(id >= 0)
                        obj = Instantiate(Walls[id], initialPosition, transform.rotation);
                }

                if(obj != null && lastIndexBuilt >= levelStructure.data_length-1)
                    obj.GetComponent<ObstacleInterface>().isLast = true;
            }
        }
        
        checkForInputs();
    }

    public void loadAssetsFromLevel(short lvl, string difficulty){
        currentLevel = lvl;
        currentSong = Resources.Load<AudioClip>(levels.levels[currentLevel].song_path);
        currentSongIcon = Resources.Load<Sprite>(levels.levels[currentLevel].song_icon_path);
        isRandom = levels.levels[currentLevel].isRandom;

        foreach (LevelStructure x in levels.levels[currentLevel].level_structure)
        {
            if (x.difficulty.Equals(difficulty))
            {
                levelStructure = x;
                break;
            }
        }

        EndMenu.setGradeKey(levels.key, levelStructure);
    }

    public IEnumerator EndLevel(float time)
    {
        yield return new WaitForSeconds(time);

        source.PlayOneShot(endAudio);
        EndMenu.Activate(Score);
        scoreOut.Disactivate();
    }

    public void addScore(float headDis, float rightDis, float leftDis){

        float totalDis = (new Vector3(headDis, rightDis, leftDis)).magnitude;
        int scoreIncrease = DistanceToScore(totalDis);
        addCombo(scoreIncrease);
        Score += Mathf.FloorToInt(scoreIncrease * comboMultiplier);

        scoreOut.updateCombo(combo);

        scoreFeedback(rightDis, leftDis);

        scoreTotal.text = "SCORE:\n" + Score;
        scoreOut.ShowScore(scoreIncrease);
    }

    static int DistanceToScore(float dis){
        return (dis <= lowDis? perfectScore : (dis > highDis? 0 : Mathf.FloorToInt(vertexPointToQuad(highDis, minScore, lowDis, maxScore, dis)))); //arbitrary values here
    }

    static float vertexPointToQuad(float vx, float vy, float x, float y, float t){
        return (y-vy)/Mathf.Pow(x-vx, 2) * Mathf.Pow(t-vx, 2) + vy;
    }
    
    void addCombo(int s, bool playAnim = true){
        if(s <= 0){
            combo = 0;
            comboMultiplier = 1;
            scoreOut.upgrade(comboMultiplier, playAnim);
        }else{
            combo += 1;
            if(comboMultiplier < maxComboMult && Mathf.Floor(combo/comboStep) + 1 != comboMultiplier){ //arbitrary values here
                comboMultiplier = Mathf.Floor(combo/comboStep) + 1;                          //arbitrary values here
                scoreOut.upgrade(comboMultiplier, playAnim);
            }
        }
    }

    public void restart(){
        foreach(ObstacleInterface w in FindObjectsOfType<ObstacleInterface>()){
            Destroy(w.gameObject);
        }
        songTimeElapsed = -1.0f; //arbitrary values here
        lastIndexBuilt = -1;
        source.Stop();
        source.PlayOneShot(currentSong);
        Score = 0;
        scoreTotal.text = "SCORE:\n" + Score;
        addCombo(0, false);
        scoreOut.updateCombo(combo);
        EndMenu.Disactivate();
        scoreOut.Activate();
        ResumeGame();
    }

    void checkForInputs(){
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
        if(hasLevelStarted){
            Obstacle.isPaused = true;
            source.Pause();
            PauseMenu.SetActive(true);
        }else{

        }
    }

    public void ResumeGame(){
        //Time.timeScale = 1.0f;
        if(hasLevelStarted){
            Obstacle.isPaused = false;
            source.UnPause();
            PauseMenu.SetActive(false);
        }else{

        }
    }

    void scoreFeedback(float rightDis, float leftDis){
        if(rightDis >= 15f){                                                                                //arbitrary values here
            hapticFeedback(true, rightDis > 30f ? .5f : lineTwoPoints(15f, .1f, 30f, .5f, rightDis), 0.2f); //arbitrary values here 
        }

        if(leftDis >= 15f){                                                                                 //arbitrary values here
            hapticFeedback(false, leftDis > 30f ? .5f : lineTwoPoints(15f, .1f, 30f, .5f, leftDis), 0.2f);  //arbitrary values here 
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
        hapticFeedback(true, 0.4f, 0.3f);  //arbitrary values here
        hapticFeedback(false, 0.4f, 0.3f); //arbitrary values here
        addCombo(0);
        scoreOut.updateCombo(combo);
        scoreOut.ShowHit();
    }

    public void ResetHeight(){
        float playerHeight = FindObjectOfType<XROrigin>().CameraInOriginSpaceHeight + additionalHeight;
        if(playerHeight > 2.5f) playerHeight = 2.5f; //arbitrary values here
        if(playerHeight < 1.2f) playerHeight = 1.2f; //arbitrary values here
        
        transform.localScale = initialScale * (playerHeight / initialHeightOfPanels);
        transform.position = new Vector3(initialPosition.x, initialPosition.y*(transform.localScale.y/initialScale.y), initialPosition.z);
    }

    public void QuitLevel(){
        hasLevelStarted = false;
        Application.Quit();
    }
}