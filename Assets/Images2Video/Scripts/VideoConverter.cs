using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class VideoConverter : MonoBehaviour
{
	/// <summary>
	/// Inits the converter variables.
	/// </summary>
	/// <param name="gameObjName">The attached game object name.</param>
	/// <param name="callbackMethod">The callback method which will be called by UnitySendMessage.</param>
	/// <param name="ext">The extension name of the input images.</param>
	/// <param name="audioPath">The audio path which you wnat to attach, if you don't want to attach the audio file, set as empty string</param>
	/// <param name="width">The video width.</param>
	/// <param name="height">The video height.</param>
	/// <param name="frameRate">The video frame rate.</param>
	/// <param name="shortestClip">Enable the shortest clip while generating the video file.</param> 
	public void InitVars(String gameObjName, String callbackMethod, String ext, String audioPath, int width, int height, int frameRate, bool shortestClip)
	{
#if UNITY_IOS
		InitConverter(gameObjName, callbackMethod, audioPath, width, height, frameRate, shortestClip);
#elif UNITY_ANDROID
	    using (AndroidJavaClass _androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			AndroidJavaObject _activity = _androidClass.GetStatic<AndroidJavaObject>("currentActivity");
			Debug.Log("_activity ==>" + _activity);
	        using (AndroidJavaObject _javaClass = new AndroidJavaObject("tw.com.championtek.videoconverter.VideoConverter", _activity)) {
				object[] _params = {gameObjName, callbackMethod, ext, audioPath, width, height, frameRate, shortestClip};
				Debug.Log("params => " + _params);
				_javaClass.Call("initVars", _params);
	        }
	    }
#endif
}

	/// <summary>
	/// Converts the image to video.
	/// </summary>
	/// <param name="bytes">The images data bytes array.</param>
	/// <param name="frameIndicator">The image frame indicator which indicate the position of the image in the video.</param>
	public void ConvertImageToVideo(byte[] bytes, int frameIndicator) {
#if UNITY_IOS
	    StartCoroutine(Convertering(bytes, frameIndicator));
#elif UNITY_ANDROID
	        using (AndroidJavaClass _androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
	            AndroidJavaObject _activity = _androidClass.GetStatic<AndroidJavaObject>("currentActivity");
	            using (AndroidJavaObject _javaClass = new AndroidJavaObject("tw.com.championtek.videoconverter.VideoConverter", _activity)) {
	            object[] _params = {Convert.ToBase64String(bytes), frameIndicator};
	            _javaClass.Call("savePNGDataToAppDir", _params);
	        }
	    }
#endif
	}

	/// <summary>
	/// Converting images finished, and after generating the video, the callback method which the developer pass through InitVars will be trigger.
	/// </summary>
	public void ConvertImagesFinished() {
#if UNITY_IOS
	    FinishedEncodingVideo();
#elif UNITY_ANDROID
	    using (AndroidJavaClass _androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
	        AndroidJavaObject _activity = _androidClass.GetStatic<AndroidJavaObject>("currentActivity");
	        using (AndroidJavaObject _javaClass = new AndroidJavaObject("tw.com.championtek.videoconverter.VideoConverter", _activity)) {
	            _javaClass.Call("convertImagesToVideo", Application.persistentDataPath);
	        }
	    }
#endif
	}

#if UNITY_IOS
	IEnumerator Convertering(byte[] bytes, int indicator) {
	    EncodeVideoData(bytes, bytes.Length, indicator);
	    yield return null;
	}
#endif
  
	public void DisplayProgress(String title, String message) {
		/* Display progress */
#if UNITY_IPHONE
		
#elif UNITY_ANDROID
		using (AndroidJavaClass _androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			AndroidJavaObject _activity = _androidClass.GetStatic<AndroidJavaObject>("currentActivity");
			using (AndroidJavaObject _javaClass = new AndroidJavaObject("tw.com.championtek.videoconverter.VideoConverter", _activity)) {
				object[] _params = {title, message};
				_javaClass.Call("displayProgressDialog", _params);
			}
		}
#endif
	}
	
	public void DisplayMessage(String message) {
#if UNITY_IOS
		DisplayAlertView(message);
#elif UNITY_ANDROID
		using (AndroidJavaClass _androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			AndroidJavaObject _activity = _androidClass.GetStatic<AndroidJavaObject>("currentActivity");
			using (AndroidJavaObject _javaClass = new AndroidJavaObject("tw.com.championtek.videoconverter.VideoConverter", _activity)) {
				object[] _params = {message};
				_javaClass.Call("displayToastMessage", _params);
			}
		}
#endif
	}
	
#if UNITY_IOS
[DllImport ("__Internal")]
/// <summary>
/// Inits the converter.
/// </summary>
/// <param name="obj">Attached object</param>
/// <param name="method">Callback method name</param>
/// <param name="param">Parameters</param>
/// <param name="width">Captured Image Width</param>
/// <param name="height">Captured Image Height</param>
/// <param name="fps">Fps</param>
/// <param name="shortestClip">Enable the shortest clip while generating the video</param> 
	private static extern void InitConverter(string obj, string method, string audioPath, int width, int height, int fps, bool shortestClip);

[DllImport ("__Internal")]
/// <summary>
/// Encodes the video data.
/// </summary>
/// <param name="bytes">Bytes.</param>
/// <param name="byteLength">Byte length.</param>
/// <param name="frameIndicator">Frame indicator.</param>
	private static extern void EncodeVideoData(byte[] bytes, int byteLength, int frameIndicator);

[DllImport ("__Internal")]
/// <summary>
/// Finish to encode the video.
/// </summary>
	private static extern void FinishedEncodingVideo();
	
	[DllImport ("__Internal")]
	private static extern void DisplayAlertView(string message);
#endif
}