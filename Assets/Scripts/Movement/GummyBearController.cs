using UnityEngine;
using UnityEngine.InputSystem;

public class GummyBearController : MonoBehaviour
{
    public float moveForce = 50.0f;
    public float stretchForce = 50.0f;

    private Vector2 moveDirection;
    private Vector2 stretchDirection;

    private JellySprite m_JellySprite;

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

    void Start()
    {
        m_JellySprite = GetComponent<JellySprite>();
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

                    refPoint.Body2D.AddForce(new Vector2(stretchForceX, stretchForceY));
                }
            }
        }
    }

}
