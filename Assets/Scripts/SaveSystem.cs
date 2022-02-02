using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
  // Makes it a singleton / single instance
  static public SaveSystem instance;
  string filePath;

  private void Awake()
  {
    // Check there are no other instances of this class in the scene
    if (!instance)
    {
      instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    filePath = Application.persistentDataPath + "/save.data";
  }

  public void SaveGame(SavedData saveData)
  {
    FileStream dataStream = new FileStream(filePath, FileMode.Create);

    BinaryFormatter converter = new BinaryFormatter();
    converter.Serialize(dataStream, saveData);

    dataStream.Close();
  }

  public SavedData LoadGame()
  {
    if(File.Exists(filePath))
    {
      // File exists 
      FileStream dataStream = new FileStream(filePath, FileMode.Open);

      BinaryFormatter converter = new BinaryFormatter();
      SavedData saveData = converter.Deserialize(dataStream) as SavedData;

      dataStream.Close();
      return saveData;
    }
    else
    {
      // File does not exist
      // Debug.LogError("Save file not found in " + filePath);
      return null;
    }
  }
}