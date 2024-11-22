using UnityEngine;

public class RoomVisualizer : MonoBehaviour
{
    public Color exploredColor = Color.white; // 房間被探索後的顏色
    public Color unexploredColor = Color.gray; // 房間未探索時的顏色
    public Material lineMaterial; // 用於繪製房間邊框的材質
    public bool isExplored = false; // 房間是否已被探索

    private PolygonCollider2D polygonCollider;
    private LineRenderer lineRenderer;

    void Start()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();

        // 創建 LineRenderer 用於繪製邊框
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.loop = true; // 閉合線條
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = lineMaterial;

        // 初始化邊框形狀
        UpdateRoomVisualization();
        ExploreRoom();
    }

    /// <summary>
    /// 更新房間的可視化狀態（顏色和邊框）
    /// </summary>
    public void UpdateRoomVisualization()
    {
        // 設置房間的邊框形狀
        Vector2[] points = polygonCollider.points;
        lineRenderer.positionCount = points.Length + 1;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, (Vector3)points[i] + transform.position);
        }
        lineRenderer.SetPosition(points.Length, (Vector3)points[0] + transform.position); // 閉合

        // 更新房間的顏色
        lineRenderer.startColor = isExplored ? exploredColor : unexploredColor;
        lineRenderer.endColor = isExplored ? exploredColor : unexploredColor;
    }

    /// <summary>
    /// 探索房間
    /// </summary>
    public void ExploreRoom()
    {
        if (!isExplored)
        {
            isExplored = true;
            UpdateRoomVisualization();
        }
    }
}
