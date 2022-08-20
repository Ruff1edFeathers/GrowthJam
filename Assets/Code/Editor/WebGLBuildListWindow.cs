using System.IO;
using System.Diagnostics;

using UnityEngine;
using UnityEditor;

public class WebGLBuildListWindow : EditorWindow
{
    private string m_RunningBuild;
    private Process m_WebServer;
    private string m_TargetDirectory;
    private int m_PortNumber;

    [MenuItem("Window/WebGL Builds")]
    static void OpenWindow()
    {
        WebGLBuildListWindow Window = GetWindow<WebGLBuildListWindow>();
        Window.titleContent = new GUIContent("WebGL Builds");
        Window.Show();
    }

    private void OnEnable()
    {
        if(EditorPrefs.HasKey("WebGL_BuildsTarget"))
        {
            m_TargetDirectory = EditorPrefs.GetString("WebGL_BuildsTarget");
            m_PortNumber      = EditorPrefs.GetInt("WebGL_PortNumber");
            return;
        }

        m_TargetDirectory = EditorApplication.applicationPath;
        m_PortNumber      = 8080;

        EditorPrefs.SetString("WebGL_BuildsTarget", m_TargetDirectory);
        EditorPrefs.SetInt("WebGL_BuildsTarget", m_PortNumber);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("WebGL Builds", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        if(m_WebServer != null)
        {
            if(m_WebServer.HasExited)
            {
                m_WebServer = null;
                Repaint();
                return;
            }

            EditorGUILayout.LabelField("Web Server:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Running Build: {m_RunningBuild}");
            EditorGUILayout.LabelField("Build Hosted at:");
            EditorGUILayout.LabelField($"http://localhost:{m_PortNumber}/");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Open Webpage"))
            {
                Application.OpenURL($"http://localhost:{m_PortNumber}/");
            }
            if (GUILayout.Button("Kill Server"))
            {
                m_WebServer.Kill();
                m_WebServer = null;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //Web Server is running!
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Builds Folder:");
            string NewTarget = EditorGUILayout.DelayedTextField(m_TargetDirectory);
            if (GUILayout.Button("Select"))
                NewTarget = EditorUtility.OpenFolderPanel("WebGL Builds Root", "", "");

            if (NewTarget != m_TargetDirectory)
            {
                EditorPrefs.SetString("WebGL_BuildsTarget", NewTarget);
                m_TargetDirectory = NewTarget;
                Repaint();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("PortNumber:");
            int NewPortNumber = EditorGUILayout.DelayedIntField(m_PortNumber);
            if (NewPortNumber != m_PortNumber)
            {
                EditorPrefs.SetInt("WebGL_PortNumber", NewPortNumber);
                m_PortNumber = NewPortNumber;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (!Directory.Exists(m_TargetDirectory))
        {
            EditorGUILayout.HelpBox("Build Folder doesn't exist", MessageType.Error);
            return;
        }

        if (File.Exists(m_TargetDirectory))
        {
            EditorGUILayout.HelpBox("Build Folder isn't a directory", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Builds:", EditorStyles.boldLabel);

        string[] Builds = Directory.GetDirectories(m_TargetDirectory);
        int      Builds_Len = Builds.Length;

        EditorGUI.indentLevel++;
        for(int i = 0; i < Builds_Len; i++)
        {
            if (!File.Exists($"{Builds[i]}/index.html"))
            {
                //Invalid build, no webpage inside
                continue;
            }

            string BuildName = Path.GetFileName(Builds[i]);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{(i + 1)}) {BuildName}");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Play"))
            {
                string WebServerPath = $"{EditorApplication.applicationContentsPath}/PlaybackEngines/WebGLSupport/BuildTools/SimpleWebServer.exe";
                UnityEngine.Debug.Log($"Trying to Start Webserver at: {WebServerPath}");
                if (File.Exists(WebServerPath))
                {
                    m_RunningBuild = BuildName;
                    m_WebServer = Process.Start(new ProcessStartInfo()
                    {
                        FileName  = WebServerPath,
                        Arguments = $"\"{Builds[i]}\" {m_PortNumber}",
                        CreateNoWindow = false
                    });
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("WebGL Builds", "Failed to start Web Server, Are you sure WebGL module is installed?\nCheck the Unity Hub", "Alrighty");
                }
            }
            if(GUILayout.Button("Delete"))
            {
                if(EditorUtility.DisplayDialog("WebGL Builds", $"This will permentantly delete:\n{BuildName}\nAre you sure?", "Yep", "Nope"))
                {
                    Directory.Delete(Builds[i], true);
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
    }

}
