using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelRenderer : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera = null;

    private TextureManager textureHandler;
    private GameObject textureObject;
    private Material textureMaterial;
    private Vector2Int simulationSize;
    
    private const int PixelDensity = 4;

    private static readonly Color WaterColor = Color.blue;
    private static readonly Color SandColor = Color.yellow;

    // debug stuff
    private bool allowUpdate = false;
    public static List<Vector3> debugPositions = new List<Vector3>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < debugPositions.Count; i++)
        {
            Gizmos.DrawWireCube(debugPositions[i] - (Vector3)((Vector2)simulationSize / 2) + (Vector3)Vector2.one / 2, Vector3.one);
        }
    }
    
        
    private IEnumerator Start()
    {
        Debug.Break();

        yield return 0;

        allowUpdate = true;
        
        CreateSimulationSize();
        SetCameraProperties();
        CreateTextureObject();
        CreateTextureHandler();
        CreateInitialPixels();
        CreateMaterial();
        AssignMaterial();
    }
    private void Update()
    {
        if(allowUpdate)
            PollPixels();
    }

    private void PollPixels()
    {
        debugPositions.Clear();


        int x, y;
        
        while (textureHandler.ContainsDirtyPixels)
        {
            textureHandler.GetDirtyPixel(out x, out y);
                                    
            PollPixel(new Vector2Int(x, y));
        }
        
        textureHandler.Apply();
    }
    private void PollPixel(Vector2Int position)
    {
        Color currentColor = textureHandler.GetPixel(position.x, position.y);

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
        
        Color pixelColor = textureHandler.GetPixel(position.x, position.y);
        Color targetColor = textureHandler.GetPixel(targetPosition.x, targetPosition.y);

        if (pixelColor == WaterColor)
        {
            if(targetColor == Color.clear)
            {
                textureHandler.MovePixel(position.x, position.y, targetPosition.x, targetPosition.y);

                return true;
            }
        }
        else if(pixelColor == SandColor)
        {
            if(targetColor == WaterColor)
            {
                textureHandler.SwapPixels(position.x, position.y, targetPosition.x, targetPosition.y);
            }
            else if(targetColor == Color.clear)
            {
                textureHandler.MovePixel(position.x, position.y, targetPosition.x, targetPosition.y);
            }

            return true;
        }
        else
        {
            throw new System.NotImplementedException("Cannot recognize color " + pixelColor);
        }

        return false;
    }
    private bool IsOutOfBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x > simulationSize.x - 1 || position.y < 0 || position.y > simulationSize.y - 1)
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
    private void CreateTextureHandler()
    {
        textureHandler = new TextureManager(simulationSize);
    }
    private void CreateInitialPixels()
    {
        for (int y = 0; y < simulationSize.y; y++)
        {
            for (int x = 0; x < simulationSize.x; x++)
            {
                if (Random.Range(0f, 1f) > 0.8f)
                {
                    if (Random.Range(0f, 1f) > 0.6f)
                    {
                        textureHandler.CreatePixel(x, y, SandColor);
                    }
                    else
                    {
                        textureHandler.CreatePixel(x, y, WaterColor);
                    }
                }
            }
        }

        textureHandler.Apply();
    }
    private void CreateMaterial()
    {
        Shader shader = Shader.Find("Standard");
        textureMaterial = new Material(shader);

        textureHandler.AssignTextureToMaterial(textureMaterial);
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
