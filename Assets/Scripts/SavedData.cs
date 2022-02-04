using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData
{
    public int EnvironementID;
    public short[] songsLastDifficultyPlayed; //i=index, 0=easy, 1=medium, 2=hard
    public string[][] songsBestGrade; // [song index][song difficulty]
    public int[][] songsBestScore;
    public int[] uniqueSongIdentifier;
    public float musicVolume;
    public float fxVolume;
    public float playerHeight;
    //add everything related to high scores there

    public void ResetData(Levels levels){
        EnvironementID = 0;
        musicVolume = .7f;
        fxVolume = .7f;
        playerHeight = 1.75f;
        
        songsLastDifficultyPlayed = new short[levels.levels.Length];
        songsBestGrade = new string[levels.levels.Length][];
        songsBestScore = new int[levels.levels.Length][];
        uniqueSongIdentifier = new int[levels.levels.Length];

        for(int i = 0; i < levels.levels.Length; i++){
            songsLastDifficultyPlayed[i] = 1;
            uniqueSongIdentifier[i] = levels.levels[i].uniqueID;
            songsBestGrade[i] = new string[levels.levels[i].level_structure.Length];
            songsBestScore[i] = new int[levels.levels[i].level_structure.Length];
            for(int j = 0; j < levels.levels[i].level_structure.Length; j++){
                songsBestGrade[i][j] = " ";
                songsBestScore[i][j] = 0;
            }
        }
    }

    public void checkForLevelsChange(Levels levels){
        int[] newSongsID = new int[levels.levels.Length];
        string[][] newBestGrade = new string[levels.levels.Length][];
        int[][] newBestScore = new int[levels.levels.Length][];
        short[] newLastDif = new short[levels.levels.Length];

        for(int i = 0; i < levels.levels.Length; i++){

            newSongsID[i] = levels.levels[i].uniqueID;
            newBestGrade[i] = new string[levels.levels[i].level_structure.Length];
            newBestScore[i] = new int[levels.levels[i].level_structure.Length];

            bool isMatched = false;
            for(int j = 0; j < uniqueSongIdentifier.Length; j++){

                if(uniqueSongIdentifier[j] == newSongsID[i]){

                    if(songsLastDifficultyPlayed[j] >= levels.levels[i].level_structure.Length)
                        newLastDif[i] = 1;
                    else
                        newLastDif[i] = songsLastDifficultyPlayed[j];

                    int k = 0;
                    for(; k < Mathf.Min(newBestScore[i].Length, songsBestScore[j].Length); k++){
                        newBestGrade[i][k] = songsBestGrade[j][k];
                        newBestScore[i][k] = songsBestScore[j][k];
                    }
                    for(; k < newBestScore[i].Length; k++){
                        newBestGrade[i][k] = " ";
                        newBestScore[i][k] = 0;
                    }

                    isMatched = true;
                    break;
                }
            }
            if(!isMatched){
                newLastDif[i] = 1;
                for(int k = 0; k < levels.levels[i].level_structure.Length; k++){
                    newBestGrade[i][k] = " ";
                    newBestScore[i][k] = 0;
                }
            }
        }

        songsBestGrade = newBestGrade;
        songsBestScore = newBestScore;
        uniqueSongIdentifier = newSongsID;
        songsLastDifficultyPlayed = newLastDif;
    }
}
