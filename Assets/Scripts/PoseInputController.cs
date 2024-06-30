using System;
using Mediapipe.Unity;
using UnityEngine;

public class PoseInputController : MonoBehaviour
{
    [SerializeField] 
    private PoseLandmarkListAnnotation listAnnotation;
    // [SerializeField] 
    // private float deltaThreshold = 0.5f;
    [SerializeField, Tooltip("The maximum angle between hands and elbow. Used to smooth the plane movement")] 
    private float _maxAngle = 70;
    [SerializeField]
    private float _verticalDeadZoneDegrees = 10;
    public bool InvertVertical = true;
    private float _verticalInverted = 1;
    
    [Header("Left And right movement")]
    [SerializeField, Tooltip("The max Angle the player needs to lean to move the plane left or right, lower values should make it snappier, but less granular.")] 
    private float _maxHorizontalAngle = 30;
    [SerializeField] 
    private float _horizontolDeadZoneDegrees = 5;

    private Vector2 _input;

    private void Start()
    {
        _verticalInverted = InvertVertical ? -1 : 1;
    }

    private void Update()
    {
        var result = GetInputFromPose();
        
        _input = NormalizeInputVector(result);
        // _input = CalculateMovement(result.x, result.y);
    }

    public Vector2 GetInput()
    {
        return _input;
    }

    private Vector2 NormalizeInputVector(Vector2 angleVector)
    {
        // x is the vertical angle, y is the horizontal angle
        
        float horizontal = 0;
        if (!(Mathf.Abs(angleVector.y) < _horizontolDeadZoneDegrees))
        {
            horizontal = Mathf.Clamp(angleVector.y / -_maxHorizontalAngle, -1, 1); // TODO: we need to introduce a deadzone 
        }
        
        float vertical = 0;
        if (!(Mathf.Abs(angleVector.x) < _verticalDeadZoneDegrees))
        {
            // 180 is neutral, negative is up.
            vertical = Mathf.Clamp((angleVector.x - 180) / _maxAngle * _verticalInverted, -1, 1);
        }
        
        return new Vector2(horizontal, vertical);
    }
    
    private Vector2 GetInputFromPose()
    {
        var lefthand = listAnnotation[15];
        var leftshoulder = listAnnotation[11];
        
        var righthand = listAnnotation[16];
        var rightshoulder = listAnnotation[12];

        // Calculate vectors from shoulders to hands
        var leftVector = lefthand.transform.position - leftshoulder.transform.position;
        var rightVector = righthand.transform.position - rightshoulder.transform.position;

        // Calculate the angles with the x-axis
        float leftAngle = Mathf.Clamp(-Mathf.Atan2(-leftVector.y, -leftVector.x) * Mathf.Rad2Deg, -_maxAngle, _maxAngle);
        float rightAngle = Mathf.Clamp(Mathf.Atan2(rightVector.y, rightVector.x) * Mathf.Rad2Deg, -_maxAngle, _maxAngle);

        
        var horizontalLine = righthand.transform.position - lefthand.transform.position;

        var verticalAngle = GetAdjustedAngleBetweenVectors(rightVector, leftVector, horizontalLine);
        
        // Positive means left, negative means right
        var horizontalAngle = Mathf.Atan2(horizontalLine.y, horizontalLine.x) * Mathf.Rad2Deg;
        
        return new Vector2(verticalAngle, horizontalAngle);
        // return new Vector2(leftAngle, rightAngle);
    }

    float GetAdjustedAngleBetweenVectors(Vector2 v1, Vector2 v2, Vector2 j)
    {
        // Get the initial angle between vectors
        float angle = Vector2.Angle(v1, v2);
    
        // Check if both vectors are below vector J
        if (AreBothVectorsBelowJ(v1, v2, j))
        {
            // Adjust the angle to be above 180 degrees
            angle = 360 - angle;
        }
    
        return angle;
    }

    bool AreBothVectorsBelowJ(Vector2 v1, Vector2 v2, Vector2 j)
    {
        // Normalize J to use it as a reference direction
        Vector2 jNormalized = j.normalized;
    
        // Calculate the perpendicular vector to J (rotate 90 degrees counterclockwise)
        Vector2 jPerpendicular = new Vector2(-jNormalized.y, jNormalized.x);
    
        // Check if both vectors are on the "below" side of J
        return Vector2.Dot(v1, jPerpendicular) < 0 && Vector2.Dot(v2, jPerpendicular) < 0;
    }
    
    private Vector2 CalculateMovement(float leftAngle, float rightAngle)
    {
        var leftNormalized = leftAngle / _maxAngle;
        var rightNormalized = rightAngle / _maxAngle;
        

        var movingUp = leftNormalized > 0 && rightNormalized > 0;
        var movingDown = leftNormalized < 0 && rightNormalized < 0;
        var movingLeft = leftNormalized < 0 && rightNormalized > 0;
        var movingRight = leftNormalized > 0 && rightNormalized < 0;
        
        if(movingUp || movingDown){
            return new Vector2(0, rightNormalized);
        }

        if(movingLeft || movingRight){
            return new Vector2(leftNormalized, 0);
        }

        return Vector2.zero;
    }
}
