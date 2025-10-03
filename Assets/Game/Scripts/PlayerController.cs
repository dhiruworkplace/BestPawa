using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Solo.MOST_IN_ONE;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject boxPrefab;
    public float dragThreshold = 30f;

    private Vector2 dragStartScreenPos;
    private bool isDragging;
    private List<Vector3> dragPath = new List<Vector3>(); // stores local positions of boxes only
    private HashSet<Vector3> placedPositions = new HashSet<Vector3>(); // local positions

    private float laneMoveSpeed = 10f;

    private float dragStartTime;
    public float swipeTimeThreshold = 0.2f;
    private float startZ; // Initial Z position

    private ObstacleWallManager wallManager;
    private ObstacleWallBuilder currentWall;

    public float wallPassOffset = 1.0f;  // adjustable offset distance for passing wall check

    [Header("Speed Settings")]
    public float speedMultiplier = 2f; // Public speed multiplier for break-through

    private bool shapeCompleted = false; // Track when the shape is completed
    private bool inputEnabled = true; // Control input state
    private float normalMoveSpeed;

    private bool pendingBreakthrough = false;
    public float breakthroughTriggerOffset = 0.5f; // distance before wall to trigger

    private bool startGame = false;

    private bool hasGameEnded = false;

    public float breakthroughPush = 10f;
    public float breakthroughTorque = 4f;

    public float wallPushMultiplier = 0.5f;  // relative push for wall cubes


    // [SerializeField] private float laneWidth = 1f;   // distance between lanes (your case: 1 unit)
    private int currentLane = 0; // Starts at x = 0

    private int lastLane;

    private enum GestureType { None, Swipe, Drag }
    private GestureType currentGesture = GestureType.None;

    public Transform ground;
    private int lanes;

    void Start()
    {
        startZ = transform.position.z;
        normalMoveSpeed = moveSpeed; // store starting speed

        // Double speed if Pro Mode
        if (GameManager.IsProMode)
        {
            moveSpeed *= 1.5f;
            normalMoveSpeed = moveSpeed;
        }

        // Find the Wall Manager in the generated level
        if (wallManager == null)
        {
            GameObject wmObj = GameObject.FindWithTag("WallManager"); // Tag the wall manager prefab
            if (wmObj != null)
            {
                wallManager = wmObj.GetComponent<ObstacleWallManager>();
            }
            else
            {
                // Debug.LogError("Wall Manager not found! Make sure it has the 'WallManager' tag.");
            }
        }

        // Get the first wall
        if (wallManager != null)
        {
            currentWall = wallManager.GetCurrentWall();
            // Debug.Log(currentWall.name);
        }
        // Debug.Log(currentWall.name);

        startGame = false;
    }

    public void SetGround(int lanes)
    {
        this.lanes = (lanes - 1);
        //Debug.Log("lll : " + this.lanes);
        for (int i = 0; i < ground.childCount; i++)
        {
            if (i < lanes)
                ground.GetChild(i).gameObject.SetActive(true);
            else
                ground.GetChild(i).gameObject.SetActive(false);
        }
    }

    public int TotalWallToClear()
    {
        if (wallManager != null)
        {
            return wallManager.walls.Count;
        }

        return 0;
    }


    void Update()
    {

        if (!startGame || hasGameEnded)
            return;
        // If input disabled, skip handling movement input
        if (!inputEnabled)
        {
            MoveForward();
            HandleLaneMovement(); // still allow lane interpolation
        }
        else
        {
            MoveForward();

#if UNITY_EDITOR
            HandleMouseInput();
#else
        HandleTouchInput();
#endif

            HandleLaneMovement();
        }
        // Fail condition: shape not completed before wall
        if (currentWall != null && !shapeCompleted)
        {
            float triggerZ = currentWall.transform.position.z - breakthroughTriggerOffset;
            if (transform.position.z >= triggerZ)
            {
                hasGameEnded = true;
                GameOver();
                // Step 1: Make player boxes collapse
                DetachPlayerBoxesAndEnablePhysics();

                // Step 2: Delay before showing the lose popup
                StartCoroutine(ShowLosePopupAfterDelay(0.5f));
            }
        }

        // Trigger breakthrough when player is close enough
        if (pendingBreakthrough && currentWall != null)
        {
            float triggerZ = currentWall.transform.position.z - breakthroughTriggerOffset;
            if (transform.position.z >= triggerZ)
            {
                // Debug.Log("Triggering breakthrough effect!");
                SoundManager.Instance.PlayCoin();
                // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.MediumImpact);
                VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.MediumImpact);
                PrepareBreakthrough();
                pendingBreakthrough = false; // only trigger once
            }
        }

        // Check if player passed the current wall
        if (currentWall != null && transform.position.z > currentWall.transform.position.z + wallPassOffset)
        {
            // Debug.Log("Player passed the current wall");

            // Move to the next wall first
            wallManager.MoveToNextWall();
            currentWall = wallManager.GetCurrentWall();

            if (wallManager.AllWallsCompleted() || currentWall == null)
            {
                // Debug.Log("All walls completed! Disabling input.");
                inputEnabled = false;  // Disable player input because level finished
                moveSpeed = normalMoveSpeed;

                // Start coroutine to boost speed after a delay (e.g., 2 seconds)
                StartCoroutine(BoostSpeedAfterDelay(2f, speedMultiplier));

                // Optionally trigger level complete here, e.g.
                // GameUIManager.Instance.LevelComplete();
            }
            else
            {
                // Still have walls to pass, so keep input enabled and reset movement
                ShowPathController[] allShowPaths = GetComponentsInChildren<ShowPathController>();
                foreach (var path in allShowPaths)
                {
                    path.SetCompletedState(false); // go back to yellow mode
                }
                inputEnabled = true;
                moveSpeed = normalMoveSpeed;
                shapeCompleted = false;



                // Debug.Log("Next wall: " + currentWall.name);
            }
        }

    }

    // Add this coroutine inside your player script
    private IEnumerator BoostSpeedAfterDelay(float delay, float speedMultiplier)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed *= speedMultiplier;
    }


    public void StartGame()
    {
        startGame = true;
    }

    public void GameOver()
    {
        hasGameEnded = true;
    }

    private IEnumerator ShowLosePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameOver();
        GameUIManager.Instance.LevelFailed();
        // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
        VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.Failure);
    }

    bool IsShapeFilled()
    {
        if (currentWall == null) return false;

        var obstacleBuilder = currentWall.GetComponent<ObstacleWallBuilder>();
        if (obstacleBuilder == null) return false;


        return placedPositions.Count + 1 == obstacleBuilder.emptyCubes.Count;
    }

    bool AreColorsEqualWithAlpha(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance &&
               Mathf.Abs(a.a - b.a) < tolerance;
    }

    bool AreAllPathsClear()
    {
        ShowPathController[] allShowPaths = GetComponentsInChildren<ShowPathController>();

        foreach (var showPath in allShowPaths)
        {

            if (!showPath.isGreen)
            {
                // Debug.Log("Green False");
                // Debug.Log(showPath.name);
                return false;
            }
        }

        // Debug.Log("Gree True");
        return true;
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private float lastTapTime = 0f;
    [SerializeField] private float doubleTapThreshold = 0.3f; // Seconds

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Double-click check
            if (Time.time - lastTapTime <= doubleTapThreshold)
            {
                if (placedPositions.Count == 0)
                {
                    // Auto-move player to first empty slot in current wall
                    TryAutoMoveToEmptySlot();
                    // Debug.Log("Change Lane");
                }
                else
                {
                    // Existing behavior for multiple cubes
                    SoundManager.Instance.PlayRemoveCube();
                    DetachPlayerBoxesAndEnablePhysics();
                }
            }
            lastTapTime = Time.time;

            StartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 endPos = Input.mousePosition;
            Vector2 swipeDelta = endPos - dragStartScreenPos;
            float swipeTime = Time.time - dragStartTime;

            isDragging = false;

            if (swipeTime <= swipeTimeThreshold && Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) && swipeDelta.magnitude > dragThreshold)
            {
                if (swipeDelta.x < 0)
                {
                    if (CanChangeLane(-1))
                        MoveLeft();
                    // else
                    //     Debug.Log("❌ Lane change blocked on LEFT");
                }
                else
                {
                    if (CanChangeLane(+1))
                        MoveRight();
                    // else
                    //     Debug.Log("❌ Lane change blocked on RIGHT");
                }
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Double-tap check
                if (Time.time - lastTapTime <= doubleTapThreshold)
                {
                    if (placedPositions.Count == 0)
                    {
                        TryAutoMoveToEmptySlot();
                    }
                    else
                    {
                        SoundManager.Instance.PlayRemoveCube();
                        DetachPlayerBoxesAndEnablePhysics();
                    }
                }
                lastTapTime = Time.time; // ✅ update timestamp

                StartDrag(touch.position); // ✅ begin drag
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                ContinueDrag(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                Vector2 endPos = touch.position;
                Vector2 swipeDelta = endPos - dragStartScreenPos;
                float swipeTime = Time.time - dragStartTime;

                isDragging = false;

                if (swipeTime <= swipeTimeThreshold && Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) && swipeDelta.magnitude > dragThreshold)
                {
                    if (swipeDelta.x < 0)
                    {
                        if (CanChangeLane(-1))
                            MoveLeft();
                        // else
                        //     Debug.Log("❌ Lane change blocked on LEFT");
                    }
                    else
                    {
                        if (CanChangeLane(+1))
                            MoveRight();
                        // else
                        //     Debug.Log("❌ Lane change blocked on RIGHT");
                    }
                }
            }
        }
    }

    // void HandleTouchInput()
    // {
    //     if (Input.touchCount == 0) return;

    //     Touch touch = Input.GetTouch(0);

    //     if (touch.phase == TouchPhase.Began)
    //     {
    //         // ✅ Double-tap check
    //         if (Time.time - lastTapTime <= doubleTapThreshold)
    //         {
    //             if (placedPositions.Count == 0)
    //             {
    //                 TryAutoMoveToEmptySlot();
    //             }
    //             else
    //             {
    //                 SoundManager.Instance.PlayRemoveCube();
    //                 DetachPlayerBoxesAndEnablePhysics();
    //             }
    //         }
    //         lastTapTime = Time.time;

    //         dragStartScreenPos = touch.position;
    //         dragStartTime = Time.time;
    //         currentGesture = GestureType.None;
    //         isDragging = true;
    //     }
    //     else if (touch.phase == TouchPhase.Moved)
    //     {
    //         Vector2 delta = touch.position - dragStartScreenPos;
    //         float elapsed = Time.time - dragStartTime;

    //         // ✅ Decide gesture only once
    //         if (currentGesture == GestureType.None)
    //         {
    //             if (elapsed <= swipeTimeThreshold &&
    //                 Mathf.Abs(delta.x) > Mathf.Abs(delta.y) &&
    //                 delta.magnitude > dragThreshold)
    //             {
    //                 currentGesture = GestureType.Swipe; // lock as swipe
    //                 isDragging = false;
    //             }
    //             else if (delta.magnitude > dragThreshold)
    //             {
    //                 currentGesture = GestureType.Drag; // lock as drag
    //             }
    //         }

    //         // ✅ Only drag adds cubes
    //         if (currentGesture == GestureType.Drag)
    //         {
    //             ContinueDrag(touch.position);
    //         }
    //     }
    //     else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
    //     {
    //         if (currentGesture == GestureType.Swipe)
    //         {
    //             Vector2 endPos = touch.position;
    //             Vector2 swipeDelta = endPos - dragStartScreenPos;

    //             if (swipeDelta.x < 0)
    //             {
    //                 if (CanChangeLane(-1)) MoveLeft();
    //             }
    //             else if (swipeDelta.x > 0)
    //             {
    //                 if (CanChangeLane(+1)) MoveRight();
    //             }
    //         }

    //         isDragging = false;
    //         currentGesture = GestureType.None; // reset
    //     }
    // }

    void StartDrag(Vector2 screenPos)
    {
        dragStartScreenPos = screenPos;
        dragStartTime = Time.time;
        isDragging = true;
    }

    // void ContinueDrag(Vector2 currentScreenPos)
    // {
    //     if (!isDragging) return;

    //     Vector2 delta = currentScreenPos - dragStartScreenPos;

    //     if (delta.magnitude >= dragThreshold)
    //     {
    //         Vector3 direction = Vector3.zero;
    //         if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
    //             direction = Vector3.right * Mathf.Sign(delta.x);
    //         else
    //             direction = Vector3.up * Mathf.Sign(delta.y);

    //         Vector3 currentTop = dragPath.Count > 0 ? dragPath[dragPath.Count - 1] : Vector3.zero;
    //         Vector3 nextLocalPos = SnapToGrid(currentTop + direction);

    //         Debug.Log(nextLocalPos.x);

    //         // Restrict position in LOCAL space
    //         if (nextLocalPos.y < 0f || nextLocalPos.y > 10f) return;
    //         if (nextLocalPos.x < -3f || nextLocalPos.x > 0f) return;

    //         // // Restrict relative to current lane (prevents extra cubes on right side)
    //         // if (nextLocalPos.x > currentLane) return;


    //         // Prevent double step in same direction
    //         if (dragPath.Count > 0 && nextLocalPos == dragPath[dragPath.Count - 1]) return;

    //         // Back to base
    //         if (dragPath.Count == 1 && nextLocalPos == Vector3.zero)
    //         {
    //             RemoveLastBox();
    //         }
    //         else if (dragPath.Count >= 2 && nextLocalPos == dragPath[dragPath.Count - 2])
    //         {
    //             RemoveLastBox();
    //         }
    //         else if (!placedPositions.Contains(nextLocalPos) && nextLocalPos != Vector3.zero)
    //         {
    //             AddBox(nextLocalPos);
    //         }

    //         dragStartScreenPos = currentScreenPos;
    //     }
    // }

    void ContinueDrag(Vector2 currentScreenPos)
    {
        if (!isDragging) return;

        Vector2 delta = currentScreenPos - dragStartScreenPos;

        if (delta.magnitude >= dragThreshold)
        {
            Vector3 direction = Vector3.zero;

            // Decide drag direction
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                direction = Vector3.right * Mathf.Sign(delta.x);
            else
                direction = Vector3.up * Mathf.Sign(delta.y);

            // Get current top position in local drag path
            Vector3 currentTop = dragPath.Count > 0 ? dragPath[dragPath.Count - 1] : Vector3.zero;

            // Apply lane offset → snap → shift back to local
            Vector3 nextWorldPos = new Vector3(currentLane, 0, 0) + currentTop + direction;
            Vector3 nextLocalPos = SnapToGrid(nextWorldPos);
            nextLocalPos.x -= currentLane; // shift back relative to player

            // Restrict vertical placement
            if (nextLocalPos.y < 0f || nextLocalPos.y > 10f) return;

            // Prevent double step in same direction
            if (dragPath.Count > 0 && nextLocalPos == dragPath[dragPath.Count - 1]) return;

            // Handle backtracking
            if (dragPath.Count == 1 && nextLocalPos == Vector3.zero)
            {
                RemoveLastBox();
            }
            else if (dragPath.Count >= 2 && nextLocalPos == dragPath[dragPath.Count - 2])
            {
                RemoveLastBox();
            }
            else if (!placedPositions.Contains(nextLocalPos) && nextLocalPos != Vector3.zero)
            {
                AddBox(nextLocalPos); // ✅ already snapped
            }

            dragStartScreenPos = currentScreenPos;
        }
    }

    // Vector3 SnapToGrid(Vector3 pos)
    // {
    //     return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0f);
    // }

    Vector3 SnapToGrid(Vector3 pos)
    {
        float snappedX = Mathf.Round(pos.x);
        float snappedY = Mathf.Round(pos.y);

        // 🔹 Always restrict horizontal placement into range [-3, 0]
        snappedX = Mathf.Clamp(snappedX, -lanes, 0f);

        return new Vector3(snappedX, snappedY, 0f);
    }


    void EndDrag()
    {
        isDragging = false;
    }

    // void AddBox(Vector3 localPos)
    // {
    //     GameObject newBox = Instantiate(boxPrefab, transform);
    //     newBox.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);

    //     placedPositions.Add(localPos);
    //     dragPath.Add(localPos);

    //     SoundManager.Instance.PlayAddCube();
    //     // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.SoftImpact);
    //     VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.SoftImpact);

    //     // Debug.Log(IsShapeFilled());
    //     // Debug.Log(AreAllPathsClear());

    //     // Call check with a short delay
    //     StartCoroutine(CheckCompletionDelayed());
    //     // CheckCompletionDelayed();
    // }

    // void RemoveLastBox()
    // {
    //     if (dragPath.Count == 0) return;

    //     Vector3 lastLocalPos = dragPath[dragPath.Count - 1];
    //     dragPath.RemoveAt(dragPath.Count - 1);
    //     placedPositions.Remove(lastLocalPos);
    //     // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.SoftImpact);
    //     VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.SoftImpact);

    //     foreach (Transform child in transform)
    //     {
    //         if (SnapToGrid(child.localPosition) == lastLocalPos)
    //         {
    //             Destroy(child.gameObject);
    //             break;
    //         }
    //     }
    //     SoundManager.Instance.PlayRemoveCube();

    //     StartCoroutine(CheckCompletionDelayed());
    //     // CheckCompletionDelayed();
    // }

    void AddBox(Vector3 localPos)
    {
        GameObject newBox = Instantiate(boxPrefab, transform);
        Vector3 fixedLocal = new(localPos.x, localPos.y, 0f);
        newBox.transform.localPosition = fixedLocal;

        placedPositions.Add(localPos);
        dragPath.Add(localPos);

        SoundManager.Instance.PlayAddCube();
        // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.SoftImpact);
        VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.SoftImpact);

        // Debug.Log(IsShapeFilled());
        // Debug.Log(AreAllPathsClear());

        // Call check with a short delay
        StartCoroutine(CheckCompletionDelayed());
        // CheckCompletionDelayed();
    }

    IEnumerator CheckCompletionDelayed()
    {
        // yield return new WaitForSeconds(0.1f); // small delay to let things update

        yield return null;

        if (IsShapeFilled() && AreAllPathsClear() && !shapeCompleted)
        {
            shapeCompleted = true;
            inputEnabled = false; // disable player input
            moveSpeed *= speedMultiplier; // boost forward movement speed

            // Debug.Log("Shape complete! Break through wall!");

            // Set flag to trigger breakthrough when close to wall
            pendingBreakthrough = true;

            // 🔹 Turn all lasers green
            ShowPathController[] allShowPaths = GetComponentsInChildren<ShowPathController>();
            foreach (var path in allShowPaths)
            {
                path.SetCompletedState(true);
            }
        }
    }


    // void RemoveLastBox()
    // {
    //     if (dragPath.Count == 0) return;

    //     // Get last placed cube position (snapped)
    //     Vector3 lastLocalPos = dragPath[dragPath.Count - 1];
    //     Vector3 snappedLast = SnapToGrid(lastLocalPos);

    //     // Remove from path + set
    //     dragPath.RemoveAt(dragPath.Count - 1);
    //     placedPositions.Remove(lastLocalPos);

    //     // Haptic feedback
    //     VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.SoftImpact);

    //     // Find matching child cube safely (integer compare)
    //     foreach (Transform child in transform)
    //     {
    //         Vector3 snappedChild = SnapToGrid(child.localPosition);

    //         if (Mathf.RoundToInt(snappedChild.x) == Mathf.RoundToInt(snappedLast.x) &&
    //             Mathf.RoundToInt(snappedChild.y) == Mathf.RoundToInt(snappedLast.y))
    //         {
    //             Destroy(child.gameObject);
    //             break;
    //         }
    //     }

    //     SoundManager.Instance.PlayRemoveCube();
    //     StartCoroutine(CheckCompletionDelayed());
    // }


    void RemoveLastBox()
    {
        if (dragPath.Count == 0) return;

        // Get last placed local position
        Vector3 lastLocalPos = dragPath[dragPath.Count - 1];
        dragPath.RemoveAt(dragPath.Count - 1);
        placedPositions.Remove(lastLocalPos);

        VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.SoftImpact);

        // Convert stored local position into world space for comparison
        Vector3 worldTarget = transform.TransformPoint(lastLocalPos);
        Vector3 snappedWorldTarget = SnapToGrid(worldTarget);

        foreach (Transform child in transform)
        {
            Vector3 snappedChild = SnapToGrid(child.position); // child.position = world space

            if (Mathf.RoundToInt(snappedChild.x) == Mathf.RoundToInt(snappedWorldTarget.x) &&
                Mathf.RoundToInt(snappedChild.y) == Mathf.RoundToInt(snappedWorldTarget.y))
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
                break;
            }
        }

        SoundManager.Instance.PlayRemoveCube();
        StartCoroutine(CheckCompletionDelayed());
    }
    void HandleLaneMovement()
    {
        Vector3 pos = transform.position;
        float targetX = currentLane;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * laneMoveSpeed);
        transform.position = pos;

        // Run check once when lane actually changes
        if (lastLane != currentLane)
        {
            lastLane = currentLane;
            StartCoroutine(CheckCompletionDelayed());
        }
    }

    bool CanChangeLane(int direction)  // -1 left, +1 right
    {
        int targetLane = currentLane + direction;

        // Clamp allowed lanes between -3 and 0
        if (targetLane < -lanes || targetLane > 0)
            return false;

        // Check player root itself
        if (targetLane < -lanes || targetLane > 0)
            return false;

        // Check all attached cubes
        foreach (var pos in placedPositions)
        {
            // Convert cube’s local position into "future world lane" by adding future lane
            float shiftedX = pos.x + targetLane;

            // If any cube would fall outside bounds → block
            if (shiftedX < -lanes || shiftedX > 0)
            {
                Debug.Log($"❌ Blocked: cube {pos} would end up at lane {shiftedX}");
                return false;
            }
        }

        return true; // ✅ player + all cubes fit inside [-3,0]
    }


    void MoveLeft()
    {
        if (CanChangeLane(-1))
        {
            currentLane--;
        }
    }

    void MoveRight()
    {
        if (CanChangeLane(+1))
        {
            currentLane++;
        }
    }

    private void PrepareBreakthrough()
    {
        EnablePhysicsAndPushWallCubes();

        DetachPlayerBoxesAndEnablePhysics();
    }

    private void EnablePhysicsAndPushWallCubes()
    {
        // ENABLE PHYSICS & ADD FORCES TO WALL CUBES
        if (currentWall == null) return;

        Rigidbody[] wallBodies = currentWall.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in wallBodies)
        {
            if (rb == null) continue;

            // Disable stars
            if (rb.transform.CompareTag("Star"))
            {
                rb.gameObject.SetActive(false);
                continue; // Skip force application to stars
            }

            // Make collider solid
            Collider c = rb.GetComponent<Collider>();
            if (c != null)
                c.isTrigger = false;

            // Enable dynamics
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.WakeUp();

            // Give initial push and random spin
            rb.AddForce(
                transform.forward * (breakthroughPush * wallPushMultiplier) +
                Vector3.up * (breakthroughPush * 0.1f),
                ForceMode.VelocityChange
            );

            rb.AddTorque(
                Random.insideUnitSphere * (breakthroughTorque * wallPushMultiplier),
                ForceMode.VelocityChange
            );
        }
    }

    private void DetachPlayerBoxesAndEnablePhysics()
    {
        // DETACH PLAYER-ADDED BOXES AND ENABLE THEIR PHYSICS
        Rigidbody[] playerBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in playerBodies)
        {
            if (rb == null) continue;
            // Skip if the Rigidbody is on the player root object
            if (rb.gameObject == gameObject) continue;

            // Disable path controller if present
            ShowPathController pathController = rb.GetComponent<ShowPathController>();
            if (pathController != null)
                pathController.enabled = false;

            // Hide all child mesh renderers
            foreach (Transform child in rb.transform)
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                if (mr != null)
                    mr.enabled = false;
            }

            // Detach from player so physics will move them independently
            rb.transform.SetParent(null);

            // Make collider solid
            Collider c = rb.GetComponent<Collider>();
            if (c != null)
                c.isTrigger = false;

            // Enable dynamics
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.WakeUp();

            // Optional: tweak mass/drag
            rb.mass = Mathf.Max(0.1f, rb.mass);
            rb.linearDamping = Mathf.Clamp(rb.linearDamping, 0f, 2f);

            // Give initial push + torque
            rb.AddForce(transform.forward * breakthroughPush + Vector3.up * (breakthroughPush * 0.15f),
                        ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * breakthroughTorque, ForceMode.VelocityChange);
        }

        // Clear placement tracking so cubes can’t be reused
        dragPath.Clear();
        placedPositions.Clear();
    }

    void TryAutoMoveToEmptySlot()
    {
        if (currentWall == null || currentWall.emptyCubes.Count == 0)
        {
            Debug.Log("⚠️ No empty cubes available in current wall");
            return;
        }

        // Pick first empty cube (or later: pick nearest)
        GameObject targetEmpty = currentWall.emptyCubes[0];

        Vector3 targetLocal = targetEmpty.transform.localPosition;

        // Convert cube.x to lane index
        // Your wall places cubes at 0, -1, -2, -3
        int targetLane = Mathf.RoundToInt(targetLocal.x);

        // Debug.Log($"✅ Auto-moving to empty slot at localX={targetLocal.x}, lane={targetLane}");

        // Just update the lane → HandleLaneMovement() will move player smoothly
        currentLane = targetLane;
    }


}
