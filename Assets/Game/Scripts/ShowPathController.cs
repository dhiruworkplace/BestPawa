using UnityEngine;

public class ShowPathController : MonoBehaviour
{
    public GameObject pathBox;  // child cube used as the line/path
    public GameObject endBox;   // child cube at the obstacle position

    public float maxDistance = 50f; // max raycast distance forward

    public LayerMask obstacleLayerMask; // assign obstacle layer(s) in inspector

    public Color emptyColor = Color.green;
    public Color filledColor = Color.red;

    [Tooltip("Laser color when shape is fully completed")]
    public Color completedColor = Color.green;

    private bool forceCompleted = false;

    void Start()
    {
        isGreen = false;
    }

    void LateUpdate()
    {
        // if (forceCompleted)
        // {
        //     // ✅ Always green if wall is completed
        //     SetColor(pathBox, completedColor);
        //     SetColor(endBox, completedColor);
        //     return;
        // }

        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, obstacleLayerMask, QueryTriggerInteraction.Collide))
        {
            bool isEmpty = hit.collider.isTrigger;

            // Use the cube’s transform position for stable positioning
            Vector3 obstaclePos = hit.collider.transform.position;

            UpdatePathAndEndBox(obstaclePos, !isEmpty);
            SetActivePath(true);

            isGreen = isEmpty;
        }
        else
        {
            SetActivePath(false);
            isGreen = false;
        }

        if (forceCompleted)
        {
            // ✅ Always green if wall is completed
            SetColor(pathBox, completedColor);
            SetColor(endBox, completedColor);
            return;
        }


    }

    // void UpdatePathAndEndBox(Vector3 endPos, bool isObstacleFilled)
    // {
    //     Vector3 startPos = transform.position;

    //     // Position endBox at the obstacle cube position + offset
    //     endBox.transform.position = endPos + new Vector3(0f, 0f, 0f);

    //     // Position and scale pathBox between startPos and endPos
    //     Vector3 midPoint = (startPos + endPos) / 2f;
    //     pathBox.transform.position = midPoint;

    //     Vector3 dir = (endPos - startPos).normalized;
    //     float distance = Vector3.Distance(startPos, endPos);

    //     Vector3 originalScale = pathBox.transform.localScale;
    //     pathBox.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

    //     // pathBox.transform.rotation = Quaternion.LookRotation(dir);

    //     SetColor(pathBox, isObstacleFilled ? filledColor : emptyColor);
    //     SetColor(endBox, isObstacleFilled ? filledColor : emptyColor);
    // }

    void UpdatePathAndEndBox(Vector3 endPos, bool isObstacleFilled)
    {
        Vector3 startPos = transform.position;

        // ✅ Force X and Y same as player, only stretch along Z
        endBox.transform.position = new Vector3(startPos.x, startPos.y, endPos.z);

        // Midpoint only along Z
        float midZ = (startPos.z + endPos.z) / 2f;
        pathBox.transform.position = new Vector3(startPos.x, startPos.y, midZ);

        // Scale only along Z
        float distance = Mathf.Abs(endPos.z - startPos.z);
        Vector3 originalScale = pathBox.transform.localScale;
        pathBox.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

        // Always point forward on Z (no rotation needed)
        pathBox.transform.rotation = Quaternion.identity;

        SetColor(pathBox, isObstacleFilled ? filledColor : emptyColor);
        SetColor(endBox, isObstacleFilled ? filledColor : emptyColor);
    }



    void SetColor(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    public bool isGreen = false;
    void SetActivePath(bool isActive)
    {
        if (pathBox.activeSelf != isActive) pathBox.SetActive(isActive);
        if (endBox.activeSelf != isActive) endBox.SetActive(isActive);
    }

    public void SetCompletedState(bool completed)
    {

        forceCompleted = completed;

        if (completed)
        {
            SetColor(pathBox, completedColor);
            SetColor(endBox, completedColor);
        }

    }

}
