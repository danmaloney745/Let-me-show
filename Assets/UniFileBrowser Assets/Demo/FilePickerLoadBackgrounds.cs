using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class FilePickerLoadBackgrounds : MonoBehaviour
{
    string message = "";
    float alpha = 1.0f;
    char pathChar = '/';
    string imageSelect;
    private static string fileName = "Standard.jpg";
    List<string> children = new List<string>();
    // imageImport sn = gameObject.GetComponent<imageImport>();
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            pathChar = '\\';
        }
    }

    void OnClick()
    {
        Debug.Log("xfzfd");
        enabled = true;

    }

    void OnGUI()
    {
        /* if (GUI.Button(new Rect(100, 50, 95, 35), "Open"))
         {
             if (UniFileBrowser.use.allowMultiSelect)
             {
                 UniFileBrowser.use.OpenFileWindow(OpenFiles);
             }
             else {
                 UniFileBrowser.use.OpenFileWindow(OpenFile);
             }
         }*/
        if (GUI.Button(new Rect(100, 125, 95, 35), "Save"))
        {
            UniFileBrowser.use.SaveFileWindow(SaveFile);
        }
        if (GUI.Button(new Rect(100, 200, 95, 35), "Open Folder"))
        {
            UniFileBrowser.use.OpenFolderWindow(true, OpenFolder);
        }
        var col = GUI.color;
        col.a = alpha;
        GUI.color = col;
        GUI.Label(new Rect(100, 275, 500, 1000), message);
        col.a = 1.0f;
        GUI.color = col;
    }
    /*
    void OpenFile(string pathToFile)
    {
        var fileIndex = pathToFile.LastIndexOf(pathChar);
        message = "You selected file: " + pathToFile.Substring(fileIndex + 1, pathToFile.Length - fileIndex - 1);
        Fade();
    }

    void OpenFiles(string[] pathsToFiles)
    {
        message = "You selected these files:\n";
        for (var i = 0; i < pathsToFiles.Length; i++)
        {
            var fileIndex = pathsToFiles[i].LastIndexOf(pathChar);
            message += pathsToFiles[i].Substring(fileIndex + 1, pathsToFiles[i].Length - fileIndex - 1) + "\n";
        }
        Fade();
    }
*/
    void SaveFile(string pathToFile)
    {
        var fileIndex = pathToFile.LastIndexOf(pathChar);
        message = "You're saving file: " + pathToFile.Substring(fileIndex + 1, pathToFile.Length - fileIndex - 1);
        fileName = pathToFile.Substring(fileIndex + 1, pathToFile.Length - fileIndex - 1);
        imageSelect = pathToFile;
        if (pathToFile != null)
        {
            LoadPNG(pathToFile);
            children.Add(imageSelect);
            Debug.Log(imageSelect);
        }
        Fade();

    }
    StreamWriter fileWriter = null;
    private SpriteRenderer spriteRenderer;
    private Texture2D LoadPNG(string filePath)
    {

        //this.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load(filePath, typeof(Sprite)) as Sprite;
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
           // File.WriteAllBytes(Application.persistentDataPath + "/LayoutImages/"+ fileData + "1", fileData);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
      
            Debug.Log("In the Tex: " + tex.LoadImage(fileData));
        }

        Rect rec = new Rect(0, 0, tex.width, tex.height);
        Sprite.Create(tex, rec, new Vector2(0, 0), 1);
        //spriteRenderer.sprite = Sprite.Create(tex, rec, new Vector2(0, 0), .01f);
        GetComponent<Renderer>().material.mainTexture = tex;
        SaveTextureToFile(tex, filePath);
        //this.gameObject.transform.GetComponent<Image>().sprite= spriteRenderer.sprite ;
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, rec, new Vector2(0, 0), 1);
        return tex;
    }

    public static void SaveTextureToFile(Texture2D texture, string filename)
    {
        byte[] bytes;
        bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/LayoutImages/" + fileName, bytes);
        System.IO.FileStream fileSave;
        fileSave = new FileStream(Application.dataPath + "/Resources/Stickers/Standard/" + fileName, FileMode.Create);

        System.IO.BinaryWriter binary;
        binary = new BinaryWriter(fileSave);
        binary.Write(bytes);
        fileSave.Close();
    }

    void OpenFolder(string pathToFolder)
    {
        message = "You selected folder: " + pathToFolder;
        Fade();
    }

    void Fade()
    {
        StopCoroutine("FadeAlpha"); // Interrupt FadeAlpha if it's already running, so only one instance at a time can run
        StartCoroutine("FadeAlpha");
    }

    IEnumerator FadeAlpha()
    {
        alpha = 1.0f;
        yield return new WaitForSeconds(5.0f);
        for (alpha = 1.0f; alpha > 0.0f; alpha -= Time.deltaTime / 4)
        {
            yield return null;
        }
        message = "";
    }
}
