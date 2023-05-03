using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GummyBearController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The force applied to the gummy bear when moving.")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    public float moveForce = 50.0f;

    [Header("Stretch Settings")]
    [Tooltip("The force applied to the gummy bear when stretching.")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    public float stretchForce = 50.0f;

    [Tooltip("The maximum distance the gummy bear can stretch as a percentage of its original size.")]
    [SerializeField]
    [Range(1.0f, 3.0f)]
    public float maxStretchDistancePercent = 1.5f;

    [Tooltip("The force applied to the gummy bear when returning to its unstretched state as a percentage of the stretch force.")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float unstretchedForcePercentage = 0.25f;

    private bool isInitialized = false;
    private Vector2 moveDirection;
    private Vector2 stretchDirection;

    private UnityJellySprite m_JellySprite;
    private List<Vector2> initialReferencePointOffsets;


    private GummyBearActions gummyBearActions;
    private List<Vector2> initialOffsets;
    private float initialTotalArea;

    private void CalculateInitialOffsetsAndArea()
    {
        initialOffsets = new List<Vector2>();

        Vector2 centralPointPosition = m_JellySprite.CentralPoint.Body2D.position;
        float totalArea = 0f;
        int offsetIndex = 0;

        for (int i = 0; i < m_JellySprite.ReferencePoints.Count; i++)
        {
            JellySprite.ReferencePoint refPoint = m_JellySprite.ReferencePoints[i];
            if (refPoint != m_JellySprite.CentralPoint)
            {
                Vector2 offset = refPoint.Body2D.position - centralPointPosition;
                initialOffsets.Add(offset);
                totalArea += Vector2.SignedAngle(initialOffsets[offsetIndex], initialOffsets[(offsetIndex + 1) % initialOffsets.Count]) * 0.5f;
                offsetIndex++;
            }
        }
        initialTotalArea = Mathf.Abs(totalArea);
    }


    private void StretchGummyBearPosition()
    {
        float stretchFactorX = 1.0f + stretchDirection.x;
        float stretchFactorY = 1.0f + stretchDirection.y;

        float targetArea = initialTotalArea * stretchFactorX * stretchFactorY;
        float scaleFactor = Mathf.Sqrt(targetArea / initialTotalArea);

        int refPointIndex = 0;
        for (int i = 0; i < m_JellySprite.ReferencePoints.Count; i++)
        {
            JellySprite.ReferencePoint refPoint = m_JellySprite.ReferencePoints[i];
            if (refPoint.Body2D != null && refPoint != m_JellySprite.CentralPoint)
            {
                Vector2 initialOffset = initialOffsets[refPointIndex];
                Vector2 stretchedOffset = new Vector2(initialOffset.x * stretchFactorX, initialOffset.y * stretchFactorY);
                Vector2 targetPosition = m_JellySprite.CentralPoint.Body2D.position + (Vector2)(stretchedOffset * scaleFactor);
                refPoint.Body2D.MovePosition(targetPosition);
                refPointIndex++;
            }
        }
    }


    void Awake()
    {
        gummyBearActions = new GummyBearActions();

        gummyBearActions.GummyBearTest.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        gummyBearActions.GummyBearTest.Move.canceled += ctx => moveDirection = Vector2.zero;

        gummyBearActions.GummyBearTest.StretchHorizontal.performed += ctx => stretchDirection.x = ctx.ReadValue<float>();
        gummyBearActions.GummyBearTest.StretchHorizontal.canceled += ctx => stretchDirection.x = 0;

        gummyBearActions.GummyBearTest.StretchVertical.performed += ctx => stretchDirection.y = ctx.ReadValue<float>();
        gummyBearActions.GummyBearTest.StretchVertical.canceled += ctx => stretchDirection.y = 0;
        m_JellySprite = GetComponent<UnityJellySprite>();
    }

    void OnEnable()
    {
        gummyBearActions.Enable();
    }

    void OnDisable()
    {
        gummyBearActions.Disable();
    }

    void InitializeJellySprite()
    {
        if (!isInitialized)
        {
            initialReferencePointOffsets = new List<Vector2>();

            foreach (var refPoint in m_JellySprite.ReferencePoints)
            {
                if (refPoint != m_JellySprite.CentralPoint)
                {
                    Vector2 initialOffset = refPoint.transform.position - m_JellySprite.CentralPoint.transform.position;
                    initialReferencePointOffsets.Add(initialOffset);
                }
            }
            CalculateInitialOffsetsAndArea();
            isInitialized = true;
        }
    }

    void FixedUpdate()
    {
        InitializeJellySprite();
        MoveGummyBear();
        StretchGummyBearPosition();
    }

    private void MoveGummyBear()
    {
        foreach (var refPoint in m_JellySprite.ReferencePoints.Where(r => r.Body2D != null))
        {
            refPoint.Body2D.AddForce(moveDirection * moveForce);
        }
    }

    private void StretchGummyBearPower()
    {
        float totalStretchForce = stretchDirection.x + stretchDirection.y;
        if (Mathf.Abs(totalStretchForce) > Mathf.Epsilon)
        {

            var nonCentralReferencePoints = m_JellySprite.ReferencePoints.Where(r => r.Body2D != null && r != m_JellySprite.CentralPoint).ToList();
            for (int refPointIndex = 0; refPointIndex < nonCentralReferencePoints.Count; refPointIndex++)
            {
                var refPoint = nonCentralReferencePoints[refPointIndex];

                // ... (keep the same logic for applying stretch force and unstretched force)
                if (refPoint.Body2D != null && refPoint != m_JellySprite.CentralPoint)
                {
                    Vector2 stretchVector = (refPoint.Body2D.position - m_JellySprite.CentralPoint.Body2D.position).normalized;
                    float angle = Vector2.SignedAngle(Vector2.right, stretchVector);

                    float horizontalStretchForce = Mathf.Cos(angle * Mathf.Deg2Rad) * stretchForce * stretchDirection.x;
                    float verticalStretchForce = Mathf.Sin(angle * Mathf.Deg2Rad) * stretchForce * stretchDirection.y;

                    float stretchForceX = horizontalStretchForce - verticalStretchForce;
                    float stretchForceY = verticalStretchForce + horizontalStretchForce;

                    // Calculate the current distance from the central point
                    float currentDistance = Vector2.Distance(refPoint.Body2D.position, m_JellySprite.CentralPoint.Body2D.position);

                    // Calculate the initial distance from the central point using the stored initial offsets
                    float initialDistance = initialReferencePointOffsets[refPointIndex].magnitude;

                    // Calculate the maximum allowed stretch distance
                    float maxStretchDistance = initialDistance * maxStretchDistancePercent;

                    // Apply the stretch force if the current distance is less than the maximum allowed stretch distance
                    if (currentDistance < maxStretchDistance)
                    {
                        refPoint.Body2D.AddForce(new Vector2(stretchForceX, stretchForceY));
                    }

                    // Apply a force to unstretched reference points towards the central point
                    if (Mathf.Approximately(stretchForceX, 0) && Mathf.Approximately(stretchForceY, 0))
                    {
                        Vector2 unstretchedForce = (m_JellySprite.CentralPoint.Body2D.position - refPoint.Body2D.position) * unstretchedForcePercentage;
                        refPoint.Body2D.AddForce(unstretchedForce);
                    }
                }
            }

     
        }
    }
}
