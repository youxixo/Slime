using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    private JsonData testData;

    public class JsonData
    {
        public PlayerData playerData;
    }

    public class PlayerData
    {
        public Vector2 position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SavePoint"))
        {
            InitJsonData();
            PlayerDataSave(collision.gameObject);
            JsonSave();
        }
        if (collision.gameObject.CompareTag("DeathPoint"))
        {
            PlayerDataLoad();
        }
    }

    private void InitJsonData()
    {
        testData = new JsonData();
        PlayerData data = new PlayerData();
        testData.playerData = data;
    }

    private string JsonPath()
    {
        return Path.Combine(Application.persistentDataPath, "JsonSaveTest.json");
    }

    private void JsonSave()
    {
        string path = JsonPath();

        // 如果檔案不存在，建立檔案並關閉流
        if (!File.Exists(path))
        {
            Debug.Log("File does not exist, creating...");
            using (File.Create(path))
            {
                // 檔案流會在 using 區塊結束時自動關閉
            }
        }

        // 將資料轉換成 JSON
        string json = JsonUtility.ToJson(testData, true);

        // 寫入檔案
        File.WriteAllText(path, json);

        Debug.Log("Finished save at: " + path);
    }

    private void PlayerDataSave(GameObject savePoint)
    {
        testData.playerData.position = savePoint.transform.position;
    }

    private void PlayerDataLoad()
    {
        this.transform.position = testData.playerData.position;
        Debug.Log("Data load successful, player at position: " + testData.playerData.position);
    }
}
