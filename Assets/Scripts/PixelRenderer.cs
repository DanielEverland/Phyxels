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
    
    private const int PixelDensity = 8;

    private static readonly Color WaterColor = Color.blue;
    private static readonly Color SandColor = Color.yellow;
            
    private void Awake()
    {        
        CreateSimulationSize();
        SetCameraProperties();
        CreateTextureObject();
        CreateTextureHandler();
        CreateMaterial();
        AssignMaterial();
    }
    private void Update()
    {
        PollPixels();
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
