using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager
{
    public TextureManager(Vector2Int size)
    {
        simulationSize = size;
        swapChain = new SwapChain(2, size.x * size.y);

        CreateTexture(size);
        
    }

    public bool ContainsDirtyPixels => swapChain.ReadBufferContainsDiryPixels;

    private readonly Vector2Int simulationSize;
    private readonly SwapChain swapChain;
    private Texture2D texture;

    public void AssignTextureToMaterial(Material material)
    {
        material.mainTexture = texture;
    }
    public void Apply()
    {
        swapChain.Swap();

        ApplyTexture();
    }
    public void GetDirtyPixel(out int x, out int y)
    {
        int value = swapChain.Read();

        ResolvePixelPositionIndex(value, out x, out y);
    }
    public void CreatePixel(int x, int y, Color color)
    {
        texture.SetPixel(x, y, color);

        Write(x, y);
    }
    public void SwapPixels(int ax, int ay, int bx, int by)
    {
        Color aColor = texture.GetPixel(ax, ay);
        Color bColor = texture.GetPixel(bx, by);

        texture.SetPixel(ax, ay, bColor);
        texture.SetPixel(bx, by, aColor);

        Write(ax, ay);
        Write(bx, by);
    }
    public void MovePixel(int xStart, int yStart, int xEnd, int yEnd)
    {
        Color color = texture.GetPixel(xStart, yStart);
        texture.SetPixel(xStart, yStart, Color.clear);

        texture.SetPixel(xEnd, yEnd, color);
        Write(xEnd, yEnd);
    }
    public Color GetPixel(int x, int y)
    {
        return texture.GetPixel(x, y);
    }

    private void Write(int x, int y)
    {
        int value = GetPixelPositionIndex(x, y);
        swapChain.Write(value);

        PixelRenderer.debugPositions.Add(new Vector3(x, y));
    }
    private int GetPixelPositionIndex(int x, int y)
    {
        return y * simulationSize.x + x;
    }
    private void ResolvePixelPositionIndex(int index, out int x, out int y)
    {
        x = index % simulationSize.x;
        y = index / simulationSize.x;
    }
    private void CreateTexture(Vector2Int size)
    {
        texture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false, true);
        texture.filterMode = FilterMode.Point;

        Clear();
        ApplyTexture();
    }
    private void Clear()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
    }
    private void ApplyTexture()
    {
        texture.Apply();
    }
}
