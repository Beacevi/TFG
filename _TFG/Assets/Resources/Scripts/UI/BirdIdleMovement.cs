using UnityEngine;

public class BirdIdleMovement : MonoBehaviour
{
    [Header("Free Roam Settings")]
    public Rect roamBounds = new Rect(-5f, -3f, 10f, 6f);
    public float roamSpeed = 2f;
    public float directionChangeInterval = 2f;
    public float boundaryAvoidDistance = 0.8f;
    public float smoothTurnSpeed = 3f;

    private Vector2 roamVelocity;
    private Vector2 roamTargetDirection;
    private float directionChangeTimer;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        roamTargetDirection = Random.insideUnitCircle.normalized;
        roamVelocity = roamTargetDirection * roamSpeed;
        directionChangeTimer = Random.Range(0f, directionChangeInterval);
    }

    private void Update()
    {
        // 1. Count down and pick a new wander direction periodically
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            Vector2 forward = roamVelocity.normalized;
            Vector2 randomOffset = Random.insideUnitCircle;
            roamTargetDirection = (forward + randomOffset).normalized;
            directionChangeTimer = directionChangeInterval + Random.Range(-0.5f, 0.5f);
        }

        // 2. Boundary avoidance — steer away from edges when close
        Vector2 pos2D = transform.position;
        Vector2 avoidance = Vector2.zero;

        float leftDist = pos2D.x - roamBounds.xMin;
        float rightDist = roamBounds.xMax - pos2D.x;
        float bottomDist = pos2D.y - roamBounds.yMin;
        float topDist = roamBounds.yMax - pos2D.y;

        if (leftDist < boundaryAvoidDistance) avoidance.x += 1f - (leftDist / boundaryAvoidDistance);
        if (rightDist < boundaryAvoidDistance) avoidance.x -= 1f - (rightDist / boundaryAvoidDistance);
        if (bottomDist < boundaryAvoidDistance) avoidance.y += 1f - (bottomDist / boundaryAvoidDistance);
        if (topDist < boundaryAvoidDistance) avoidance.y -= 1f - (topDist / boundaryAvoidDistance);

        Vector2 desiredDirection = (roamTargetDirection + avoidance * 2f).normalized;

        // 3. Smoothly steer current velocity toward desired direction
        roamVelocity = Vector2.Lerp(
            roamVelocity.normalized,
            desiredDirection,
            Time.deltaTime * smoothTurnSpeed
        ).normalized * roamSpeed;

        // 4. Move and hard-clamp inside bounds as a safety net
        Vector2 newPos = pos2D + roamVelocity * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, roamBounds.xMin, roamBounds.xMax);
        newPos.y = Mathf.Clamp(newPos.y, roamBounds.yMin, roamBounds.yMax);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        // 5. Flip sprite to match horizontal travel direction
        if (spriteRenderer != null)
        {
            if (roamVelocity.x > 0.01f)
                spriteRenderer.flipX = true;   // was false
            else if (roamVelocity.x < -0.01f)
                spriteRenderer.flipX = false;  // was true
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3(roamBounds.center.x, roamBounds.center.y, 0),
            new Vector3(roamBounds.width, roamBounds.height, 0)
        );
    }
#endif
}