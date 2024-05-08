using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Ellyality
{
    [System.Serializable]
    public struct Config
    {
        public string file1;
        public string file2;
		public OverlayManager.Dir mode;
    }

    [AddComponentMenu("Ellyality/UI/Fast Two Image Blend")]
    public class OverlayManager : MonoBehaviour
    {
        public enum Dir
        {
            LeftToRight, RightToLeft, UpToDown, DownToUp
        }

        [Header("Register")]
        [SerializeField] Image F1;
        [SerializeField] Image F2;
        [Header("Setting")]
        [SerializeField] bool LoadFromFile;
        [SerializeField] string ConfigFileName;

        Config config;
        float scrollValue;

        void Start()
        {
            if (!LoadFromFile) return;
            config = JsonUtility.FromJson<Config>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, ConfigFileName)));
            F1.sprite = LoadNewSprite(Path.Combine(Application.streamingAssetsPath, config.file1));
            F2.sprite = LoadNewSprite(Path.Combine(Application.streamingAssetsPath, config.file2));
        }

        void Update()
        {
            Vector2 clickPos = Vector2.zero;
            bool hasClick = false;
            if(Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(Input.touchCount - 1);
                clickPos = t.position;
                hasClick = true;
            }
            else if (Input.GetMouseButton(0))
            {
                clickPos = Input.mousePosition;
                hasClick = true;
            }

            if (hasClick)
            {
                switch (config.mode)
                {
                    case Dir.LeftToRight:
                        scrollValue = (float)clickPos.x / (float)Screen.width;
                        F2.fillOrigin = 0;
                        F2.fillMethod = Image.FillMethod.Horizontal;
                        F2.fillAmount = scrollValue;
                        break;
                    case Dir.RightToLeft:
                        scrollValue = (float)clickPos.x / (float)Screen.width;
                        F2.fillOrigin = 1;
                        F2.fillMethod = Image.FillMethod.Horizontal;
                        F2.fillAmount = 1.0f - scrollValue;
                        break;
                    case Dir.UpToDown:
                        scrollValue = (float)clickPos.y / (float)Screen.height;
                        F2.fillOrigin = 1;
                        F2.fillMethod = Image.FillMethod.Vertical;
                        F2.fillAmount = 1.0f - scrollValue;
                        break;
                    case Dir.DownToUp:
                        scrollValue = (float)clickPos.y / (float)Screen.height;
                        F2.fillOrigin = 0;
                        F2.fillMethod = Image.FillMethod.Vertical;
                        F2.fillAmount = scrollValue;
                        break;
                }
            }
        }

        Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
            return NewSprite;
        }

        Texture2D LoadTexture(string FilePath)
        {

            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
    }
}
