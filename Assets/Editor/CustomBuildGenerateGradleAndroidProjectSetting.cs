/****************************************************
	文件：CustomBuildGenerateGradleAndroidProjectSetting.cs
	作者：Zhangying
	邮箱：zhy18125@gmail.com
	日期：#CreateTime#
	功能：Nothing
*****************************************************/

using System.IO;
using UnityEditor.Android;
using UnityEngine;

public class CustomBuildGenerateGradleAndroidProjectSetting : IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        // 同种插件的优先级
        get { return 999; }
    }
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        //Debug.Log("Bulid path : " + path);

        string gradlePropertiesFile = path + "/gradle.properties";
        //ModifyGradlePropertiesFile(gradlePropertiesFile);

        string buildGradleFile = path + "/build.gradle";
        ModifyBuildGradleFile(buildGradleFile);
    }

    private static void ModifyGradlePropertiesFile(string gradlePropertiesFile)
    {
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        using (StreamWriter writer = File.CreateText(gradlePropertiesFile))
        {
            //writer.WriteLine("org.gradle.jvmargs=-Xmx**JVM_HEAP_SIZE**M");
            //writer.WriteLine("org.gradle.parallel=true");
            //writer.WriteLine("android.enableR8=**MINIFY_WITH_R_EIGHT**");
            //writer.WriteLine("unityStreamingAssets=.unity3d**STREAMING_ASSETS**");
            writer.WriteLine("**ADDITIONAL_PROPERTIES**");
            writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
            writer.WriteLine("android.useAndroidX=true");
            writer.WriteLine("android.enableJetifier=true");
            writer.Flush();
        }
    }

    private static void ModifyBuildGradleFile(string buildGradleFile)
    {
        string data;
        using (FileStream stream = File.Open(buildGradleFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            StreamReader reader = new StreamReader(stream);
            data = reader.ReadToEnd();
            
            reader.Close();
            data = data.Replace("https://maven.google.com", "https://maven.aliyun.com/repository/google").Replace("\n", "\r\n");
        }
        File.Delete(buildGradleFile);
        using (StreamWriter writer = File.CreateText(buildGradleFile))
        {
            writer.Write(data);
            writer.Flush();
        }
    }
}
