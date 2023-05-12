/************************************************************
	文件：UnityToSVN.cs
	作者：Plane
	邮箱：1785275942@qq.com
	日期：2015/10/18 12:01
	功能：整合SVN命令到Unity编辑器
*************************************************************/

using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace Extension {
    public class UnityToSVN
    {
        private const string Menu_Commit = "SVN/Commit Selected";
        private const string Menu_Commit_All = "SVN/Commit All";
        private const string Menu_Update = "SVN/Update Selected";
        private const string Menu_Update_All = "SVN/Update All";
        private const string Menu_Log = "SVN/Log Selected";
        private const string Menu_Log_All = "SVN/Log All";
        private const string Menu_Cleanup = "SVN/Cleanup";

        private const string Menu_LnkCommit = "Assets/SVN Tool/Commit";
        private const string Menu_LnkUpdate = "Assets/SVN Tool/Update";
        private const string Menu_LnkRevert = "Assets/SVN Tool/Revert";
        private const string Menu_LnkCleanup = "Assets/SVN Tool/Cleanup";

        #region Menu Item
        [MenuItem(Menu_Commit)]
        public static void SVNCommit()
        {
            string path = GetSelObjPath(true);
            if (path != "")
            {
                SVNCmd("commit", path);
            }
            else
            {
                SVNCommitAll();
            }
        }

        [MenuItem(Menu_Commit_All)]
        public static void SVNCommitAll()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.Length - 7);
            SVNCmd("commit", path);
        }

        [MenuItem(Menu_Update)]
        public static void SVNUpdate()
        {
            string path = GetSelObjPath(true);
            if (path != "")
            {
                SVNCmd("update", path);
            }
            else
            {
                SVNUpdateAll();
            }
        }

        [MenuItem(Menu_Update_All)]
        public static void SVNUpdateAll()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.Length - 7);
            SVNCmd("update", path);
        }

        [MenuItem(Menu_Log)]
        public static void SVNLog()
        {
            string path = GetSelObjPath(true);
            if (path != "")
            {
                SVNCmd("log", GetSelObjPath(true));
            }
            else
            {
                SVNLogAll();
            }
        }

        [MenuItem(Menu_Log_All)]
        public static void SVNLogAll()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.Length - 7);
            SVNCmd("log", path);
        }

        [MenuItem(Menu_Cleanup)]
        public static void SVNCleanup()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.Length - 7);
            SVNCmd("cleanup", path);
        }
        #endregion

        #region Shortcut Menu Item
        [MenuItem(Menu_LnkCommit, priority = 0, validate = true)]
        private static bool ValidateSVNLnkCommit()
        {
            return Selection.assetGUIDs.Length != 0;
        }

        [MenuItem(Menu_LnkCommit, priority = 0)]
        public static void SVNLnkCommit()
        {
            string[] paths = GetSelectionFilePaths();
            if (paths.Length != 0)
            {
                SVNCmd("commit", paths);
            }
        }

        [MenuItem(Menu_LnkUpdate, priority = 1, validate = true)]
        private static bool ValidateSVNLnkUpdate()
        {
            return Selection.assetGUIDs.Length != 0;
        }

        [MenuItem(Menu_LnkUpdate, priority = 1)]
        public static void SVNLnkUpdate()
        {
            string[] paths = GetSelectionFilePaths();
            if (paths.Length != 0)
            {
                SVNCmd("update", paths);
            }
        }

        [MenuItem(Menu_LnkRevert, priority = 100, validate = true)]
        private static bool ValidateSVNLnkRevert()
        {
            return Selection.assetGUIDs.Length != 0;
        }

        [MenuItem(Menu_LnkRevert, priority = 100)]
        public static void SVNLnkRevert()
        {
            string[] paths = GetSelectionFilePaths();
            if (paths.Length != 0)
            {
                SVNCmd("revert", paths);
            }
        }

        [MenuItem(Menu_LnkCleanup, priority = 101, validate = true)]
        private static bool ValidateSVNLnkCleanup()
        {
            foreach (var item in GetSelectionFilePaths(false))
            {
                if (!Directory.Exists(item))
                    return false;
            }
            return true;
        }

        [MenuItem(Menu_LnkCleanup, priority = 101)]
        public static void SVNLnkCleanup()
        {
            string[] paths = GetSelectionFilePaths(false);
            if (paths.Length != 0)
            {
                SVNCmd("cleanup", paths);
            }
        }

        private static string[] GetSelectionFilePaths(bool bSelectMeta = true)
        {
            List<string> assetPaths = new List<string>();
            foreach (var guid in Selection.assetGUIDs)
            {
                if (string.IsNullOrEmpty(guid))
                {
                    continue;
                }

                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                {
                    assetPaths.Add(path);
                    if (bSelectMeta)
                    {
                        if (File.Exists(path + ".meta"))
                        {
                            assetPaths.Add(path + ".meta");
                        }
                        
                    }
                        
                }
            }
            return assetPaths.ToArray();
        }

        #endregion

        public static void SVNCmd(string command, string path)
        {
            string cmd = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" /closeonend 2";
            cmd = string.Format(cmd, command, path);
            ProcessStartInfo proc = new ProcessStartInfo("cmd.exe", cmd);
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc);
        }

        public static void SVNCmd(string command, params string[] paths)
        {
            string path = paths[0];
            for (int i = 1 ; i < paths.Length; i++)
            {
                path += "*" + paths[i];
            }
            string cmd = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" /closeonend 2";
            
            cmd = string.Format(cmd, command, path);
            ProcessStartInfo proc = new ProcessStartInfo("cmd.exe", cmd);
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc);
        }

        private static string GetSelObjPath(bool firstOne = false)
        {
            string path = string.Empty;
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                path += ConvertToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i]));
                if (firstOne) break;
                path += "*";
                path += ConvertToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i])) + ".meta";
                path += "*";
            }
            return path;
        }

        public static string ConvertToFilePath(string path)
        {
            string m_path = Application.dataPath;
            m_path = m_path.Substring(0, m_path.Length - 6);
            m_path += path;
            return m_path;
        }
    }
}

