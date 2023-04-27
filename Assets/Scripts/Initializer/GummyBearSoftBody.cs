using UnityEditor;
using UnityEngine;

public class GummyBearSoftBody : MonoBehaviour
{
    // Add fields for Soft Body physics settings, e.g. stiffness, damping, etc.
    // You can add properties or methods to control and manipulate the Soft Body physics.

    public Sprite gummyBearSprite;

    private void OnGUI()
    {
        gummyBearSprite = (Sprite)EditorGUILayout.ObjectField("Gummy Bear Sprite", gummyBearSprite, typeof(Sprite), false);

        if (GUILayout.Button("Create Gummy Bear Prefab"))
        {
            CreateGummyBearPrefab();
        }
    }

    private void CreateGummyBearPrefab()
    {
        // Create a new empty GameObject for the Gummy Bear
        GameObject gummyBear = new GameObject("GummyBear");

        // Add a Sprite Renderer and assign the Gummy Bear sprite
        SpriteRenderer sr = gummyBear.AddComponent<SpriteRenderer>();
        sr.sprite = gummyBearSprite;

        // Add BoxCollider2D for collisions
        gummyBear.AddComponent<BoxCollider2D>();

        // Add Rigidbody2D for physics
        Rigidbody2D rb = gummyBear.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3.0f;

        // Add Soft Body physics script
        gummyBear.AddComponent<GummyBearSoftBody>();

        // Create a prefab from the Gummy Bear game object
        PrefabUtility.SaveAsPrefabAsset(gummyBear, "Assets/GummyBear.prefab");

        // Destroy the temporary Gummy Bear game object in the scene
        DestroyImmediate(gummyBear);
    }

}
