using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capture2 : MonoBehaviour
{
    //nombre de carpeta para almacenar imagenes
    public string folder = "ScreenshotFolder";
    // Start is called before the first frame update
    void Start()
    {
        //create Folder
        System.IO.Directory.CreateDirectory(folder);
    }

    // Update is called once per frame
    void Update()
    {
        //Apend filename to folder name(format is '0005 shot.png')
        string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);

        //Capture the screenshot to the specified file
        if (Input.GetKeyDown("p"))
        {
            ScreenCapture.CaptureScreenshot(name);
        }
        
    }
}
