using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System;
using System.IO;

public class ScreenshotExample : MonoBehaviour
{
	
	Vector3 vrotate;
    VideoConverter _videoConverter;
    float timeLeft = 0.0f;
    private int _frameRate = 10;// Setup fps
    
    private bool _keepGoing;
    
    private string _folder = "Screenshot";
    
    private string _path;
    
    private int _imageWidth;
    private int _imageHeight;
    
	private int _count = 0;
	
	// Use this for initialization
	void Start ()
    {
		Screen.autorotateToPortrait = true;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		
		Screen.orientation = ScreenOrientation.AutoRotation;
		
		// Use screen size as image size
        if (Application.platform != RuntimePlatform.OSXEditor) {
            _imageWidth = Screen.width;
            _imageHeight = Screen.height;
			
            _videoConverter = (VideoConverter)gameObject.GetComponent("VideoConverter");
        }
        
        int i = UnityEngine.Random.Range(1, 4);
        switch(i) {
            case 1:
                RotateLeft();
                break;
            case 2:
                RotateRight();
                break;
            case 3:
                RotateDown() ;
                break ;
            case 4:
                RotateUp();
                break;
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
        timeLeft += Time.deltaTime;
        if (timeLeft > .5)
        {
            if (!_keepGoing)
                return;

            string _name = String.Format("{0}/{1:D04}_shot.png", _folder, Time.frameCount);
            Debug.Log("screenshot file path : " + _name);
            Application.CaptureScreenshot(_name, 1);
            _count++;
            timeLeft = 0;

        }
	}
	
	// Here we use ienumerator to trigger www and wait until download complete.
	IEnumerator InitialAllProperties() {
		if (Application.platform != RuntimePlatform.OSXEditor) {	
		
			CreateDirectoryIfNotExist();
			// You can put the audio files into StreamingAssets folder or download from backend server
			// 1. Put the audio file in the StreamingAssets folder
			string base_path;
			#if UNITY_EDITOR
			base_path = "file:" + Application.dataPath + "/StreamingAssets";
			#elif UNITY_ANDROID
			base_path = "jar:file://" + Application.dataPath + "!/assets";
			#elif UNITY_IOS
			base_path = "file:" + Application.dataPath + "/Raw";
			#else
			//Desktop (Mac OS or Windows)
			base_path = "file:"+ Application.dataPath + "/StreamingAssets";
			#endif
			
			Debug.Log("read audio file => " + System.IO.Path.Combine(base_path, "success.mp3"));
			
			WWW www = new WWW(System.IO.Path.Combine(base_path, "success.mp3"));
			yield return www;
			
			// 2. Download the audio file from server
			// WWW www = new WWW("http://www.cupcake.tw/success.mp3");
			// yield return www;
			
			//Audio Path
			string file_path = "";
			
			if (string.IsNullOrEmpty(www.error)) {
				Debug.Log("read audio file successed! and length => " + www.bytes.Length);
				file_path = System.IO.Path.Combine(_path, "success.mp3");
				File.WriteAllBytes(file_path, www.bytes);
				Debug.Log("write audio file to " + file_path);
			}
			
			if (gameObject == null)
				_videoConverter.InitVars("default", "ConvertEnd", "png", file_path, _imageWidth, _imageHeight, _frameRate, true);
			else
				_videoConverter.InitVars(gameObject.name, "ConvertEnd", "png", file_path, _imageWidth, _imageHeight, _frameRate, true);
			
			Time.captureFramerate = _frameRate;
			_keepGoing = true;
		}
	}

    public void ScreenshotBegin() {
		StartCoroutine("InitialAllProperties");
    }
    
    public void ScreenshotEnd() {
        if (Application.platform != RuntimePlatform.OSXEditor) {
            ResetParameters();
            
			StopRotate();
			
			_videoConverter.DisplayProgress("Converting", "Processing");
			
			/* Prepare for convert processing */
            DirectoryInfo _info = new DirectoryInfo(_path);
            FileInfo[] _files = _info.GetFiles("*.png");
            int _indicator = 0 ;
            foreach (FileInfo _file in _files) {
				Debug.Log(">>>> file path - " + _file.FullName);
				
                byte[] _bytes = File.ReadAllBytes(_path + "/" + _file);
                _videoConverter.ConvertImageToVideo(_bytes, _indicator);
                
                _indicator++;
            }
			
			_videoConverter.ConvertImagesFinished();
        }
    }
    
    private void CreateDirectoryIfNotExist ()
    {
        _path = Path.Combine(Application.persistentDataPath, _folder);
        
        Debug.Log("Screenshot folder path - " + _path);
        
        if (!Directory.Exists(_path)) {
            Directory.CreateDirectory(_path);
        } else {//clean all files
            DirectoryInfo _info = new DirectoryInfo(_path);
            FileInfo[] _files = _info.GetFiles("*.*");
            foreach (FileInfo _file in _files) {
                _file.Delete();
            }
        }
    }
    
    private void ResetParameters() {
        _keepGoing = false;
    }
    
    /// <summary>
    /// Callback function when convertering finished
    /// </summary>
	public void ConvertEnd(string videoPath)
    {
		Debug.Log("Save to camera roll finished and temporary video path is " + videoPath);
		_videoConverter.DisplayMessage("The video has been saved to CameraRoll");
    }
    
    void RotateCube() {
        transform.Rotate (vrotate * Time.deltaTime * 100, Space.World);
    }
    
	void StopRotate() {
		transform.Rotate(0.0f, 0.0f, 0.0f);
	}
	
	void RotateLeft ()
	{
		vrotate = Vector3.up;
	}
	
	void RotateRight ()
	{
		vrotate = Vector3.down;
	}

	void RotateUp ()
	{
		vrotate = Vector3.right;
	}

	void RotateDown ()
	{
		vrotate = Vector3.left;
	}
}
