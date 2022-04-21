using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageProcessing : MonoBehaviour
{
    //create a variable that will hold a public static instance of this script so a coroutine can be run externally
    static public ImageProcessing instance;

    //make sure an instance referenced to the manager it is tied to when it is used (Awakes)
    void Awake()
    {
        instance = this;

        //set the save location for data on the device
        //persitent path can only be set in Start / Awake methods
        destPath = Application.persistentDataPath;
        print(destPath);

        //create an images folder to hold all of the app images
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(destPath, "PizzaImages"));
        //fix destination path to include new image folder loaction
        destPath = System.IO.Path.Combine(destPath, "PizzaImages");
        //Debug.Log("image path =  " + destPath);
    }


    static string source; //the URL of the image
    static string image; //the name of the image
    static string destPath; //the local location to save images
    static string destFile; //the local file path

    static public void FetchMyImage(RawImage imageHolder, string imageName)
    {
        //include the file type for the image
        image = imageName += ".png";
        //set up the path to the image
        //source = "http://eqsoc2184012/demer7/Pizza/Images/" + imageName;
        source = "https://ihelensvaleshs.com/digitalsolutions/Pizza/Images/" + imageName;

        //set the local destination file path
        destFile = System.IO.Path.Combine(destPath, imageName);

        //start a coroutine to go get the image - pass in the image Holder so we can use it later
        instance.StartCoroutine(instance.DownloadImage(imageHolder));
    }

    IEnumerator DownloadImage(RawImage imageHolder)
    {
        //start a web request
        WWW www = new WWW(source);

        //temporarily stop the method from running (yield) until the www request finishes. 
        yield return www;

        Debug.Log(www.error);

        //check for things that can go wrong
        if (www != null && www.isDone && www.error == null)
        {
            ////set the stream as a texture
            //Texture2D imageTexture = new Texture2D(100, 100);
            //imageTexture.LoadImage(www.bytes);
            ////imageTexture.name = imageFile;
            //imageHolder.texture = imageTexture;


            //start a file stream to pip the binary data into a file
            FileStream stream = new FileStream(destFile, FileMode.Create);
            //use the stream to write the data to the destination file
            stream.Write(www.bytes, 0, www.bytes.Length);
            //close the stream
            stream.Close();

            //convert the image file to a texture and apply it
            ApplyImage(imageHolder);

        }
    }

    void ApplyImage(RawImage imageHolder)
    {
        //create a basic texture (will be resized when file is loaded)
        Texture2D imageTexture = new Texture2D(100, 100);
        //read the imagedata into the bytes array (method level variable bytes )
        byte[] bytes = File.ReadAllBytes(destFile);
        //create a texture from the bytes
        imageTexture.LoadImage(bytes);
        //set the textures name property to the file name
        imageTexture.name = image;

        //apply the texture to the holder provided
        imageHolder.texture = imageTexture;
    }
}
