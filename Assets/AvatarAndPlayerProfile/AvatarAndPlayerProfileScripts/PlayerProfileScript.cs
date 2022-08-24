using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


using DG.Tweening;

public class PlayerProfileScript : MonoBehaviour
{
    bool ovalSelection = true;
    bool autoZoom = false;

    float minAspectRatio = 1f, maxAspectRatio = 1f;

    public string playerId; 

    public GameObject externalPlayerImage;
    public GameObject externalDisplayName;

    public float scale;

    string avatarType = "avatarType";

    string avatarTypeGallery = "GALLERY";
    string avatarTypeCartoon  = "CARTOON";

    string avatarImageGalleryPath = "";
    string avatarImageCartoonPath = "";


     string filePath = "playerImage.png";

     public int numOfAvatar;

    string getKey(string playerId, string keyType) {
        return playerId + "_" + keyType;
    }


    Texture2D loadTextureFromFile(string path) {
        Texture2D load_s01_texture;
        byte[] bytes;
        bytes = System.IO.File.ReadAllBytes (path);
        load_s01_texture = new Texture2D(1,1);
        load_s01_texture.LoadImage(bytes);

        return load_s01_texture; 

    }

   public void setImageForPlayerId(string playerId) {  

       if(!PlayerPrefs.HasKey(getKey(playerId, avatarType))) {
           Debug.Log("Nothing in memory. Not changing image for " + playerId);
       }
       string avatarImageType = PlayerPrefs.GetString(getKey(playerId, avatarType), avatarTypeCartoon);
       Debug.Log("AvatarType is " + avatarImageType);

       try {
        if(avatarImageType.Equals(avatarTypeCartoon)) {
               Debug.Log(" IS CARTOON AvatarType is " + avatarImageType);
            
                string path = PlayerPrefs.GetString(getKey(playerId, avatarImageCartoonPath));
                //We got the name of the object         
                externalPlayerImage.GetComponent<RawImage>().texture = GameObject.Find(path).GetComponent<AvatarCartoonScript>().GetComponent<RawImage>().texture;

        } else {
               Debug.Log("IS GALLERY AvatarType is " + avatarImageType);

                string path = PlayerPrefs.GetString(getKey(playerId, avatarImageGalleryPath));
                Texture2D loaded = loadTextureFromFile(path);
                externalPlayerImage.GetComponent<RawImage>().texture = loaded;


        }

       }  catch(Exception e) {

       }



   }  


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void setAvatar(GameObject avatarNew) {

        //GameObject.Find("avatarParent/seatProfile/faceProfile").GetComponent<RawImage>().texture = avatarNew.transform.Find("face").gameObject.GetComponent<RawImage>().texture;
        GameObject.Find("avatarParent/selectedPlayerImage").GetComponent<RawImage>().texture = avatarNew.GetComponent<RawImage>().texture;
        externalPlayerImage.GetComponent<RawImage>().texture = avatarNew.GetComponent<RawImage>().texture;
        scaleUpAndDown(GameObject.Find("avatarParent/selectedPlayerImage"));

        PlayerPrefs.SetString(getKey(playerId, avatarType), avatarTypeCartoon);
        PlayerPrefs.SetString(getKey(playerId, avatarImageCartoonPath), avatarNew.name);
        PlayerPrefs.Save();

    }
  

    void getPlayerProfile(string playerId) {


        //Get the name saved from PlayerPref

    }

    void Update()
    {
        
    }

    string getImagePath() {
        return Application.persistentDataPath + "/" + playerId + filePath;

    }

    public void close() {
        GameObject.Find("avatarParent").transform.DOScale(0f, 0.3f);
        Vector3 finalPos =  externalPlayerImage.transform.position;
        GameObject.Find("avatarParent").transform.DOMove( finalPos, .3f);
    }


    public void open() {


        GameObject.Find("avatarParent/selectedPlayerImage").GetComponent<RawImage>().texture = externalPlayerImage.GetComponent<RawImage>().texture;
        GameObject.Find("avatarParent").transform.localScale = new Vector3(0f, 0f, 1f);
        GameObject.Find("avatarParent").transform.position = externalPlayerImage.transform.position;
        GameObject.Find("avatarParent").transform.DOMove( new Vector2(Screen.width/2, Screen.height/2), .3f);        
        GameObject.Find("avatarParent").transform.DOScale( scale, .3f);        
    }


    Texture2D duplicateTexture(Texture2D source)
    {

        Debug.Log("duplicateTexture");
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

private void  scaleUpAndDown(GameObject obj) {
    Sequence mySequence = DOTween.Sequence();
    mySequence.Append(obj.transform.DOScale(1.15f, .3f));
    mySequence.Append(obj.transform.DOScale(1f, .3f));
    mySequence.Play();
}

  public void PickImage(int maxSize )    
  {

    RawImage croppedImageHolder = GameObject.Find("selectedPlayerImage").GetComponent<RawImage>();
	NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
	{
		Debug.Log( "Image path: " + path );
		if( path != null )
		{
			// Create Texture from selected image
			Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
			if( texture == null )
			{
				Debug.Log( "Couldn't load texture from " + path );
				return;
			}


                //GameObject picObj = GameObject.Find("seat/pic");
                //GameObject.Find("seat/pic").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));



            ImageCropper.Instance.Show( texture, ( bool result, Texture originalImage, Texture2D croppedImage ) =>
			{
				// Destroy previously cropped texture (if any) to free memory
				//Destroy( croppedImageHolder.texture, 5f );

				// If screenshot was cropped successfully
				if( result )
				{
                    
                    //GameObject.Find("seatProfile/faceProfile").GetComponent<RawImage>().enabled = false;
					// Assign cropped texture to the RawImage
					croppedImageHolder.enabled = true;
					croppedImageHolder.texture = croppedImage;
                    
                    externalPlayerImage.GetComponent<RawImage>().texture = croppedImage;

                    Texture2D duplicate = duplicateTexture(croppedImage);

                    System.IO.File.WriteAllBytes (getImagePath(), duplicate.EncodeToPNG());

                    PlayerPrefs.SetString(getKey(playerId, avatarType), avatarTypeGallery);
                    PlayerPrefs.SetString(getKey(playerId, avatarImageGalleryPath), getImagePath());
                    PlayerPrefs.Save();

					Vector2 size = croppedImageHolder.rectTransform.sizeDelta;
					if( croppedImage.height <= croppedImage.width )
						size = new Vector2( 400f, 400f * ( croppedImage.height / (float) croppedImage.width ) );
					else
						size = new Vector2( 400f * ( croppedImage.width / (float) croppedImage.height ), 400f );
                

                    scaleUpAndDown(GameObject.Find("selectedPlayerImage"));
				}
				else
				{
					croppedImageHolder.enabled = false;
					//croppedImageSize.enabled = false;
				}

				// Destroy the screenshot as we no longer need it in this case
				//Destroy( screenshot );
			},
			settings: new ImageCropper.Settings()
			{
				ovalSelection = ovalSelection,
				autoZoomEnabled = autoZoom,
				imageBackground = Color.clear, // transparent background
				selectionMinAspectRatio = minAspectRatio,
				selectionMaxAspectRatio = maxAspectRatio

			},
			croppedImageResizePolicy: ( ref int width, ref int height ) =>
			{
				// uncomment lines below to save cropped image at half resolution
				//width /= 2;
				//height /= 2;
			} );
  

		}
	} );

	Debug.Log( "Permission result: " + permission );
}
}
