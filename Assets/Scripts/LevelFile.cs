[System.Serializable]
public class LevelFile
{
    public int index;
    public int uniqueID;
    public string name;
    public string author;
    public string song_path;
    public string preview_path;
    public string song_icon_path;
    public string length;
    public bool isRandom;
    public LevelStructure[] level_structure;
}