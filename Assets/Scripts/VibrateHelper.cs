using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateHelper : MonoBehaviour
{
    public static void vibrateSmall()
    {
        Debug.Log("Vibrate");
    }
    static public bool Acitve { get { return VibrateController.Vibratable; } }
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void _vibrateSmall();
        [DllImport("__Internal")]
        static extern void _vibrateMedium();
        [DllImport("__Internal")]
        static extern void _vibrateLarge();

        public static void vibrateSmall()
        {
            if(Acitve)
#if !UNITY_EDITOR
            _vibrateSmall();
#endif
        }

        public static void vibrateMedium()
        {
#if !UNITY_EDITOR
    if(Acitve)
            _vibrateMedium();
#endif
        }

        public static void vibrateLarge()
        {
#if !UNITY_EDITOR
    if(Acitve)
                _vibrateLarge();
#endif
        }

#elif UNITY_ANDROID
    static AndroidJavaObject _vibrator;
    static AndroidJavaObject vibrateManager
    {
        get
        {
            var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            _vibrator = currentActivity.Call<AndroidJavaObject>("getApplication").Call<AndroidJavaObject>("getSystemService", "vibrator");
            return _vibrator;
        }
    }

    public static void vibrateSmall()
    {
        if (!Application.isEditor && Acitve)
            vibrateManager.Call("vibrate", 20L);
    }

    public static void vibrateMedium()
    {
        if (!Application.isEditor && Acitve)
            vibrateManager.Call("vibrate", 45L);
    }

    public static void vibrateLarge()
    {
        if (!Application.isEditor && Acitve)
            vibrateManager.Call("vibrate", 85L);
    }
#endif
}