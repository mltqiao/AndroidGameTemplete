/****************************************************
	文件：ScreenShotWindow.cs
	作者：Zhangying
	邮箱：zhy18125@gmail.com
	日期：2022/5/7 17:16:40
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ScreenShotWindow : EditorWindow
{
    private static Camera m_Camera;
    private static string filePath;
    private bool m_IsEnableAlpha = false;
    private CameraClearFlags m_CameraClearFlags;

    [MenuItem("扩展功能/屏幕截图")]
    private static void Init()
    {
        m_Camera = Camera.main;
        filePath = SimplifyPath(Application.dataPath + "/../");

        ScreenShotWindow window = GetWindowWithRect<ScreenShotWindow>(new Rect(0, 0, 450, 180));
        window.titleContent = new GUIContent("屏幕截图");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        m_Camera = EditorGUILayout.ObjectField("选择摄像机:", m_Camera, typeof(Camera), true) as Camera;

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("保存路径:", GUILayout.Width(75f));
        GUILayout.Label(filePath);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("自定义位置"))
        {
            filePath = EditorUtility.OpenFolderPanel("", "", "");
        }

        m_IsEnableAlpha = EditorGUILayout.Toggle("是否开启透明通道", m_IsEnableAlpha);
        EditorGUILayout.Space();
        
        EditorGUILayout.Space();
        if (GUILayout.Button("打开导出文件夹"))
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("<color=red>" + "没有选择截图保存位置" + "</color>");
                return;
            }
            Application.OpenURL("file://" + filePath);
        }

        if (GUILayout.Button("截图"))
        {
            TakeShot();
        }
    }

    private void TakeShot()
    {
        if (m_Camera == null)
        {
            Debug.LogError("<color=red>" + "没有选择摄像机" + "</color>");
            return;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("<color=red>" + "没有选择截图保存位置" + "</color>");
            return;
        }

        m_CameraClearFlags = m_Camera.clearFlags;
        if (m_IsEnableAlpha)
        {
            m_Camera.clearFlags = CameraClearFlags.Depth;
        }

        int resolutionX = (int)Handles.GetMainGameViewSize().x;
        int resolutionY = (int)Handles.GetMainGameViewSize().y;
        RenderTexture rt = new RenderTexture(resolutionX, resolutionY, 24);
        m_Camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resolutionX, resolutionY, TextureFormat.RGB24, false);
        m_Camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resolutionX, resolutionY), 0, 0);
        m_Camera.targetTexture = null;
        RenderTexture.active = null;
        m_Camera.clearFlags = m_CameraClearFlags;
        //Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string fileName = filePath + "/" + $"{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}" + ".png";
        File.WriteAllBytes(fileName, bytes);


        Debug.Log("截图成功");
    }

    [MenuItem("扩展功能/带UI的屏幕截图 &C")]
    public static void CaptureScreenByRT()
    {
        //Application.CaptureScreenshot(fileName, 0);
        UnityEngine.ScreenCapture.CaptureScreenshot($"{Application.dataPath}/../{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}" + ".png");
        //System.IO.File.WriteAllBytes(fileName, bytes);
    }

    private static string SimplifyPath(string path)
    {
        string[] names = path.Split('/');
        List<string> queue = new List<string>();
        foreach (var name in names)
        {
            if (string.IsNullOrEmpty(name) || name.Equals("."))
                continue;
            else if (name.Equals(".."))
            {
                if (queue.Count > 0)
                    queue.RemoveAt(queue.Count - 1);
            }
            else
                queue.Add(name);
        }
        return queue.Count == 0 ? "/" : string.Join("/", queue.ToArray());
    }

    [MenuItem("扩展功能/Clear Data")]
    public static void ClearData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All Data Cleared");
    }
}
