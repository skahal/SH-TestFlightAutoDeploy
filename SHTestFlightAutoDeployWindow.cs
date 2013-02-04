#region Usings
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
#endregion

/// <summary>
/// The editor window to Skahal's TestFlight Auto Deploy.
/// </summary>
public class SHTestFlightAutoDeployWindow : EditorWindow
{
	#region Initialize
	/// <summary>
	/// Build the menu item and entry point.
	/// </summary>
	[MenuItem("Skahal/TestFlight Auto Deploy")]
	private static void Init ()
	{	
		var instance = GetWindow<SHTestFlightAutoDeployWindow> ();
		instance.ShowPopup ();
	}
	#endregion
	
	#region Fields
	private string m_projectName;
	private string m_outputPath;
	private string m_appFileName;
	private string m_sdkVersion = "iphoneos6.0";
	private bool m_autoIncrementBundleVersion = true;
	private string m_apiToken;
	private string m_teamToken;
	private string m_notes;
	private bool m_notify;
	private string m_distributionLists;
	#endregion
	
	#region Constructors
	/// <summary>
	/// Initializes a new instance of the <see cref="SHTestFlightAutoDeployWindow"/> class.
	/// </summary>
	public SHTestFlightAutoDeployWindow ()
	{
		m_projectName = Application.dataPath.Replace ("/Assets", "");
		m_projectName = m_projectName.Substring (m_projectName.LastIndexOf (Path.DirectorySeparatorChar) + 1);

		title = "TestFlight Auto Deploy";
		autoRepaintOnSceneChange = true;
		minSize = new Vector2 (480, 280);
		LoadPrefs ();	
	}
	
	#endregion
	
	#region Methods	
	/// <summary>
	/// Draws the window's GUI.
	/// </summary>
	private void OnGUI ()
	{
		m_appFileName = EditorGUILayout.TextField ("App file name", m_appFileName);
		m_outputPath = EditorGUILayout.TextField ("Output path", m_outputPath);
		m_sdkVersion = EditorGUILayout.TextField ("SDK version", m_sdkVersion);
		m_autoIncrementBundleVersion = EditorGUILayout.Toggle ("Auto increment version", m_autoIncrementBundleVersion);
		m_apiToken = EditorGUILayout.TextField ("API token", m_apiToken);
		m_teamToken = EditorGUILayout.TextField ("Team token", m_teamToken);
		EditorGUILayout.LabelField ("Notes");
		m_notes = EditorGUILayout.TextArea (m_notes, GUILayout.Height (50));
		m_notify = EditorGUILayout.Toggle ("Notify", m_notify);
		m_distributionLists = EditorGUILayout.TextField ("Distribution lists", m_distributionLists);
	
		if (GUILayout.Button ("Send to TestFlight")) {
			SavePrefs ();
			PublishToTestFlight ();
		}
	}
	
	private void PublishToTestFlight ()
	{
		BuildPlayer ();
		BuildXcodeProject ();		
		BuildIpa ();
		SendToTestFlight ();
		
		EditorUtility.ClearProgressBar();
		ShowStatus ("Done.");
		
		EditorGUIUtility.ExitGUI ();	
	}

	private void BuildPlayer ()
	{
		EditorUtility.DisplayProgressBar (title, "Building player...", 0.1f);
	
		var scenes = new List<string> ();
		
		foreach (var scene in EditorBuildSettings.scenes) {
			if (scene.enabled) {
				scenes.Add (scene.path);
			}
		}
		
		if (m_autoIncrementBundleVersion) {
			var versionParts = PlayerSettings.bundleVersion.Split ('.');
			
			var newVersion = string.Empty;
			int minor = 0;
			
			if (versionParts.Length > 0) {
				int index = versionParts.Length - 1;
				newVersion = string.Join (".", versionParts, 0, index);
				minor = int.Parse (versionParts [index]);
			}
			
			newVersion += "." + (++minor);
			PlayerSettings.bundleVersion = newVersion;
		}
		
		BuildPipeline.BuildPlayer (scenes.ToArray (), m_outputPath, BuildTarget.iPhone, BuildOptions.None);
	}

	private void BuildXcodeProject ()
	{
		Run ("Building Xcode project...", 
			0.25f, 0.5f,
			"/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild", 
			"-target Unity-iPhone -sdk {0} -configuration Release", 
			m_sdkVersion);
	}
	
	private void BuildIpa ()
	{
		Run ("Building .IPA...",
			0.5f, 0.75f,
			"/Applications/Xcode.app/Contents/Developer/usr/bin/xcrun", 
			"-sdk iphoneos PackageApplication -v build/{0}.app -o {1}/build/{0}.ipa", 
			m_appFileName,
			m_outputPath);
	}

	private void SendToTestFlight ()
	{
		Run ("Sending to TestFlight ...",
			0.75f,
			1f,
			"curl", 
			"http://testflightapp.com/api/builds.json -F file=@{0}/build/{1}.ipa -F api_token='{2}' -F team_token='{3}' -F notes='{4}' -F notify={5} -F distribution_lists='{6}'", 
			m_outputPath,
			m_appFileName,
			m_apiToken,
			m_teamToken,
			m_notes,
			m_notify,
			m_distributionLists);
	}
	
	private void OnLostFocus ()
	{
		SavePrefs ();
	}
	
	private void OnDestroy ()
	{
		SavePrefs ();
	}
	#endregion
	
	#region Helpers	
	private string GetKey (string key)
	{
		var fullKey = string.Format ("SHTFAD_{0}_{1}", m_projectName, key);
		return fullKey;
	}
	
	private string GetString (string key)
	{
		return EditorPrefs.GetString (GetKey(key));
	}
	
	private bool GetBool (string key)
	{
		return EditorPrefs.GetBool (GetKey (key));
	}
				
	private void SetString (string key, string value)
	{
		EditorPrefs.SetString (GetKey (key), value);
	}
	
	private void SetBool (string key, bool value)
	{
		EditorPrefs.SetBool (GetKey (key), value);
	}
				
	private void LoadPrefs ()
	{
		m_appFileName = GetString("appFileName");
		m_outputPath = GetString("outputPath");
		m_sdkVersion = GetString("sdkVersion");
		m_autoIncrementBundleVersion = GetBool ("autoIncrementBundleVersion");
		m_apiToken = GetString("apiToken");
		m_teamToken = GetString("teamToken");
		m_notes = GetString("notes");
		m_notify = GetBool ("notify");
		m_distributionLists = GetString("distributionLists");
	}
	
	private void SavePrefs ()
	{
		SetString("appFileName", m_appFileName);
		SetString("outputPath", m_outputPath);
		SetString("sdkVersion", m_sdkVersion);
		SetBool("autoIncrementBundleVersion", m_autoIncrementBundleVersion);
		SetString("apiToken", m_apiToken);
		SetString("teamToken", m_teamToken);
		SetString("notes", m_notes);
		SetBool("notify", m_notify);
		SetString("distributionLists", m_distributionLists);
	}
	
	private void ShowStatus (string text)
	{
		ShowNotification (new GUIContent (text));
	}
	
	private void Run (string runTitle, float fromProgress, float toProgress, string command, string argsFormat, params object[] args)
	{
		EditorUtility.DisplayProgressBar (title, runTitle, fromProgress);
		
		var ps = new System.Diagnostics.Process ();	
		var argsFormatted = string.Format (argsFormat, args);
		ps.StartInfo = new ProcessStartInfo (command, argsFormatted);
		ps.StartInfo.WorkingDirectory = m_outputPath;
		ps.StartInfo.RedirectStandardOutput = true;
		ps.StartInfo.UseShellExecute = false;
		ps.Start ();
		while (true) {		
			byte[] buffer = new byte[256];
			var ar = ps.StandardOutput.BaseStream.BeginRead (buffer, 0, 256, null, null);
			ar.AsyncWaitHandle.WaitOne ();
			var bytesRead = ps.StandardOutput.BaseStream.EndRead (ar);
			if (bytesRead > 0) {
				if (fromProgress < toProgress) {
					fromProgress += 0.00005f;
					EditorUtility.DisplayProgressBar (title, runTitle, fromProgress);
				}
			} else {
				ps.WaitForExit ();
				break;
			}
		}	
	}
	#endregion
}