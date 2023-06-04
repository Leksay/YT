using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static MapSaveLoadUtils;

public static class MapPrefabUtils
{
    #region constants

    private const string SPRITE_FOLDER_NAME = "MapSprites";
    private const string SPRITE_NAME_MASK = "map_sprite*%.png";
    private const string PREFAB_NAME = "map_prefab.prefab";

    #endregion

    #region fields

    private static readonly string _prefabFolder;
    private static readonly string _prefabRelativeFolder;
    private static readonly string _spriteFolder;
    private static readonly Vector2 _center;

    #endregion

    #region enums

    private enum Anchors
    {
        Center,
        Stretch
    }

    #endregion

    #region constructors

    static MapPrefabUtils()
    {
        _prefabFolder = Path.Combine(Application.dataPath, "Sources", "Minimap", "Prefabs");
        _prefabRelativeFolder = _prefabFolder.Substring(_prefabFolder.IndexOf("Assets", StringComparison.Ordinal));
        _spriteFolder = Path.Combine(_prefabFolder, SPRITE_FOLDER_NAME);
        _center = new Vector2(0.5f, 0.5f);
    }

    #endregion

    #region public methods

    public static void CreateMapPrefab(
        int subdivideCount,
        int mapSize,
        Vector3 startPointPosition,
        Vector3 endPointPosition
    )
    {
        AssetDatabase.Refresh();

        CreateDirectoryIfNotExists(_prefabFolder);

        Texture2D[] screens = LoadAllMapTextures();
        Vector2 size = new Vector2(mapSize, mapSize);

        Canvas previewCanvas = CreateMinimapCanvas();

        RectTransform rootRectTransform = CreateDefaultRectTransform(previewCanvas.transform, size, Anchors.Center, "mini_map");
        RectTransform maskRectTransform = CreateDefaultRectTransform(rootRectTransform, Vector2.zero, Anchors.Stretch, "map_mask");
        RectTransform mapRectTransform = CreateDefaultRectTransform(maskRectTransform, size, Anchors.Center, "minimap");
        RectTransform outlineRectTransform = CreateDefaultRectTransform(rootRectTransform, Vector2.zero, Anchors.Stretch, "outline");
        RectTransform iconsRoot = CreateDefaultRectTransform(maskRectTransform, size, Anchors.Center, "icons_root");

        Image maskImage = maskRectTransform.gameObject.AddComponent<Image>();
        maskRectTransform.gameObject.AddComponent<Mask>();

        maskImage.raycastTarget = false;
        maskImage.maskable = false;

        Image outlineImage = outlineRectTransform.gameObject.AddComponent<Image>();
        outlineImage.enabled = false;
        outlineImage.raycastTarget = false;
        outlineImage.maskable = false;

        float screenScale = (float)mapSize / subdivideCount;

        ClearDirectory(_spriteFolder);

        for (int x = 0; x < subdivideCount; x++)
        {
            for (int z = 0; z < subdivideCount; z++)
                CreateMapPart(subdivideCount, x, z, mapRectTransform, screens, screenScale);
        }

        CreateDirectoryIfNotExists(_prefabRelativeFolder);

        Minimap minimap = mapRectTransform.AddComponent<Minimap>();
        minimap.SetData(startPointPosition, endPointPosition, iconsRoot, maskRectTransform);

        SetStretchAnchors(maskRectTransform);
        SetStretchAnchors(outlineRectTransform);

        PrefabUtility.SaveAsPrefabAssetAndConnect(rootRectTransform.gameObject, Path.Combine(_prefabRelativeFolder, PREFAB_NAME),
            InteractionMode.AutomatedAction);

        Selection.activeObject = rootRectTransform.gameObject;
    }

    #endregion

    #region service methods

    private static void CreateMapPart(int subdivideCount, int x, int z, RectTransform prefabRectTransform,
        Texture2D[] screens, float screenScale)
    {
        GameObject mapPart = new GameObject($"Map part [{x}, {z}]");
        RectTransform partRectTransform = mapPart.AddComponent<RectTransform>();

        Vector2 downLeft = -Vector2.one * (screenScale * subdivideCount * 0.5f);

        SetRectTransform(
            partRectTransform,
            prefabRectTransform, 
            Anchors.Center, 
            Vector2.one * screenScale,
            downLeft + new Vector2(x * screenScale, z * screenScale),
            Vector2.zero
        );

        string spriteName = SPRITE_NAME_MASK
            .Replace("*", x.ToString())
            .Replace("%", z.ToString());

        int textureIndex = x * subdivideCount + z;
        Texture2D partTexture = textureIndex >= screens.Length ? null : screens[x * subdivideCount + z];

        if (!partTexture)
        {
            string missingTextureName = SCREENSHOT_FILE_NAME.Replace("*", textureIndex.ToString());

            Debug.LogError($"Missing texture {missingTextureName}");

            return;
        }

        string spritePath = SaveTextureAsSprite(screens[textureIndex], spriteName);
        
        if(string.IsNullOrEmpty(spriteName))
            return;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        Image partImage = mapPart.AddComponent<Image>();

        partImage.sprite = sprite;
        partImage.raycastTarget = false;
        partImage.maskable = true;
    }

    private static string SaveTextureAsSprite(Texture2D screen, string spriteName)
    {
        CreateDirectoryIfNotExists(_spriteFolder);

        string relativeSpritePath = Path.Combine(_prefabRelativeFolder, SPRITE_FOLDER_NAME, spriteName);
        string relativeTexturePath = AssetDatabase.GetAssetPath(screen);

        TextureImporter textureImporter = AssetImporter.GetAtPath(relativeTexturePath) as TextureImporter;
        if (!textureImporter)
            return string.Empty;

        textureImporter.textureType = TextureImporterType.Sprite;

        AssetDatabase.ImportAsset(relativeTexturePath);
        AssetDatabase.MoveAsset(relativeTexturePath, relativeSpritePath);

        return relativeSpritePath;
    }

    private static Canvas CreateMinimapCanvas()
    {
        Canvas canvas = new GameObject("Minimap Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.referencePixelsPerUnit = 100;

        return canvas;
    }

    private static RectTransform CreateDefaultRectTransform(Transform parent, Vector2 size, Anchors anchors, string name)
    {
        RectTransform rectTransform = new GameObject(name).AddComponent<RectTransform>();

        SetRectTransform(rectTransform, parent, anchors, size, Vector2.zero, _center);

        return rectTransform;
    }

    private static void SetRectTransform(
        RectTransform rectTransform,
        Transform parent,
        Anchors anchors,
        Vector2 size,
        Vector2 anchoredPosition,
        Vector2 pivot)
    {
        rectTransform.SetParent(parent, false);
        rectTransform.pivot = pivot;

        if (anchors == Anchors.Center)
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        else if (anchors == Anchors.Stretch)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }
        
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchoredPosition;
    }

    private static void SetStretchAnchors(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    #endregion
}