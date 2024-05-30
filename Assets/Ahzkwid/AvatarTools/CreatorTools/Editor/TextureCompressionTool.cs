using UnityEngine;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
class TexturesCompressionTool : EditorWindow
{
    public Object textureFolder;
    public Texture2D[] textures;

    static Texture2D[] GetFolderToTextureAssets(Object folder)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePathTextures = AssetDatabase.FindAssets($"t:{typeof(Texture2D).Name}", new string[] { folderPath });
        var textures = new Texture2D[filePathTextures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(filePathTextures[i]));
        }
        return textures;
    }

    public static Texture2D ChangeFormat(Texture2D oldTexture, TextureFormat newFormat)
    {
        var newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, oldTexture.mipmapCount > 1);
        newTex.SetPixels(oldTexture.GetPixels());
        newTex.Apply();
        return newTex;
    }

    static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        var result = new Texture2D(targetWidth, targetHeight, source.format, source.mipmapCount > 1);
        var rpixels = result.GetPixels(0);
        var incX = (1.0f / targetWidth);
        var incY = (1.0f / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    void Compress(Object folder)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePathTextures = AssetDatabase.FindAssets($"t:{typeof(Texture2D).Name}", new string[] { folderPath });
        filePathTextures = System.Array.ConvertAll(filePathTextures, x => AssetDatabase.GUIDToAssetPath(x));
        var textureImporters = System.Array.ConvertAll(filePathTextures, x => AssetImporter.GetAtPath(x) as TextureImporter);
        var textureTypes = System.Array.ConvertAll(textureImporters, x => x.textureType);
        var isReadables = System.Array.ConvertAll(textureImporters, x => x.isReadable);
        var textureSettings = System.Array.ConvertAll(textureImporters, x => {

            var from = x.GetPlatformTextureSettings("Default");
            /*
            var to = new TextureImporterPlatformSettings();
            var fields = from.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
                Debug.Log($"field.GetValue(from): {field.GetValue(from)} -> field.GetValue(to): {field.GetValue(to)}");
            }
            */
            var to = new TextureImporterPlatformSettings
            {
                format = from.format,
                maxTextureSize = from.maxTextureSize,
                compressionQuality = from.compressionQuality,
                allowsAlphaSplitting = from.allowsAlphaSplitting,
                overridden = from.overridden,
                textureCompression = from.textureCompression,
                crunchedCompression = from.crunchedCompression
            };


            return to;
            });

        for (int i = 0; i < filePathTextures.Length; i++)
        {
            var textureImporter = textureImporters[i];
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.isReadable = true;
            textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                format = TextureImporterFormat.RGBA32
            });
            AssetDatabase.ImportAsset(filePathTextures[i]);
        }

        var textures = GetFolderToTextureAssets(folder);

        for (int i = 0; i < textures.Length; i++)
        {
            Debug.Log(filePathTextures[i]);
            var texture = textures[i];
            var filePathTexture = filePathTextures[i];
            var textureImporter = textureImporters[i];

            int wid = Mathf.Min(textureImporter.maxTextureSize, texture.width);
            int hei = Mathf.Min(textureImporter.maxTextureSize, texture.height);
            //Debug.Log("texture.format: " + texture.format);

            if (wid > 4000)
            {
                continue;
            }
            {
                var textureCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

                textureCopy.SetPixels(texture.GetPixels());
                textureCopy.Apply();
                Graphics.CopyTexture(texture, textureCopy);
                //textureCopy = ChangeFormat(textureCopy, TextureFormat.RGBA32);
                textureCopy = ResizeTexture(textureCopy, wid, hei);

                var isReadable = isReadables[i];
                var textureSetting = textureSettings[i];
                var textureType = textureTypes[i];
                textureImporter.textureType = textureType;
                textureImporter.isReadable = isReadable;
                textureImporter.SetPlatformTextureSettings(textureSetting);

                File.WriteAllBytes(filePathTextures[i], textureCopy.EncodeToPNG());
                Debug.Log($"folderPath: {filePathTextures[i]}, Resolution -> ({wid},{hei})");
            }
        }
        for (int i = 0; i < filePathTextures.Length; i++)
        {
            AssetDatabase.ImportAsset(filePathTextures[i]);
        }
        /*
        textureImporters = System.Array.ConvertAll(filePathTextures, x => AssetImporter.GetAtPath(x) as TextureImporter);
        for (int i = 0; i < filePathTextures.Length; i++)
        {
            var textureImporter = textureImporters[i];
            var crunchedCompression = crunchedCompressions[i];
            var isReadable = isReadables[i];
            var textureSetting = textureSettings[i];
            //textureImporter.crunchedCompression = crunchedCompression;
            AssetDatabase.ImportAsset(filePathTextures[i]);
        }
        */
    }



    public static void Init()
    {
        var window = GetWindow<TexturesCompressionTool>(utility: false, title: nameof(TexturesCompressionTool));
        window.minSize = new Vector2(300, 200);
        window.Show();
    }

    SerializedObject serializedObject;

    void OnGUI()
    {
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(this);
        }

        var allReady = true;

        serializedObject.Update();
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textureFolder)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                //serializedObject.ApplyModifiedProperties();
                //textures = GetFolderToTextureAssets(textureFolder);
            }
            EditorGUILayout.Space();
            GUI.enabled = false;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textures)));
            GUI.enabled = true;
            EditorGUILayout.Space();
            if (textureFolder == null)
            {
                allReady = false;
            }

            if (allReady)
            {
            }
        }
        serializedObject.ApplyModifiedProperties();

        GUI.enabled = allReady;
        if (GUILayout.Button("Run"))
        {
            Compress(textureFolder);
        }
        GUI.enabled = true;
    }
}
#endif
