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
    
    private const int PixelDensity = 64;
            
    private void Awake()
    {        
        CreateSimulationSize();
        SetCameraProperties();
        CreateTextureObject();
        CreateTextureHandler();
        CreateMaterial();
        AssignMaterial();

        //for (int x = 0; x < simulationSize.x -1; x++)
        //{
        //    for (int y = 0; y < simulationSize.y-1; y++)
        //    {
        //        if(Random.Range(0f, 1f) > 0.5f)
        //        {
        //            if(Random.Range(0f, 1f) > 0.5f)
        //            {
        //                textureHandler.CreatePixel(x, y, Materials.Water);
        //            }
        //            else
        //            {
        //                textureHandler.CreatePixel(x, y, Materials.Sand);
        //            }
        //        }
        //    }
        //}
    }
    private void Update()
    {
        PollInput();
        PollPixels();
    }
    private void PollInput()
    {
        Vector2Int inputPosition = GetMouseInput();

        if(Input.GetKey(KeyCode.Mouse0))
        {
            if(textureHandler.GetPixel(inputPosition.x, inputPosition.y) == Color.clear)
            {
                textureHandler.CreatePixel(inputPosition.x, inputPosition.y, Materials.Sand);
            }
        }
        else if(Input.GetKey(KeyCode.Mouse1))
        {
            if(textureHandler.GetPixel(inputPosition.x, inputPosition.y) == Color.clear)
            {
                textureHandler.CreatePixel(inputPosition.x, inputPosition.y, Materials.Water);
            }
        }
    }
    private void PollPixels()
    {
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
        Color pixelColor = textureHandler.GetPixel(position.x, position.y);

        if(pixelColor == Materials.Sand)
        {
            PollSand(position);
        }
        else if(pixelColor == Materials.Water)
        {
            PollWater(position);
        }
    }
    private void PollWater(Vector2Int position)
    {
        if(textureHandler.GetPixel(position.x, position.y - 1) == Color.clear && !IsOutOfBounds(position + Vector2Int.down))
        {
            textureHandler.MovePixel(position.x, position.y, position.x, position.y - 1);
        }
        else if (textureHandler.GetPixel(position.x + 1, position.y) == Color.clear && !IsOutOfBounds(position + Vector2Int.right))
        {
            textureHandler.MovePixel(position.x, position.y, position.x + 1, position.y);
        }
        else if(textureHandler.GetPixel(position.x - 1, position.y) == Color.clear && !IsOutOfBounds(position + Vector2Int.left))
        {
            textureHandler.MovePixel(position.x, position.y, position.x - 1, position.y);
        }
    }
    private void PollSand(Vector2Int position)
    {
        Vector2Int targetPosition = position + Vector2Int.down;

        if (IsOutOfBounds(targetPosition))
            return;

        if (textureHandler.GetPixel(targetPosition.x, targetPosition.y) == Color.clear)
        {
            textureHandler.MovePixel(position.x, position.y, targetPosition.x, targetPosition.y);
        }
        else if(textureHandler.GetPixel(targetPosition.x, targetPosition.y) == Materials.Water)
        {
            textureHandler.SwapPixels(position.x, position.y, targetPosition.x, targetPosition.y);
        }
    }
    private bool IsOutOfBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x > simulationSize.x - 1 || position.y < 0 || position.y > simulationSize.y - 1)
            return true;

        return false;
    }
    private Vector2Int GetMouseInput()
    {
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return new Vector2Int()
        {
            x = Mathf.FloorToInt(mouseInWorld.x),
            y = Mathf.FloorToInt(mouseInWorld.y),
        };
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
        textureObject.transform.position = new Vector3((float)simulationSize.x / 2, (float)simulationSize.y / 2);
    }
    private void SetCameraProperties()
    {
        targetCamera.orthographic = true;
        targetCamera.orthographicSize = simulationSize.y / 2;
        targetCamera.aspect = simulationSize.x / simulationSize.y;
        targetCamera.ResetAspect();

        targetCamera.transform.position = new Vector3((float)simulationSize.x / 2, (float)simulationSize.y / 2, -1);
    }
}
