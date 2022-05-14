using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour {
    public float hexRadius;
    public int chunkSize;
    public float mapScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float heightMultiplier;
    public AnimationCurve heightMultiplierCurve;

    Mesh mesh;
    MeshCollider meshCollider;
    List<Vector3> meshVerticies;
    List<int> meshTriangles;
    List<Vector3> meshTriVerticies;
    List<Vector3> meshNormals;

    public Dictionary<AxialCoords, Hexagon> hexagons;

    float hexSize;

    int currentHex = 0;

    List<Color32> meshTriColors;

    int spriteAtlasAmount = 7;

    float[,] noiseMap;

    public TerrainType[] terrainTypes;
    List<Color32> meshColors;

    Vector3[] baseHexVerts;

    List<int> waterTriangles;

    void Start() {
        mesh = new Mesh();
        baseHexVerts = VoxelData.HexVerticies(hexRadius);
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        GenerateChunk();
    }

    void OnValidate() {
        GenerateChunk();
    }

    void GenerateChunk() {
        noiseMap = Noise.GenerateNoiseMap((chunkSize * 2) + 1, (chunkSize * 2) + 1, seed, mapScale, octaves, persistance, lacunarity, offset);
        GenerateGrid();
        GenerateSides();
        UpdateMesh();
    }

    void GenerateGrid() {
        currentHex = 0;
        meshVerticies = new List<Vector3>();
        meshTriangles = new List<int>();
        meshColors = new List<Color32>();
        waterTriangles = new List<int>();
        hexagons = new Dictionary<AxialCoords, Hexagon>();
        for (int r = -chunkSize; r <= chunkSize; r++) {
            int r_offset = Mathf.FloorToInt(r / 2f);
            for (int q = -chunkSize - r_offset; q <= chunkSize - r_offset; q++) {
                AxialCoords axialCoords = new AxialCoords(q, r);
                CreateHex(axialCoords);
            }
        }
    }

    void GenerateSides() {
        foreach (KeyValuePair<AxialCoords, Hexagon> hexagon in hexagons) {
            for (int i = 0; i < 6; i++) {
                Vector2Int neighbor = VoxelData.hexNeighborDirection[i];
                AxialCoords neigborAxialCoords = new AxialCoords(hexagon.Key.q + neighbor.x, hexagon.Key.r + neighbor.y);
                if (hexagons.ContainsKey(neigborAxialCoords)) {
                    if (hexagons[neigborAxialCoords].worldCoords.y < hexagon.Value.worldCoords.y) {
                        int[] sideVerticies = new int[4];
                        meshVerticies.Add(meshVerticies[hexagon.Value.verticeStartingPoint + VoxelData.hexNeigborVerticies[i * 4]]);
                        sideVerticies[0] = meshVerticies.Count - 1;
                        meshVerticies.Add(meshVerticies[hexagon.Value.verticeStartingPoint + VoxelData.hexNeigborVerticies[(i * 4) + 1]]);
                        sideVerticies[1] = meshVerticies.Count - 1;
                        meshVerticies.Add(meshVerticies[hexagons[neigborAxialCoords].verticeStartingPoint + VoxelData.hexNeigborVerticies[(i * 4) + 2]]);
                        sideVerticies[2] = meshVerticies.Count - 1;
                        meshVerticies.Add(meshVerticies[hexagons[neigborAxialCoords].verticeStartingPoint + VoxelData.hexNeigborVerticies[(i * 4) + 3]]);
                        sideVerticies[3] = meshVerticies.Count - 1;
                        Color32[] color32s = new Color32[4];
                        for (int j = 0; j < 4; j++) {
                            color32s[j] = hexagon.Value.terrainType.color;
                        }
                        ArrayFunctions.AppendArrayToList(color32s, meshColors);
                        foreach (int triangle in VoxelData.hexNeighborTriangles) {
                            if (hexagon.Value.terrainType.voxelType == TerrainType.VoxelType.Solid) {
                                meshTriangles.Add(sideVerticies[triangle]);
                            } else {
                                waterTriangles.Add(sideVerticies[triangle]);
                            }
                        }
                    }
                }
            }
        }
    }

    void CreateHex(AxialCoords axialCoords) {
        float voxelHeight = noiseMap[axialCoords.AxialToOffset().x + chunkSize, axialCoords.AxialToOffset().y + chunkSize];
        Vector3 worldCoords = axialCoords.AxialToWorld(hexRadius, heightMultiplierCurve.Evaluate(voxelHeight) * heightMultiplier);
        TerrainType terrainType = new TerrainType();
        foreach (TerrainType terrain in terrainTypes) {
            if (terrain.minHeight <= voxelHeight && voxelHeight <= terrain.maxHeight) {
                terrainType = terrain;
                break;
            }
        }
        hexagons.Add(axialCoords, new Hexagon(worldCoords, meshVerticies.Count, terrainType));
        ArrayFunctions.AppendArrayToList(relativeHexVerts(worldCoords), meshVerticies);
        if (terrainType.voxelType == TerrainType.VoxelType.Solid) {
            ArrayFunctions.AppendArrayToList(relativeTriangles(), meshTriangles);
        } else {
            ArrayFunctions.AppendArrayToList(relativeTriangles(), waterTriangles);
        }

        Color32[] color32s = new Color32[6];
        for (int i = 0; i < color32s.Length; i++) {
            color32s[i] = terrainType.color;
        }
        ArrayFunctions.AppendArrayToList(color32s, meshColors);
        currentHex += 1;
    }

    void UpdateMesh() {
        mesh.Clear();
        meshNormals = new List<Vector3>();
        mesh.vertices = meshVerticies.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(meshTriangles.ToArray(), 0);
        mesh.SetTriangles(waterTriangles.ToArray(), 1);
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        mesh.colors32 = meshColors.ToArray();
    }

    int[] relativeTriangles() {
        int[] triangles = new int[VoxelData.hexTriangles.Length];
        for (int i = 0; i < triangles.Length; i++) {
            triangles[i] = (6 * currentHex) + VoxelData.hexTriangles[i];
        }
        return triangles;
    }

    Vector3[] relativeHexVerts(Vector3 center) {
        Vector3[] relativeHexVerts = new Vector3[6];
        for (int i = 0; i < baseHexVerts.Length; i++) {
            relativeHexVerts[i] = new Vector3(baseHexVerts[i].x + center.x, baseHexVerts[i].y + center.y, baseHexVerts[i].z + center.z);
        }
        return relativeHexVerts;
    }
}

public struct AxialCoords {
    public int q { get; private set; }
    public int r { get; private set; }
    public int s { get { return -q - r; } }
    public AxialCoords(int q, int r) {
        this.q = q;
        this.r = r;
    }
    public Vector3 AxialToWorld(float hexSize, float hexHeight) {
        float vectorX = hexSize * (3f / 2 * r);
        float vectorY = hexSize * (Mathf.Sqrt(3) * q + Mathf.Sqrt(3) / 2 * r);
        return new Vector3(vectorX, hexHeight, vectorY);
    }

    public Vector2Int AxialToOffset() {
        int col = this.q + (this.r - (this.r & 1)) / 2;
        int row = this.r;
        return new Vector2Int(col, row);
    }
}

public struct Hexagon {
    public Vector3 worldCoords { get; private set; }
    public TerrainType terrainType { get; private set; }
    public int verticeStartingPoint;

    public Hexagon(Vector3 worldCoords, int verticeStartingPoint, TerrainType terrainType) {
        this.worldCoords = worldCoords;
        this.verticeStartingPoint = verticeStartingPoint;
        this.terrainType = terrainType;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float minHeight;
    public float maxHeight;
    public Color32 color;
    public VoxelType voxelType;
    public enum VoxelType {
        Solid,
        Water
    }
}