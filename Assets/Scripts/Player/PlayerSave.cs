using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSave : MonoBehaviour
{
    private bool inSaveZone;
    [SerializeField] GameObject initSavePoint;

    private JsonData testData;

    public class JsonData
    {
        public PlayerData playerData;
    }

    public class PlayerData
    {
        public Vector2 position;
    }

    private void Start()
    {
        InitJsonData();
        PlayerDataSave(initSavePoint.transform.position);
        JsonSave();
    }

    public void Save(GameObject savePoint)
    {
        InitJsonData();
        PlayerDataSave(savePoint.GetComponent<SavePoint>().savePos);
        JsonSave();
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

        // 如果檔案不存在，建立檔案後繼續處理
        if (!File.Exists(path))
        {
            Debug.Log("檔案不存在，正在建立...");
            using (File.Create(path))
            {
                // 檔案流會在 using 結束時自動釋放資源
            }
        }

        // 將資料轉換為 JSON
        string json = JsonUtility.ToJson(testData, true);

        // 寫入檔案
        File.WriteAllText(path, json);

        Debug.Log("已完成儲存於: " + path);
    }

    private void PlayerDataSave(Vector2 savePointPos)//GameObject savePoint)
    {
        testData.playerData.position = savePointPos;// savePoint.transform.position;
    }

    public void PlayerDataLoad()
    {
        PlayerMove.StopMovement();
        this.transform.position = testData.playerData.position;
        Debug.Log("Data load successful, player at position: " + testData.playerData.position);
    }
}
