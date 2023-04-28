using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellySpriteFollowMouse : MonoBehaviour
{
    public float m_MaxDistanceOffset = 2.0f;
    
    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        JellySprite jellySprite = GetComponent<JellySprite>();

        // This uses Physics system's MovePosition to move the jelly sprite using physics forces
        if(jellySprite)
        {
            if(jellySprite.CentralPoint.Body2D)
            {                
                jellySprite.CentralPoint.Body2D.MovePosition(mousePos);
            }        
            else if (jellySprite.CentralPoint.Body3D)
            {
                jellySprite.CentralPoint.Body3D.MovePosition(mousePos);
            }

            foreach(JellySprite.ReferencePoint refPoint in jellySprite.ReferencePoints)
            {
                if(refPoint != jellySprite.CentralPoint)
                {
                    Vector2 offset = refPoint.transform.position - jellySprite.CentralPoint.transform.position;
                    float distance = offset.magnitude;
                    float initialDistance = refPoint.InitialOffset.magnitude;

                    // This prevents the physics body from getting too far away from the central point (eg. if it gets stuck behind an object)
                    if(distance > initialDistance * m_MaxDistanceOffset)
                    {                        
                        refPoint.transform.position = (jellySprite.CentralPoint.transform.position) + (Vector3)(offset.normalized * initialDistance);
                    }
                }
            }
        }
    }
}
