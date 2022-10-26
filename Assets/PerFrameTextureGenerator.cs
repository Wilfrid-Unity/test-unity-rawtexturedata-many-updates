using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PerFrameTextureGenerator : MonoBehaviour
{
    public int PixelColorOffset = 0;

    NativeArray<Color32> AllPixelColors;
    
    //const byte MaxTextureCoord = 255; // too big? Start() does not end with that value
    const byte MaxTextureCoord = 127;

    Texture2D Texture;

    void Start()
    {
        // Generate data that will be copied to textures
        Color32[] pixelColors = new Color32[ (MaxTextureCoord+1) * (MaxTextureCoord+1) * 2];
        for (byte k = 0; k <= 1; k++)
        {
            for (byte i = 0; i <= MaxTextureCoord; i++)
            {
                for (byte j = 0; j <= MaxTextureCoord; j++)
                {
                    int pixelColorIndex = k * (MaxTextureCoord+1) * (MaxTextureCoord+1) + i * (MaxTextureCoord+1) + j;
                    pixelColors[pixelColorIndex].r = i;
                    pixelColors[pixelColorIndex].g = j;
                    pixelColors[pixelColorIndex].b = 0;
                    pixelColors[pixelColorIndex].a = 255;
                }
            }
        }
        AllPixelColors = new NativeArray<Color32>(pixelColors, Allocator.Persistent);

        // Set up object texture
        Texture = new Texture2D((MaxTextureCoord+1), (MaxTextureCoord+1), TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = Texture;
    }

    void Update()
    {
        var textureData = Texture.GetRawTextureData<Color32>();
        NativeArray<Color32> pixelColors = AllPixelColors.GetSubArray(PixelColorOffset, (MaxTextureCoord+1) * (MaxTextureCoord+1));
        unsafe
        {
            Color32* textureDataPtr = (Color32*)NativeArrayUnsafeUtility.GetUnsafePtr(textureData);
            Color32* pixelColorsPtr = (Color32*)NativeArrayUnsafeUtility.GetUnsafePtr(pixelColors);
            UnsafeUtility.MemCpy(textureDataPtr,pixelColorsPtr,(MaxTextureCoord+1) * (MaxTextureCoord+1) * sizeof(Color32));
        }
        Texture.Apply();
        
        // cycle through the pixel colors
        PixelColorOffset = (PixelColorOffset + 1) % ((MaxTextureCoord+1) * (MaxTextureCoord+1));
    }
}
