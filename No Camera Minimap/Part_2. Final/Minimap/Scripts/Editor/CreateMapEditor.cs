using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using GL =UnityEngine.GUILayout; 

public class CreateMapEditor : EditorWindow
{
    private int _subdivideCount;

    private Rect[] _rects;
    private Texture2D[] _screens;
    
    private Transform _startPoint;
    private Transform _endPoint;
    private Rect _borders;

    private Color _backgroundColor;
    private float _cameraHeight;
    private int _prefabMapSize;

    [MenuItem("Leksay/ Mini Map Creator")]
    public static void Open()
    {
        CreateMapEditor window = GetWindow<CreateMapEditor>();
        
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += SetBorders;

        _startPoint = new GameObject("Start Point").transform;
        _endPoint = new GameObject("End Point").transform;

        LoadWindowData();
    }

    private void OnDisable()
    {
        EditorApplication.update -= SetBorders;

        SaveWindowData();

        DestroyImmediate(_startPoint.gameObject);
        DestroyImmediate(_endPoint.gameObject);
    }

    private void OnGUI()
    {
        BeginHorizontal();
        {
            BeginVertical();
            {
                _subdivideCount = IntField("Subdivide count", _subdivideCount);
            }
            EndVertical();
            BeginVertical();
            {
                LabelField($"Screenshot count: {_subdivideCount * _subdivideCount}");
            }
            EndVertical();

            _cameraHeight = FloatField("Camera Height", _cameraHeight);
            _backgroundColor = ColorField("Background Color", _backgroundColor);
        }
        EndHorizontal();

        if (GL.Button("Create Screenshots"))
            CreateScreenshots();

        if (_screens is { Length: > 0 })
            DrawMapPreview();

        BeginHorizontal();
        {
            BeginVertical();
            {
                _prefabMapSize = IntField("Prefab Map Size", _prefabMapSize);
                if (_screens is { Length: > 0 } && GL.Button("Create Prefab"))
                {
                    MapPrefabUtils.CreateMapPrefab(_subdivideCount, _prefabMapSize, _startPoint.position, _endPoint.position);
                    _screens = null;
                }
            }
            EndVertical();
        }
        EndHorizontal();
    }

    private void DrawMapPreview()
    {
        const float previewSize = 400;
        const float previewYPadding = 50;
        const float previewXPadding = 5;

        if(_subdivideCount == 0 || _rects == null || _subdivideCount * _subdivideCount > _rects.Length)
            return;

        Rect mapRect = BeginHorizontal(GL.Width(previewXPadding + previewSize), GL.Height(previewYPadding + previewSize));
        {
            float scale = previewSize / _subdivideCount;

            for (int x = 0; x < _subdivideCount; x++)
            {
                for (int z = 0; z < _subdivideCount; z++)
                {
                    int index = x * _subdivideCount + z;

                    Rect worldRect = _borders;
                    Rect previewRect = new Rect(mapRect.x, mapRect.y, previewSize, previewSize);

                    Vector2 localPosition =
                        MapUtils.GetRemappedRectPosition(worldRect, previewRect, _rects[index].position);

                    localPosition.y = mapRect.y + previewRect.yMax - scale - localPosition.y;

                    Rect rect = new Rect(localPosition.x, localPosition.y, scale, scale);
                    
                    GUI.Box(rect, _screens[index]);
                }
            }
        }
        EndHorizontal();
        Space(previewSize);
    }

    private void CreateScreenshots()
    {
        _screens = new Texture2D[_rects.Length];

        for (int i = 0; i < _rects.Length; i++)
        {
            _screens[i] = CameraTextureUtils.CreateScreen(_backgroundColor, _cameraHeight, _rects[i]);

            MapSaveLoadUtils.SaveTexture(_screens[i], i);
        }
    }

    private void SetBorders()
    {
        _borders = MapUtils.CalculateBorders(_startPoint.position, _endPoint.position);

        const float drawDebugDuration = 0.05f;
        DrawDebugRect(_borders, drawDebugDuration);
        
        MapUtils.CalculateSubRects(out _rects, _subdivideCount, _borders);
        foreach (Rect rect in _rects)
        {
            DrawDebugRect(rect, drawDebugDuration);
        }
    }

    private void DrawDebugRect(Rect rect, float duration = 0.0f)
    {
        float y = (_startPoint.position.y + _endPoint.position.y) / 2f;

        Vector3 point0 = new Vector3(rect.xMin, y, rect.yMin);
        Vector3 point1 = new Vector3(rect.xMin, y, rect.yMax);
        Vector3 point2 = new Vector3(rect.xMax, y, rect.yMax);
        Vector3 point3 = new Vector3(rect.xMax, y, rect.yMin);

        Debug.DrawLine(point0, point1, Color.red, duration, false);
        Debug.DrawLine(point1, point2, Color.red, duration, false);
        Debug.DrawLine(point2, point3, Color.red, duration, false);
        Debug.DrawLine(point3, point0, Color.red, duration, false);
    }

    private void LoadWindowData()
    {
        MinimapWindowDataModel data = MapSaveLoadUtils.LoadWindowData();

        if (data != null)
        {
            _startPoint.position = data.StartPosition.ToVector3();
            _endPoint.position = data.EndPosition.ToVector3();
            _subdivideCount = data.SubdivideCount;
            _cameraHeight = data.CameraHeight;
        }

        _prefabMapSize = 800;
        _screens ??= MapSaveLoadUtils.LoadAllMapTextures();
    }

    private void SaveWindowData()
    {
        MapSaveLoadUtils.SaveWindowData(new MinimapWindowDataModel
        {
            StartPosition = Vector3Model.FromVector3(_startPoint.position),
            EndPosition = Vector3Model.FromVector3(_endPoint.position),
            BackgroundColor = _backgroundColor,
            CameraHeight = _cameraHeight,
            SubdivideCount = _subdivideCount
        });
    }
}
