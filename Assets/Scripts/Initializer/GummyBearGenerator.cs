using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GummyBearGenerator : MonoBehaviour
{
    [Header("Soft Body Parameters")]
    public int segments = 8;
    public float jointRadius = 0.25f;
    public float circleColliderRadius = 0.1f;
    public float dampingRatio = 0.5f;
    public float frequency = 2.0f;
    public Sprite circleSprite; // 新增：用于可视化的 Sprite

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Create Gummy Bear Soft Body
        for (int i = 0; i < segments; i++)
        {
            float angle = (2 * Mathf.PI / segments) * i;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * jointRadius;

            // Create child GameObject with CircleCollider2D and Rigidbody2D
            GameObject circle = new GameObject("Circle " + i);
            circle.transform.parent = transform;
            circle.transform.localPosition = offset;

            CircleCollider2D circleCollider = circle.AddComponent<CircleCollider2D>();
            circleCollider.radius = circleColliderRadius;

            Rigidbody2D rigidBody = circle.AddComponent<Rigidbody2D>();
            rigidBody.mass = 1.0f / segments;

            // 新增：为子对象添加 SpriteRenderer 以实现可视化
            SpriteRenderer circleSpriteRenderer = circle.AddComponent<SpriteRenderer>();
            circleSpriteRenderer.sprite = circleSprite;
            circleSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

            // Create HingeJoint2D and connect it to the previous child GameObject
            if (i > 0)
            {
                HingeJoint2D hingeJoint = circle.AddComponent<HingeJoint2D>();
                hingeJoint.connectedBody = transform.GetChild(i - 1).GetComponent<Rigidbody2D>();
                hingeJoint.autoConfigureConnectedAnchor = false;
                hingeJoint.anchor = Vector2.zero;
                hingeJoint.connectedAnchor = Vector2.zero;

                hingeJoint.useLimits = true;
                JointAngleLimits2D limits = new JointAngleLimits2D
                {
                    min = -20,
                    max = 20
                };
                hingeJoint.limits = limits;
            }
        }

        // Connect the last child GameObject to the first one
        HingeJoint2D lastHingeJoint = transform.GetChild(segments - 1).gameObject.AddComponent<HingeJoint2D>();
        lastHingeJoint.connectedBody = transform.GetChild(0).GetComponent<Rigidbody2D>();
        lastHingeJoint.autoConfigureConnectedAnchor = false;
        lastHingeJoint.anchor = Vector2.zero;
        lastHingeJoint.connectedAnchor = Vector2.zero;

        lastHingeJoint.useLimits = true;
        JointAngleLimits2D lastLimits = new JointAngleLimits2D
        {
            min = -20,
            max = 20
        };
        lastHingeJoint.limits = lastLimits;
    }
}
