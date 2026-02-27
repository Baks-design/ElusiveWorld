using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/CameraInputData", order = 0)]
    public class CameraInputData : ScriptableObject
    {
        Vector2 inputVector;
        bool isZooming;
        bool zoomClicked;
        bool zoomReleased;
  
        public Vector2 InputVector
        {
            get => inputVector;
            set => inputVector = value;
        }
        public bool IsZooming
        {
            get => isZooming;
            set => isZooming = value;
        }
        public bool ZoomClicked
        {
            get => zoomClicked;
            set => zoomClicked = value;
        }
        public bool ZoomReleased
        {
            get => zoomReleased;
            set => zoomReleased = value;
        }
     
        public void ResetInput()
        {
            inputVector = Vector2.zero;
            isZooming = false;
            zoomClicked = false;
            zoomReleased = false;
        }
    }
}
