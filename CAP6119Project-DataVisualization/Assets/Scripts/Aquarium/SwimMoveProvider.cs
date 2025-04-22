using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class SwimMoveProvider : ConstrainedMoveProvider
{
    [Header("Values")]
    [SerializeField] float swimForce = 4f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] Vector3 baselineDrift = new Vector3(0.1f, 0, 0);
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;                    

    [Header("References")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference leftControllerVelocity;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] InputActionReference rightControllerVelocity;
    [SerializeField] Transform trackingReference;

    XRBodyTransformer _bodyTransformer;
    float _cooldownTimer;
    Vector3 _currentVelocity;

    bool _prevLeftStroke;
    bool _prevRightStroke;

    protected override void Awake()
    {
        // Wire into locomotionmediator
        base.Awake();  

        // Grab parent locomotion mediator if not assigned
        mediator = mediator ?? GetComponentInParent<LocomotionMediator>();
        _bodyTransformer = GetComponentInParent<XRBodyTransformer>();
    }

    protected void OnEnable()
    {
        leftControllerSwimReference.action.Enable();
        leftControllerVelocity.action.Enable();
        rightControllerSwimReference.action.Enable();
        rightControllerVelocity.action.Enable();
    }

    protected void OnDisable()
    {
        leftControllerSwimReference.action.Disable();
        leftControllerVelocity.action.Disable();
        rightControllerSwimReference.action.Disable();
        rightControllerVelocity.action.Disable();
    }

    protected override Vector3 ComputeDesiredMove(out bool attemptingMove)
    {
        attemptingMove = false;
        _cooldownTimer += Time.deltaTime;

        // apply drag to swim momentum
        _currentVelocity = Vector3.MoveTowards(
            _currentVelocity,
            Vector3.zero,
            dragForce * Time.deltaTime);

        // detect stroke on release
        bool leftPressed = leftControllerSwimReference.action.IsPressed();
        bool rightPressed = rightControllerSwimReference.action.IsPressed();
        bool leftReleased = _prevLeftStroke && !leftPressed;
        bool rightReleased = _prevRightStroke && !rightPressed;
        _prevLeftStroke = leftPressed;
        _prevRightStroke = rightPressed;                 

        if (_cooldownTimer > minTimeBetweenStrokes)
        {
            Vector3 combinedHandVel = Vector3.zero;
            if (leftReleased) combinedHandVel += leftControllerVelocity.action.ReadValue<Vector3>();
            if (rightReleased) combinedHandVel += rightControllerVelocity.action.ReadValue<Vector3>();

            if (combinedHandVel.sqrMagnitude > minForce * minForce)
            {
                // convert to world impulse
                Vector3 localVel = -combinedHandVel;
                Vector3 worldImpulse = trackingReference.TransformDirection(localVel) * swimForce;

                // if the new impulse points opposite your current velocity by more than 90°, clear the old
                if (Vector3.Dot(worldImpulse.normalized, _currentVelocity.normalized) < 0f)
                    _currentVelocity = Vector3.zero;

                // add to swim momentum
                _currentVelocity += worldImpulse;
                _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, maxSpeed);
                _cooldownTimer = 0f;
            }
        }

        // combine swim drift + constant baseline current
        Vector3 totalVelocity = _currentVelocity + baselineDrift;

// if we have any velocity, signal movement
if (totalVelocity.sqrMagnitude > 0.0001f)
{
    attemptingMove = true;
    // return distance this frame
    return totalVelocity * Time.deltaTime;
}

return Vector3.zero;
    }

    void LateUpdate()
    {
        if (_currentVelocity.sqrMagnitude <= 0.001f || _bodyTransformer == null)
            return;

        // Get the XROrigin from the transformer
        var xrOrigin = _bodyTransformer.xrOrigin;
        if (xrOrigin == null)
            return;

        xrOrigin.Origin.transform.position += _currentVelocity * Time.deltaTime;
    }
}
