using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GummyBearSketchController : MonoBehaviour
{
    public float scaleFactor = 1.1f;
    public float rotationFactor = 10.0f;

    private JellySprite m_JellySprite;

    void Start()
    {
        m_JellySprite = GetComponent<JellySprite>();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ScaleSprite(scaleFactor);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ScaleSprite(1 / scaleFactor);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateSprite(rotationFactor);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RotateSprite(-rotationFactor);
        }
    }

    private void ScaleSprite(float scale)
    {
        m_JellySprite.m_SpriteScale *= scale;
        m_JellySprite.RefreshMesh();
    }

    private void RotateSprite(float rotation)
    {
        m_JellySprite.m_SpriteRotation += rotation;
        m_JellySprite.RefreshMesh();
    }
}
