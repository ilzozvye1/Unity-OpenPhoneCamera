using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine.UI;

public class TextCam : MonoBehaviour {

    public string deviceName;
    WebCamTexture tex;//接收返回的图片数据
    public Image ShowPhoto;
    bool b_SwitchCam2;


    //拍照期间屏蔽手机旋转------TODO



    public void SwitchCam(Button btn)
    {
        b_SwitchCam2 = !b_SwitchCam2;
        if (b_SwitchCam2)
        {
            // 调用摄像头  
            StartCoroutine(start());
            btn.GetComponentInChildren<Text>().text = "停止";
        }
        else
        {
            //停止捕获镜头  
            tex.Stop();
            StopAllCoroutines();
            btn.GetComponentInChildren<Text>().text = "开始";

        }
    }

    public void Reply()
    {
        tex.Play();
    }

    public void CatchVideo()
    {
        //录像  
        StartCoroutine(SeriousPhotoes());
    }

    bool  b_SwitchCam;
    void OnGUI()
    {
        if (Application.platform== RuntimePlatform.Android)
        {
     
            //if (isOpenGUI) return;
            if (GUI.Button(new Rect(10, 20, 100, 40), "start"))
            {
                b_SwitchCam = !b_SwitchCam;
                if (b_SwitchCam)
                {
                    // 调用摄像头  
                    StartCoroutine(start());
                }
                else
                {
                    //停止捕获镜头  
                    tex.Stop();
                    StopAllCoroutines();
                }
            }

            if (GUI.Button(new Rect(10, 120, 100, 40), "replay"))
            {
                //重新开始  
                tex.Play();
            }

            if (GUI.Button(new Rect(120, 20, 80, 40), "record"))
            {
                //录像  
                StartCoroutine(SeriousPhotoes());
            }


            if (tex != null)
            {
                // 捕获截图大小               —距X左屏距离   |   距Y上屏距离   
                //  GUI.DrawTexture(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 190, 280, 200), tex);
                showCam.texture = tex;

            }

        }

    }

    public bool isOpenGUI = false;
    public RawImage showCam;


    /// <summary>  
    /// 捕获窗口位置  
    /// </summary>  
    public IEnumerator start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            deviceName = devices[0].name;
            tex = new WebCamTexture(deviceName, 300, 300, 12);
            tex.Play();
        }
    }

    /// <summary>  
    /// 获取截图  
    /// </summary>  
    /// <returns>The texture.</returns>  
    public IEnumerator getTexture()
    {
        yield return new WaitForEndOfFrame();
        Texture2D t = new Texture2D(400, 300);
        t.ReadPixels(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 360, 300), 0, 0, false);
        //距X左的距离        距Y屏上的距离  
        // t.ReadPixels(new Rect(220, 180, 200, 180), 0, 0, false);  
        t.Apply();
        byte[] byt = t.EncodeToPNG();
        Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        ShowPhoto.sprite = sprite;
        File.WriteAllBytes(Application.dataPath + "/Photoes/" + Time.time + ".jpg", byt);
        tex.Play();
    }

    /// <summary>  
    /// 连续捕获照片  
    /// </summary>  
    /// <returns>The photoes.</returns>  
    public IEnumerator SeriousPhotoes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D(400, 300, TextureFormat.RGB24, true);
            t.ReadPixels(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 50, 360, 300), 0, 0, false);
            t.Apply();

            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            ShowPhoto.sprite = sprite;
            byte[] byt = t.EncodeToPNG();
            if (Directory.Exists(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png"))
            {
                File.WriteAllBytes(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png", byt);

                hintpath.text = Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png";
            }
            else {
                showCam.GetComponent<Image>().color = Color.red;
            }

            Thread.Sleep(300);
        }
    }
   public  Text hintpath;
    private void Start()
    {
        Debug.Log(Application.dataPath + "/MulPhotoes/" + Time.time.ToString() +"_" + Time.time.ToString()+ ".png");
    }

}
