using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelRenderer : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera = null;

    private Texture2D texture;
    private GameObject textureObject;
    private Material textureMaterial;
    private Vector2Int simulationSize;

    private const int PixelDensity = 128;

    private static readonly Color WaterColor = Color.blue;
    private static readonly Color SandColor = Color.yellow;
    
    private void Awake()
    {
        CreateSimulationSize();
        SetCameraProperties();
        CreateTextureObject();
        CreateTexture();
        CreateMaterial();
        AssignMaterial();
    }
    private void Update()
    {
        PollPixels();
        Apply();
    }

    private void Apply()
    {
        texture.Apply();
    }
    private void PollPixels()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                PollPixel(new Vector2Int(x, y));
            }
        }
    }
    private void PollPixel(Vector2Int position)
    {
        Color currentColor = texture.GetPixel(position.x, position.y);

        if (currentColor == Color.clear)
            return;

        if(MovePixel(position, Vector2Int.down))
        {
            return;
        }
        else
        {
            if(Random.Range(0f, 1f) > 0.5f)
            {
                MovePixel(position, Vector2Int.right);
            }
            else
            {
                MovePixel(position, Vector2Int.left);
            }
        }
    }
    private bool MovePixel(Vector2Int position, Vector2Int direction)
    {
        Vector2Int targetPosition = position + direction;

        if (!IsOutOfBounds(targetPosition))
            return false;
        
        Color pixelColor = texture.GetPixel(position.x, position.y);
        Color targetColor = texture.GetPixel(targetPosition.x, targetPosition.y);

        if (pixelColor == WaterColor)
        {
            if(targetColor == Color.clear)
            {
                texture.SetPixel(position.x, position.y, Color.clear);
                texture.SetPixel(targetPosition.x, targetPosition.y, pixelColor);

                return true;
            }
        }
        else if(pixelColor == SandColor)
        {
            if(targetColor == WaterColor)
            {
                SwapPixels(position, targetPosition);
            }
            else if(targetColor == Color.clear)
            {
                texture.SetPixel(position.x, position.y, Color.clear);
                texture.SetPixel(targetPosition.x, targetPosition.y, pixelColor);
            }

            return true;
        }
        else
        {
            throw new System.NotImplementedException("Cannot recognize color " + pixelColor);
        }

        return false;
    }
    private void SwapPixels(Vector2Int a, Vector2Int b)
    {
        Color aColor = texture.GetPixel(a.x, a.y);
        Color bColor = texture.GetPixel(b.x, b.y);

        texture.SetPixel(a.x, a.y, bColor);
        texture.SetPixel(b.x, b.y, aColor);
    }
    private void DestroyPixel(Vector2Int position)
    {
        texture.SetPixel(position.x, position.y, Color.clear);
    }
    private bool IsOutOfBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x > texture.width - 1 || position.y < 0 || position.y > texture.height - 1)
            return false;

        return true;
    }

    private void AssignMaterial()
    {
        textureObject.GetComponent<MeshRenderer>().material = textureMaterial;
    }
    private void CreateSimulationSize()
    {
        simulationSize = new Vector2Int()
        {
            x = (int)(PixelDensity * ((float)Screen.width / Screen.height)),
            y = PixelDensity,
        };
    }
    private void CreateTexture()
    {
        texture = new Texture2D(simulationSize.x, simulationSize.y, TextureFormat.ARGB32, false, true);
        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if(Random.Range(0f, 1f) > 0.8f)
                {
                    if(Random.Range(0f, 1f) > 0.6f)
                    {
                        texture.SetPixel(x, y, SandColor);
                    }
                    else
                    {
                        texture.SetPixel(x, y, WaterColor);                        
                    }                    
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
    }
    private void CreateMaterial()
    {
        Shader shader = Shader.Find("Standard");
        textureMaterial = new Material(shader);

        textureMaterial.mainTexture = texture;
    }
    private void CreateTextureObject()
    {
        textureObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        textureObject.transform.localScale = new Vector3(simulationSize.x, simulationSize.y, 1);
    }
    private void SetCameraProperties()
    {
        targetCamera.orthographic = true;
        targetCamera.orthographicSize = simulationSize.y / 2;
        targetCamera.aspect = simulationSize.x / simulationSize.y;
        targetCamera.ResetAspect();
    }
}
