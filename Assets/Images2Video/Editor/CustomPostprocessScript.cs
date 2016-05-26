#if UNITY_IPHONE
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using Images2Video.Editor;

public class CustomPostprocessScript {
	[PostProcessBuild(100)]//To make sure the other scripts are executed.
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject) {
		UnityEngine.Debug.Log("----Executing post process build phase----");
		PBXModifier mod = new PBXModifier();
		string pbxproj = Path.Combine(pathToBuildProject, "Unity-iPhone.xcodeproj/project.pbxproj");
		if (!File.Exists(pbxproj))
			return;
			
		var lines = mod.applyTo(pbxproj);
		
		File.WriteAllLines(pbxproj, lines);
		
		UnityEngine.Debug.Log("----Finished executing post process build phase");
	}
}
#endif
	 