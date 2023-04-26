using UnityEngine;
using UnityEditor;

public class PhysicTestCreator : EditorWindow
{
    public GameObject gummyBearPrefab;
    public GameObject groundPrefab;
    public GameObject platformPrefab;
    public GameObject movingPlatformPrefab;
    public GameObject springyObstaclePrefab;
    public GameObject switchTriggerPrefab;
    public Camera mainCamera;
    public Sprite groundSprite;
    public Sprite platformSprite;

    [MenuItem("Window/Physic Test Creator")]
    public static void ShowWindow()
    {
        GetWindow<PhysicTestCreator>("Physic Test Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Physic Test Prefabs", EditorStyles.boldLabel);

        gummyBearPrefab = (GameObject)EditorGUILayout.ObjectField("Gummy Bear Prefab", gummyBearPrefab, typeof(GameObject), false);
        groundPrefab = (GameObject)EditorGUILayout.ObjectField("Ground Prefab", groundPrefab, typeof(GameObject), false);
        platformPrefab = (GameObject)EditorGUILayout.ObjectField("Platform Prefab", platformPrefab, typeof(GameObject), false);
        movingPlatformPrefab = (GameObject)EditorGUILayout.ObjectField("Moving Platform Prefab", movingPlatformPrefab, typeof(GameObject), false);
        springyObstaclePrefab = (GameObject)EditorGUILayout.ObjectField("Springy Obstacle Prefab", springyObstaclePrefab, typeof(GameObject), false);
        switchTriggerPrefab = (GameObject)EditorGUILayout.ObjectField("Switch Trigger Prefab", switchTriggerPrefab, typeof(GameObject), false);
        mainCamera = (Camera)EditorGUILayout.ObjectField("Main Camera", mainCamera, typeof(Camera), true);
        groundSprite = (Sprite)EditorGUILayout.ObjectField("Ground Sprite", groundSprite, typeof(Sprite), false);
        platformSprite = (Sprite)EditorGUILayout.ObjectField("Platform Sprite", platformSprite, typeof(Sprite), false);

        if (GUILayout.Button("Create Physic Test Scene"))
        {
            CreatePhysicTestScene();
        }
    }

    private void CreatePhysicTestScene()
    {
        // Create GummyBear character
        GameObject gummyBear = Instantiate(gummyBearPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        gummyBear.name = "GummyBear";
        Undo.RegisterCreatedObjectUndo(gummyBear, "Create GummyBear");

        // Create Ground and Platform objects
        GameObject ground = Instantiate(groundPrefab, new Vector3(0, -2, 0), Quaternion.identity);
        ground.GetComponent<SpriteRenderer>().sprite = groundSprite;
        ground.GetComponent<BoxCollider2D>().size = groundSprite.bounds.size;
        ground.name = "Ground";
        Undo.RegisterCreatedObjectUndo(ground, "Create Ground");

        // Create MovingPlatform object
        GameObject movingPlatform = Instantiate(movingPlatformPrefab, new Vector3(0, 3, 0), Quaternion.identity);
        movingPlatform.GetComponent<SpriteRenderer>().sprite = platformSprite;
        movingPlatform.GetComponent<BoxCollider2D>().size = platformSprite.bounds.size;
        movingPlatform.name = "MovingPlatform";
        Undo.RegisterCreatedObjectUndo(movingPlatform, "Create MovingPlatform");

        // Create SpringyObstacle object
        GameObject springyObstacle = Instantiate(springyObstaclePrefab, new Vector3(2, 0, 0), Quaternion.identity);
        springyObstacle.name = "SpringyObstacle";
        Undo.RegisterCreatedObjectUndo(springyObstacle, "Create SpringyObstacle");

        // Create SwitchTrigger object
        GameObject switchTrigger = Instantiate(switchTriggerPrefab, new Vector3(-2, 0, 0), Quaternion.identity);
        switchTrigger.name = "SwitchTrigger";
        Undo.RegisterCreatedObjectUndo(switchTrigger, "Create SwitchTrigger");

        // Camera Follow
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(gummyBear.transform);
        }
    }
}
