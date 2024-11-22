using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PolygonCollider2D))]
public class RoomMeshGenerator : MonoBehaviour
{
    public Color fillColor = Color.gray; // 房間內部顏色
    public Material fillMaterial; // 用於填充的材質

    private PolygonCollider2D polygonCollider;
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Start()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    //從 Polygon Collider 創建 Mesh
    void GenerateMesh()
    {
        // 創建 Mesh
        mesh = new Mesh();

        // 獲取 Polygon Collider 的頂點數據
        Vector2[] points = polygonCollider.points;

        // 轉換頂點為 Vector3
        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];
        }

        // 創建三角形索引（2D 多邊形必須進行三角化）
        // 可優化: 用插件
        Triangulator triangulator = new Triangulator(points);
        int[] triangles = triangulator.Triangulate();

        // 設置 Mesh 數據
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 添加 MeshRenderer 和 MeshFilter
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = fillMaterial; // material
        meshRenderer.material.color = fillColor; // 設置顏色
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player"))
        {
            GenerateMesh(); // 生成 Mesh
            Destroy(this); 
        }
    }
}

public class Triangulator
{
    private List<Vector2> points;

    public Triangulator(Vector2[] points)
    {
        this.points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++) V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++) V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;

        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u) u = 0;
            v = u + 1;
            if (nv <= v) v = 0;
            int w = v + 1;
            if (nv <= w) w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        float A = 0.0f;
        for (int p = points.Count - 1, q = 0; q < points.Count; p = q++)
        {
            Vector2 pval = points[p];
            Vector2 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = points[V[u]];
        Vector2 B = points[V[v]];
        Vector2 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w)) continue;
            Vector2 P = points[V[p]];
            if (InsideTriangle(A, B, C, P)) return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, px, py;
        float cXap, bXcp, aXbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        px = P.x - A.x; py = P.y - A.y;

        aXbp = ax * py - ay * px;
        bXcp = bx * py - by * px;
        cXap = cx * py - cy * px;

        return ((aXbp >= 0.0f) && (bXcp >= 0.0f) && (cXap >= 0.0f));
    }
}