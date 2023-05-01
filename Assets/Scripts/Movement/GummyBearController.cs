using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GummyBearController : MonoBehaviour
{
    public float moveForce = 50.0f;
    public float stretchForce = 50.0f;

    public float maxStretchDistancePercent = 1.5f;
    private bool isInitialized = false;


    private Vector2 moveDirection;
    private Vector2 stretchDirection;

    private UnityJellySprite m_JellySprite;
    private List<Vector2> initialReferencePointOffsets;


    private GummyBearActions gummyBearActions;

    void Awake()
    {
        gummyBearActions = new GummyBearActions();

        gummyBearActions.GummyBearTest.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        gummyBearActions.GummyBearTest.Move.canceled += ctx => moveDirection = Vector2.zero;

        gummyBearActions.GummyBearTest.StretchHorizontal.performed += ctx => stretchDirection.x = ctx.ReadValue<float>();
        gummyBearActions.GummyBearTest.StretchHorizontal.canceled += ctx => stretchDirection.x = 0;

        gummyBearActions.GummyBearTest.StretchVertical.performed += ctx => stretchDirection.y = ctx.ReadValue<float>();
        gummyBearActions.GummyBearTest.StretchVertical.canceled += ctx => stretchDirection.y = 0;
    }

    //void Start()
    //{
    //    if (GetComponent<UnityJellySprite>() != null)
    //    {
    //        Debug.Log("we can find UnityJellySprite");
    //    }

    //    m_JellySprite = GetComponent<UnityJellySprite>();

    //    initialReferencePointOffsets = new List<Vector2>();

    //    if (m_JellySprite.ReferencePoints == null)
    //    {
    //        Debug.Log("there' no reference point");
    //    }
    //    else
    //    {
    //        foreach (var refPoint in m_JellySprite.ReferencePoints)
    //        {
    //            if (refPoint != m_JellySprite.CentralPoint)
    //            {
    //                Vector2 initialOffset = refPoint.transform.position - m_JellySprite.CentralPoint.transform.position;
    //                initialReferencePointOffsets.Add(initialOffset);
    //            }
    //        }
    //    }
    //}

    void Update()
    {
        if (!isInitialized)
        {
            if (GetComponent<UnityJellySprite>() != null)
            {
                Debug.Log("we can find UnityJellySprite");
            }

            m_JellySprite = GetComponent<UnityJellySprite>();

            initialReferencePointOffsets = new List<Vector2>();

            if (m_JellySprite.ReferencePoints == null)
            {
                Debug.Log("there' no reference point");
            }
            else
            {
                foreach (var refPoint in m_JellySprite.ReferencePoints)
                {
                    if (refPoint != m_JellySprite.CentralPoint)
                    {
                        Vector2 initialOffset = refPoint.transform.position - m_JellySprite.CentralPoint.transform.position;
                        initialReferencePointOffsets.Add(initialOffset);
                    }
                }

                isInitialized = true;
            }
        }
    }


    void OnEnable()
    {
        gummyBearActions.Enable();
    }

    void OnDisable()
    {
        gummyBearActions.Disable();
    }

    void FixedUpdate()
    {
        MoveGummyBear();
        StretchGummyBear();
    }

    private void MoveGummyBear()
    {
        foreach (var refPoint in m_JellySprite.ReferencePoints)
        {
            if (refPoint.Body2D != null)
            {
                refPoint.Body2D.AddForce(moveDirection * moveForce);
            }
        }
    }

    private void StretchGummyBear()
    {
        float totalStretchForce = stretchDirection.x + stretchDirection.y;
        if (Mathf.Abs(totalStretchForce) > Mathf.Epsilon)
        {
            int refPointIndex = 0;
            foreach (var refPoint in m_JellySprite.ReferencePoints)
            {
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
                }
                refPointIndex++;
            }
        }
    }
}
