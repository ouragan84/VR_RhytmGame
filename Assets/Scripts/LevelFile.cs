[System.Serializable]
public class LevelFile
{
    public short id;
    public string name;
    public string song_path;
    public string song_icon_path;
    public float length;
    public bool isRandom;
    public LevelStructure[] level_structure;
}