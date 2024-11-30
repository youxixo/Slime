using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    private bool isOpened;
    [SerializeField] private Transform sight; //準心
    [SerializeField] private float sightBaseSize = 1; //準心size
    [SerializeField] private Camera mapCamera; //渲染小地圖的相機
    [SerializeField] private GameObject map; //小地圖
    [SerializeField] private InputActionAsset inputActions; //輸入
    [SerializeField] private int cameraMoveSpeed; //相機的移動速度
    Vector2 moveInput;

    [Header("標記")]
    [SerializeField] LayerMask signLayer; //標記的Layer
    [SerializeField] GameObject signPrefab; //標記prefab
    [SerializeField] float signRadius; //標記大小
    private List<GameObject> markers = new List<GameObject>(); //標記數組, 存所有標記
    private const int maxSigns = 100;

    private void InitializedCamera()
    {
        mapCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -3);
        mapCamera.orthographicSize = 80;
    }

    private void Update()
    {
        if(moveInput != Vector2.zero)
        {
            mapCamera.transform.Translate(moveInput * cameraMoveSpeed * Time.deltaTime);
        }
        SetSightSize();
    }

    private void SetSightSize()
    {
        // 根據相機縮放比例調整準心大小
        float scale = sightBaseSize * mapCamera.orthographicSize * 0.01f;
        sight.localScale = new Vector3(scale, scale, 1.0f);
    }

    public void ToggleMap()
    {
        Debug.Log("called");
        if(!isOpened) //open map
        {
            InitializedCamera();
            UIController.ActivateActionMap(inputActions, "Map");
            isOpened = true;
            map.SetActive(true);
        }
        else //close map
        {
            UIController.ActivateActionMap(inputActions, "Player");
            isOpened = false;
            map.SetActive(false);
        }
    }

    //可優化: leap一下 更絲滑
    public void Zoom(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() == 1f && mapCamera.orthographicSize >= 40)
        {
            mapCamera.orthographicSize -= 30;
        }
        else if (context.ReadValue<float>() == -1f && mapCamera.orthographicSize <= 150)
        {
            mapCamera.orthographicSize += 30;
        }
    }

    //優化: 跟player action設置中的上下左右同步
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //放置標記
    public void Sign(InputAction.CallbackContext context)
    {
        //使用了context.started 因為從player input 調用的話每個input都會有三個階段 - started → performed → canceled, 導致這個被調用三次 使用started可以讓她只在剛按下時調用
        if (context.started)
        {
            GameObject existingMarker = FindMarkerAtPosition(sight.position);
            if (existingMarker != null)
            {
                // 如果該位置上已有標記 刪除它
                markers.Remove(existingMarker);
                Destroy(existingMarker);
            }
            else if (markers.Count <= maxSigns)
            {
                // 則創建新標記
                GameObject newMarker = Instantiate(signPrefab, new Vector3(sight.transform.position.x, sight.transform.position.y, 0), Quaternion.identity);
                markers.Add(newMarker);
            }
        }
    }

    //從markers數組中 遍歷看看該位置是否已有標記
    private GameObject FindMarkerAtPosition(Vector3 position)
    {
        foreach (GameObject marker in markers)
        {
            if (Vector3.Distance(marker.transform.position, position) <= signRadius)
            {
                return marker;
            }
        }
        return null;
    }
}
