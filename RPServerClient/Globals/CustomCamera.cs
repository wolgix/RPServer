﻿using RAGE;

namespace RPServerClient.Globals
{
    public class CustomCamera : RAGE.Events.Script
    {
        private readonly uint _cameraHandle = RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA");
        private int _cameraID;

        public CustomCamera(Vector3 cameraPos, Vector3 cameraLookAt, bool active)
        {
            var camera = RAGE.Game.Cam.CreateCamera(_cameraHandle, true);

            RAGE.Game.Cam.SetCamCoord(camera, cameraPos.X, cameraPos.Y, cameraPos.Z);
            RAGE.Game.Cam.PointCamAtCoord(camera, cameraLookAt.X, cameraLookAt.Y, cameraLookAt.Z);
            RAGE.Game.Cam.SetCamActive(camera, active);
            RAGE.Game.Cam.RenderScriptCams(active, false, 0, true, true, 0);

            _cameraID = camera;
        }

        public void SetCameraState(bool state)
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, state);
            RAGE.Game.Cam.RenderScriptCams(state, false, 0, true, true, 0);
        }

        public void SetCameraPos(Vector3 pos, Vector3 rot)
        {
            RAGE.Game.Cam.SetCamCoord(_cameraID, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.PointCamAtCoord(_cameraID, rot.X, rot.Y, rot.Z);
        }

        public void DestroyCamera()
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, false);
            RAGE.Game.Cam.DestroyCam(_cameraID, true);
            RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, true, 0);
        }

        public Vector3 CameraPosition(int camera)
        {
            return RAGE.Game.Cam.GetCamCoord(camera);
        }
    }
}