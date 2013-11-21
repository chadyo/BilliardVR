/*  HydraDeck camera code by Teddy0@gmail.com
  
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEngine;

public class HydraDeckCamera : MonoBehaviour
{
    public Vector3 HydraToWorldScale = new Vector3(0.001f, 0.001f, 0.001f);
    public float movementSpeed = 1.0f;
    public float movementSmoothingFactor = 0.2f;
    public float sprintMultiplier = 1.5f;
    public float worldFloorY = 0.0f;

    //This is the distance (in Hydra units) from the Hydra Base to the Floor
    [HideInInspector]
    public float HydraFloorY;

    //This is the orientation of the left hydra unit (when it's attached to the chest)
    private Quaternion LeftRotationOffset;
    [HideInInspector]
    public Quaternion StartCameraOffset;

    //This is the vector from the left hydra unit to the base of the player's neck
    private Vector3 ChestToNeckOffset;

    //This is the location of the "body" in the world. It's moved by using the joystick
    [HideInInspector]
    public Vector3 BodyPosition;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;

    public enum CameraState
    {
        Disabled,
        CalibrateFloor,
        CalibrateChest,
        CalibrateNeck,
		ShowInstructions,
        Enabled,
		NoHydra
    };
    [HideInInspector]
    public CameraState State = CameraState.Disabled;

    void Start ()
    {
        targetPosition = transform.position;
        currentVelocity = Vector3.zero;
        HydraFloorY = -860;
    }

    void OnGUI()
    {
        switch (State)
        {
            case CameraState.Disabled:
                break;

            case CameraState.CalibrateFloor:
                GUI.Box(new Rect((7*Screen.width)/32, Screen.height/2, 170, 60), "Touch Right Hydra to Floor\nPull Right Trigger");
                GUI.Box(new Rect((20*Screen.width)/32, Screen.height/2, 170, 60), "Touch Right Hydra to Floor\nPull Right Trigger");
                break;

            case CameraState.CalibrateChest:
                GUI.Box(new Rect((7 * Screen.width) / 32, Screen.height / 2, 170, 90), "Attach Left Hydra to Chest\nFace Forwards\nPull Right Trigger");
                GUI.Box(new Rect((20 * Screen.width) / 32, Screen.height / 2, 170, 90), "Attach Left Hydra to Chest\nFace Forwards\nPull Right Trigger");
                break;

            case CameraState.CalibrateNeck:
                GUI.Box(new Rect((7 * Screen.width) / 32, Screen.height / 2, 190, 60), "Touch Hydra to back of neck\nPull Right Trigger");
                GUI.Box(new Rect((20 * Screen.width) / 32, Screen.height / 2, 190, 60), "Touch Hydra to back of neck\nPull Right Trigger");
                break;
			
            case CameraState.ShowInstructions:
                GUI.Box(new Rect((7 * Screen.width) / 32, Screen.height / 2, 210, 120), "Target cue ball: Right Trigger (RT)\nRerack: Right Button (RB)\n\nRight Trigger to Start");
                GUI.Box(new Rect((20 * Screen.width) / 32, Screen.height / 2, 210, 120), "Target cue ball: Right Trigger (RT)\nRerack: Right Button (RB)\n\nRight Trigger to Start");
                break;			

            case CameraState.Enabled:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SixenseInput.Controller LeftController = SixenseInput.GetController(SixenseHands.LEFT);
        SixenseInput.Controller RightController = SixenseInput.GetController(SixenseHands.RIGHT);
        if( LeftController == null || RightController == null )
            return;

        switch (State)
        {
            case CameraState.Disabled:
                if( SixenseInput.IsBaseConnected(0) )
                {
                    //Get our starting position
                    BodyPosition = transform.position;
                    BodyPosition.y = worldFloorY;
                    targetPosition = BodyPosition;

                    //If you have a normal mouse/keyboard camera controller, disable it here!
                    //FlightCamera myCamera = gameObject.GetComponent<MyCamera>();
                    //if (myCamera != null)
                    //    myCamera.enabled = false;

                    //Get the starting yaw of the scene
                    StartCameraOffset = transform.rotation;

                    //lol, why doesn't Unity have a Quaternion.Normalize function?
                    StartCameraOffset.x = 0;
                    StartCameraOffset.z = 0;
                    StartCameraOffset = Quaternion.Lerp(StartCameraOffset, StartCameraOffset, 1);

                    State = CameraState.CalibrateFloor;
                }
                break;

            case CameraState.CalibrateFloor:
                if( RightController.GetButtonDown(SixenseButtons.TRIGGER) )
                {
                    //Get the distance from the floor to the base
                    HydraFloorY = RightController.Position.y;

                    State = CameraState.CalibrateChest;
                }
                break;

            case CameraState.CalibrateChest:
                if (RightController.GetButtonDown(SixenseButtons.TRIGGER))
                {
                    //Get the Left Hydra's orientation offset
                    LeftRotationOffset = Quaternion.Inverse(LeftController.Rotation);

                    //Get the base orientation of the head rotation, so it matches up
                    Quaternion BaseCameraYaw = new Quaternion(0,0,0,0);
                    OVRDevice.GetOrientation(0, ref BaseCameraYaw);
                    //lol, why doesn't Unity have a Quaternion.Normalize function?
                    BaseCameraYaw.x = 0;
                    BaseCameraYaw.z = 0;
                    BaseCameraYaw = Quaternion.Lerp(BaseCameraYaw, BaseCameraYaw, 1);

                    StartCameraOffset = StartCameraOffset * BaseCameraYaw;

                    State = CameraState.CalibrateNeck;
                }
                break;

            case CameraState.CalibrateNeck:
                if( RightController.GetButtonDown(SixenseButtons.TRIGGER) )
                {
                    ChestToNeckOffset = RightController.Position - LeftController.Position;

                    State = CameraState.ShowInstructions;
                }
                break;
			
			case CameraState.ShowInstructions:
                if( RightController.GetButtonDown(SixenseButtons.TRIGGER) )
                {
                    State = CameraState.Enabled;
                }
                break;
			
            case CameraState.Enabled:
				UpdateHydraCamera(LeftController, RightController);
                break;
        }
    }

    void UpdateHydraCamera(SixenseInput.Controller LeftController, SixenseInput.Controller RightController)
    {
        #region Movement
        float movementMultiplier = movementSpeed * Time.deltaTime * (RightController.GetButtonDown(SixenseButtons.TRIGGER) ? sprintMultiplier : 1.0f);

        Quaternion FullBodyRotation = StartCameraOffset * LeftController.Rotation * LeftRotationOffset;

        //lol, why doesn't Unity have a Quaternion.Normalize function?
        Quaternion rotationToFaceForward = FullBodyRotation;
        rotationToFaceForward.x = 0;
        rotationToFaceForward.z = 0;
        rotationToFaceForward = Quaternion.Lerp(rotationToFaceForward, rotationToFaceForward, 1);

        float forwardBackwardTranslation = RightController.JoystickY * movementMultiplier;
        float leftRightTranslation = RightController.JoystickX * movementMultiplier;

        Vector3 movementVector = new Vector3(leftRightTranslation, 0, forwardBackwardTranslation);
        if( movementVector != Vector3.zero )
        {
            movementVector = rotationToFaceForward * movementVector;
            movementVector.y = 0;
            targetPosition += movementVector;
        }

        Vector3 currentPosition = BodyPosition;
        if( !Approximately(currentPosition, targetPosition) )
        {
            currentPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, movementSmoothingFactor, float.PositiveInfinity, Time.deltaTime);
            BodyPosition = currentPosition;
        }

        //Get the position of the left controller, and subtract the floor height
        Vector3 HydraLocation = new Vector3(LeftController.Position.x,
                                            LeftController.Position.y - HydraFloorY,
                                            LeftController.Position.z);

        //Orientate it to our scene
        HydraLocation = StartCameraOffset * HydraLocation;

        //Add the rotated offset from left hyrda to neck
        Vector3 ChestToNeckLocal = FullBodyRotation * ChestToNeckOffset;
        HydraLocation += ChestToNeckLocal;

        //Apply the scaling from Hydra units to World units
        HydraLocation.Scale(HydraToWorldScale);

        //Add in the eye offset, rotated by the body's yaw
        OVRCameraController CameraController = GetComponentInChildren<OVRCameraController>();
        Vector3 NeckToEyes = new Vector3( CameraController.EyeCenterPosition.x, CameraController.EyeCenterPosition.y, CameraController.EyeCenterPosition.z );
        NeckToEyes.y = 0; // not sure why, but changing height gives me wrong results?
        Vector3 NeckToEyesLocal = rotationToFaceForward * NeckToEyes;

        transform.position = BodyPosition + HydraLocation + NeckToEyesLocal;

        #endregion
    }

    private bool Approximately(Quaternion q1, Quaternion q2)
    {
        return Mathf.Approximately(q1.x, q2.x) &&
                Mathf.Approximately(q1.y, q2.y) &&
                Mathf.Approximately(q1.z, q2.z) &&
                Mathf.Approximately(q1.w, q2.w);
    }
    private bool Approximately(Vector3 v1, Vector3 v2)
    {
        return Mathf.Approximately(v1.x, v2.x) &&
                Mathf.Approximately(v1.y, v2.y) &&
                Mathf.Approximately(v1.z, v2.z);
    }
}
