using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ColorPaletteWindow : EditorWindow
{
    [MenuItem("Tools/Color Palette/Show Window")]
    public static void ShowColorPaletteWindow()
    {
        EditorWindow wnd = GetWindow<ColorPaletteWindow>();
        wnd.titleContent = new GUIContent("Color Palette");
    }

    private Button lastClickedButton;

    public void CreateGUI()
    {
        string searchPattern = "*.png";
        string[] filePaths = Directory.GetFiles(ColorTexturesPath(), searchPattern);
        ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
        scrollView.style.flexGrow = 1;

        foreach (var (colorName, colorList) in ColorPaletteGenerator.colors)
        {
            scrollView.Add(new Label { text = colorName });
            scrollView.style.paddingTop = 4;
            scrollView.style.paddingRight = 4;
            scrollView.style.paddingBottom = 4;
            scrollView.style.paddingLeft = 4;
            var container = new VisualElement();
            foreach (var (colorNumber, _) in colorList)
            {
                var colorCode = $"{colorName}-{colorNumber}";
                var filePath = $"Assets/Editor/ColorTextures/{colorCode}.png";
                var materialFilePath = $"Assets/_Materials/Colors/color-{colorCode}.mat";
                var button = new Button();

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                var material = AssetDatabase.LoadAssetAtPath<Material>(materialFilePath);

                button.style.backgroundImage = texture;
                button.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
                button.style.height = 20;
                button.style.flexGrow = 1;

                button.clicked += () => { ApplyMaterials(button, material, Tags.Wood); };

                int number = int.Parse(colorNumber);
                button.text = (number / 100).ToString();
                if (int.Parse(colorNumber) < 600)
                {
                    button.style.color = Color.black;
                }

                container.Add(button);
            }
            container.style.flexDirection = FlexDirection.Row;
            container.style.marginBottom = 4;
            scrollView.Add(container);
        }

        rootVisualElement.Add(scrollView);
    }

    private void EditBorder(Button button, int width)
    {
        if (button == null)
            return;

        button.style.borderTopColor = Color.white;
        button.style.borderRightColor = Color.white;
        button.style.borderBottomColor = Color.white;
        button.style.borderLeftColor = Color.white;
        button.style.borderTopWidth = width;
        button.style.borderRightWidth = width;
        button.style.borderBottomWidth = width;
        button.style.borderLeftWidth = width;
    }

    private void ApplyMaterials(Button button, Material material, string tag)
    {
        EditBorder(lastClickedButton, 0);

        var gos = Selection.gameObjects;
        var meshRenderers = new List<MeshRenderer>();
        foreach (var go in gos)
        {
            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                meshRenderers.Add(renderer);
            }
        }

        Undo.RecordObjects(meshRenderers.ToArray(), "Change material through color palette.");

        foreach (var renderer in meshRenderers)
        {
            renderer.sharedMaterial = material;
            renderer.gameObject.tag = tag;
        }

        lastClickedButton = button;
        EditBorder(button, 2);
    }

    private static string ColorTexturesPath() { return $"{Application.dataPath}/Editor/ColorTextures"; }

    [MenuItem("Tools/Color Palette/Generate Textures")]
    public static void GenerateColorPaletteTextures()
    {
        foreach (var (colorName, colorList) in ColorPaletteGenerator.colors)
        {
            foreach (var (colorNumber, colorHexCode) in colorList)
            {
                // create Texture2D
                Texture2D texture = new Texture2D(256, 256);
                ColorUtility.TryParseHtmlString(colorHexCode, out Color color);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
                texture.Apply();

                // generate filename string
                var filename = $"{ColorTexturesPath()}/{colorName}-{colorNumber}.png";


                // encode and write image to disk
                byte[] bytes = texture.EncodeToPNG();
                System.IO.File.WriteAllBytes(filename, bytes);
            }
        }
    }
}
