using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

public class screenshot : MonoBehaviour {

    //定义图片保存路径
    private string m_FullShotPath;
    //这个不是Main相机，其他摄像机，默认不激活它
    public Camera CameraTrans;
    //显示图片
    public RawImage image;

    void Start()
    {
        //初始化路径，实际使用应该用Application.persistentDataPath，
        //因为使用dataPath就是Asset文件不能读写操作
        m_FullShotPath = "D:/UnityProject/HandgrabProject/Assets/HandPhysics/Example/FullScreenShot.png";
    }

    void OnGUI()
    {
        if (GUILayout.Button("全屏截图", GUILayout.Height(50)))
        {
            print("全屏截图OK");
            CaptureByUnity(m_FullShotPath);
            AssetDatabase.Refresh();
        }
    }

    private void CaptureByUnity(string mFileName)
    {
        ScreenCapture.CaptureScreenshot(mFileName, 0);
    }


}
