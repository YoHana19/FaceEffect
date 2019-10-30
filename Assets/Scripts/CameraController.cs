using System;
using System.Text;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures.TransformGestures.Clustered;

namespace CameraControllerMoblie
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private ScreenTransformGesture manipulationGesture;
        [SerializeField] private ScreenTransformGesture twoFingerMoveGesture;
        [SerializeField] private ClusteredScreenTransformGesture multiFingerMoveGesture;
        private const float MOVE_SPEED = 3f;
        private const float ROTATION_SPEED = 0.1f;

        private void OnEnable()
        {
            manipulationGesture.Transformed        += ManipulationTransformedHandler;
            twoFingerMoveGesture.Transformed       += TwoFingerTransformHandler;
            multiFingerMoveGesture.Transformed     += MultiFingerTransformHandler;
        }

        private void OnDisable()
        {
            manipulationGesture.Transformed        -= ManipulationTransformedHandler;
            twoFingerMoveGesture.Transformed       -= TwoFingerTransformHandler;
            multiFingerMoveGesture.Transformed     -= MultiFingerTransformHandler;
        }

        private void ManipulationTransformedHandler(object sender, EventArgs e)
        {
            var rotation = new Vector3(-manipulationGesture.DeltaPosition.y, manipulationGesture.DeltaPosition.x, 0f);
            rotation = rotation * ROTATION_SPEED + transform.eulerAngles;
            transform.eulerAngles = rotation;
        }

        private void TwoFingerTransformHandler(object sender, EventArgs e)
        {
            transform.position += transform.forward * (1 - twoFingerMoveGesture.DeltaScale) * MOVE_SPEED;
        }
        
        private void MultiFingerTransformHandler(object sender, EventArgs e)
        {
//            var sb = new StringBuilder();
//            sb.AppendLine( "変形中" );
//            sb.AppendLine( "DeltaPosition : "      + g.DeltaPosition );
//            sb.AppendLine( "DeltaRotation: "       + g.DeltaRotation );
//            sb.AppendLine( "DeltaScale: "          + g.DeltaScale );
//            sb.AppendLine( "RotationAxis: "        + g.RotationAxis );
//            Debug.Log( sb );
            
            if (multiFingerMoveGesture.NumPointers == 3)
            {
                transform.position += new Vector3(multiFingerMoveGesture.DeltaPosition.x, multiFingerMoveGesture.DeltaPosition.y, 0) * 0.01f;
            }
        }
    }
}
