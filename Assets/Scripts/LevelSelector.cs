using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelector : MonoBehaviour
{
    public Button[] Environentbuttons;
    public levelSelectButton[] Levelbuttons;
    public TextAsset levelFile;
    public TextAsset environemntsFile;

    private Levels levels;
    private Environements environements;
    private int currentEvironementID;
    private SavedData gameData;
    private AudioSource source;
    private int previewMusicID = -1;
    private bool previewMusicisSelect = false;
    private AudioClip[] preiviewClips;

    void Start(){
        levelSelectButton.menuRef = this;
        source = GetComponent<AudioSource>();
        LoadSavedData();
        loadLevels();
        updateScene();
        AssignButtons();
        getPreviewClips();
    }

    public void LoadSavedData(){
        gameData = SaveSystem.instance.LoadGame(); //SaveSystem.instance.SaveGame(saveData);
        if(gameData == null){
            gameData = new SavedData();
            gameData.ResetData();
            SaveSystem.instance.SaveGame(gameData);
        }
        currentEvironementID = gameData.EnvironementID;
    }
    
    public void loadLevels(){
        levels = JsonUtility.FromJson<Levels>(levelFile.text);
        WallGenerator.levels = levels;
        environements = JsonUtility.FromJson<Environements>(environemntsFile.text);
    }

    public void updateScene(){
        SceneManager.LoadScene(environements.environements[currentEvironementID].scene_path);
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
            Environentbuttons[i].onClick.AddListener(delegate {changeEnvironement(x); });
            Environentbuttons[i].transform.Find("name").GetComponent<TextMeshProUGUI>().text = environements.environements[i].name;
            Environentbuttons[i].transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(environements.environements[i].icon_path);
        }

        for(int i = 0; i < Levelbuttons.Length; i++){
            Levelbuttons[i].setId(i);
            Levelbuttons[i].transform.Find("name").GetComponent<TextMeshProUGUI>().text = levels.levels[i].name;
            Levelbuttons[i].transform.Find("author").GetComponent<TextMeshProUGUI>().text = levels.levels[i].author;
            Levelbuttons[i].transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(levels.levels[i].song_icon_path);
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

    public void playPreview(int index){
        if(!previewMusicisSelect){
            playMusicID(index);
        }
    }

    public void stopPreview(int index){
        if(!previewMusicisSelect){
            source.Stop();
            previewMusicID = -1;
        }
    }

    public void selectLevel(int index){
        previewMusicisSelect = true;
        if(previewMusicID != index){
            playMusicID(index);
        }
    }

     public void stopSelect(int index){
        previewMusicisSelect = false;
        source.Stop();
        previewMusicID = -1;
    }

    public void playMusicID(int ID){
        previewMusicID = ID;
        source.Stop();
        source.PlayOneShot(preiviewClips[ID]);
    }
}
