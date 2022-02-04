using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public Button[] Environentbuttons;
    public Button[] Levelbuttons;
    public TextAsset levelFile;
    public TextAsset environemntsFile;
    public GameObject mainCanvas;
    public GameObject songInfoCanvas;
    public GameObject optionsCanvas;
    public Button[] Difficultybuttons;
    public Slider[] volumeSliders;
    public Slider[] volumeFxSliders;
    public AudioSource[] musicSources;
    public AudioSource[] FXSources;

    private Levels levels;
    private Environements environements;
    private int currentEvironementID;
    private SavedData gameData;
    private AudioSource source;
    private int selectedSongID = -1;
    private int previewMusicID = -1;
    private bool previewMusicisSelect = false;
    private AudioClip[] preiviewClips;
    private short currentDifficulty;
    private WallGenerator generator;
    private fadeIn_Out fader;

    void Start(){
        fader = GameObject.FindObjectOfType<fadeIn_Out>();
        generator = GameObject.FindObjectOfType<WallGenerator>();
        enableMenu();
        source = GetComponent<AudioSource>();
        loadLevels();
        LoadSavedData();
        updateScene();
        AssignButtons();
        getPreviewClips();
        generator.setHeight(gameData.playerHeight);
        changeVolume(gameData.musicVolume);
        changeFXVolume(gameData.fxVolume);
    }

    public void LoadSavedData(){
        gameData = SaveSystem.instance.LoadGame(); //SaveSystem.instance.SaveGame(gameData);
        if(gameData == null){
            gameData = new SavedData();
            gameData.ResetData(levels);
        }else{
            gameData.checkForLevelsChange(levels);
        }
        SaveSystem.instance.SaveGame(gameData);
        currentEvironementID = gameData.EnvironementID;
    }
    
    public void loadLevels(){
        levels = JsonUtility.FromJson<Levels>(levelFile.text);
        WallGenerator.levels = levels;
        environements = JsonUtility.FromJson<Environements>(environemntsFile.text);
    }

    public void updateScene(){
        SceneManager.LoadScene(environements.environements[currentEvironementID].scene_path);
        gameData.EnvironementID = currentEvironementID;
        SaveSystem.instance.SaveGame(gameData);
        disableCurrentEnvironementButton();
    }

    public void disableCurrentEnvironementButton(){
        Environentbuttons[currentEvironementID].interactable = false;
    }

    public void enableCurrentEnvironementButton(){
        Environentbuttons[currentEvironementID].interactable = true;
    }

    public void AssignButtons(){
        for(int i = 0; i < Environentbuttons.Length; i++){
            int x = i;
            Environentbuttons[i].onClick.AddListener(delegate {FadeToEnvironement(x); });
            Environentbuttons[i].transform.Find("name").GetComponent<TextMeshProUGUI>().text = environements.environements[i].name;
            Environentbuttons[i].transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(environements.environements[i].icon_path);
        }

        for(int i = 0; i < Levelbuttons.Length; i++){
            int x = i;
            Levelbuttons[i].onClick.AddListener(delegate {selectLevel(x); });
            Levelbuttons[i].transform.Find("name").GetComponent<TextMeshProUGUI>().text = levels.levels[i].name;
            Levelbuttons[i].transform.Find("author").GetComponent<TextMeshProUGUI>().text = levels.levels[i].author;
            Levelbuttons[i].transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(levels.levels[i].song_icon_path);
        }

        for(int i = 0; i < Difficultybuttons.Length; i++){
            short x = (short)i;
            Difficultybuttons[i].onClick.AddListener(delegate {changeDifficulty(x); });
        }
    }

    public void FadeToEnvironement(int id){
        if(id != currentEvironementID){
            fader.FadeWithFunctionCall(changeEnvironement, id);
        }
    }

    public void changeEnvironement(int index){
        if(index != currentEvironementID){
            enableCurrentEnvironementButton();
            currentEvironementID = index;
            updateScene();
        }
    }

    public void getPreviewClips(){
        preiviewClips = new AudioClip[Levelbuttons.Length];
        for(int i = 0; i < preiviewClips.Length; i++){
            preiviewClips[i] = Resources.Load<AudioClip>(levels.levels[i].preview_path);
        }
    }

    public void selectLevel(int index){
        previewMusicisSelect = true;
        if(previewMusicID != index){
            playMusicID(index);
        }
        if(selectedSongID != index){
            selectedSongID = index;
            displayLevelInfos();
        }
    }

    public void playMusicID(int ID){
        previewMusicID = ID;
        source.Stop();
        source.PlayOneShot(preiviewClips[ID]);
    }

    public void displayLevelInfos(){
        songInfoCanvas.SetActive(true);
        songInfoCanvas.transform.Find("Song Title").GetComponent<TextMeshProUGUI>().text = levels.levels[selectedSongID].name;
        songInfoCanvas.transform.Find("Song Author").GetComponent<TextMeshProUGUI>().text = levels.levels[selectedSongID].author;
        songInfoCanvas.transform.Find("Song Length").GetComponent<TextMeshProUGUI>().text = levels.levels[selectedSongID].length;
        songInfoCanvas.transform.Find("Song Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(levels.levels[selectedSongID].song_icon_path);
        changeDifficulty(gameData.songsLastDifficultyPlayed[selectedSongID]);
    }   

    public void displayHighScore(){
        songInfoCanvas.transform.Find("Best Grade").Find("Grade").GetComponent<TextMeshProUGUI>().text = gameData.songsBestGrade[selectedSongID][currentDifficulty];
        songInfoCanvas.transform.Find("Best Score").Find("Score").GetComponent<TextMeshProUGUI>().text = formatScore(gameData.songsBestScore[selectedSongID][currentDifficulty]);
    }

    public static string formatScore(int score){
        return "" + score;
    }

    public void changeDifficulty(short difficulty){
        foreach(Button b in Difficultybuttons){
            b.interactable = true;
        }
        Difficultybuttons[difficulty].interactable = false;
        currentDifficulty = difficulty;
        displayHighScore();
    }

    public void StartLevel(){
        if(selectedSongID >= 0){
            gameData.songsLastDifficultyPlayed[selectedSongID] = currentDifficulty;
            SaveSystem.instance.SaveGame(gameData);
            source.Stop();
            generator.StartLevel(selectedSongID, currentDifficulty, "");
            disableAllCanvas();
            fader.Fade();
        }
    }

    public string getStringDifficulty(){
        switch(currentDifficulty){
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Hard";
        }
        return "";
    }

    public void disableAllCanvas(){
        selectedSongID = -1;
        previewMusicID = -1;
        currentDifficulty = -1;
        mainCanvas.SetActive(false);
        songInfoCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
    }

    public void enableMenu(){
        mainCanvas.SetActive(true);
        songInfoCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public int getHighScore(int id, short diff){
        return gameData.songsBestScore[id][diff];
    }

    public string getHighGrade(int id, short diff){
        return gameData.songsBestGrade[id][diff];
    }
    
    public void setNewHighScore(int score, string grade, int id, short diff){
        gameData.songsBestGrade[id][diff] = grade;
        gameData.songsBestScore[id][diff] = score;
        SaveSystem.instance.SaveGame(gameData);
    }

    public void ResetData(){
        gameData.ResetData(levels);
        displayLevelInfos();
        SaveSystem.instance.SaveGame(gameData);
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void resetHeight(){
        gameData.playerHeight = generator.ResetHeight();
        SaveSystem.instance.SaveGame(gameData);
    }

    // id: 0 = music, 1 = FX
    public void changeVolume(float value){
        foreach(Slider s in volumeSliders)
            s.value = value;
        foreach(AudioSource a in musicSources)
            a.volume = value;
        gameData.musicVolume = value;
        SaveSystem.instance.SaveGame(gameData);
    }

    public void changeFXVolume(float value){
        foreach(Slider s in volumeFxSliders)
            s.value = value;
        foreach(AudioSource a in FXSources)
            a.volume = value;
        gameData.fxVolume = value;
        SaveSystem.instance.SaveGame(gameData);
    }
}
