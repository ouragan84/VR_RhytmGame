[System.Serializable]
public class LevelFile
{
    public short id;
    public string name;
    public string song_path;
    public float length;
    public LevelStructure[] level_structure;
}