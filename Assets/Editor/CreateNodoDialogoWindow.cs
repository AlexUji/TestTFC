using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class CreateNodoDialogoWindow : EditorWindow
{
    private string assetName = "NuevoAsset";
    private Sprite selectedSprite;
    private List<string> stringArray = new List<string>();

    [MenuItem("Tools/NodeDialoge creator")]
    public static void ShowWindow()
    {
        GetWindow<CreateNodoDialogoWindow>("NodeDialoge creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Editar NodoDialogo", EditorStyles.boldLabel);
        assetName = EditorGUILayout.TextField("Nombre del Asset", assetName);
        GUILayout.Label("Lineas de texto:");
        for (int i = 0; i < stringArray.Count; i++)
        {
            stringArray[i] = EditorGUILayout.TextField($"Elemento {i}", stringArray[i]);
        }

        if (GUILayout.Button("Agregar Linea"))
        {
            stringArray.Add("");
        }

        if (stringArray.Count > 0)
        {
            if (GUILayout.Button("Eliminar Linea"))
            {

                stringArray.RemoveAt(stringArray.Count-1);
            }
        }
        


        selectedSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", selectedSprite, typeof(Sprite), false);

        if (selectedSprite != null)
        {
            GUILayout.Label(new GUIContent(selectedSprite.texture), GUILayout.Width(100), GUILayout.Height(100));
        }

        GUILayout.Space(10);
        GUILayout.Label("Crear un nuevo ScriptableObject", EditorStyles.boldLabel);


        if (GUILayout.Button("Crear Nuevo Asset"))
        {
            CreateNewAsset(assetName, selectedSprite, stringArray.ToArray());
        }

    }

    void CreateNewAsset(string name, Sprite s, string[] l)
    {
        NodoDialogo newAsset = ScriptableObject.CreateInstance<NodoDialogo>();
        newAsset.lineas = l;
        newAsset.portrait = s;
        string path = "Assets/DialogSistem/Nodos/" + name + ".asset"; // Ruta donde se guardará

        AssetDatabase.CreateAsset(newAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Nuevo ScriptableObject creado en {path}");
    }
}
