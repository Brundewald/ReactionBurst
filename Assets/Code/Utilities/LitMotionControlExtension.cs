using System;
using System.Collections.Generic;
using LitMotion;

namespace Code.Utilities
{
    public static class LitMotionControlExtension
    {
        private static List<MotionHandle> _activeHandles = new List<MotionHandle>();
        private static bool _isPaused;

        public static MotionHandle RegisterActiveHandle(this MotionHandle handle)
        {
            if(_activeHandles.Contains(handle))
                return handle;
            
            _activeHandles.Add(handle);
            handle.PlaybackSpeed = _isPaused ? 0 : 1;
            handle.GetAwaiter().OnCompleted(RemoveHandle(handle));
            return handle;
        }

        private static Action RemoveHandle(MotionHandle handle)
        {
            return () => _activeHandles.Remove(handle);
        }

        public static void UnregisterActiveHandle(this MotionHandle handle)
        {
            if(!_activeHandles.Contains(handle))
                return;
            
            _activeHandles.Remove(handle);
        }

        public static void PauseMotion()
        {
            _isPaused = true;
            _activeHandles.ForEach(handle => handle.PlaybackSpeed = 0);
        }
        
        public static void UnpauseMotion()
        {
            _isPaused = false;
            _activeHandles.ForEach(handle => handle.PlaybackSpeed = 1);
        }

        public static void SetMotionSpeed(float speed)
        {
            _activeHandles.ForEach(handle => handle.PlaybackSpeed = speed);
        }

        public static void CancelMotion()
        {
            _isPaused = false;
            for (var i = 0; i < _activeHandles.Count; i++)
            {
                var handle = _activeHandles[i];
                if (handle.IsActive())
                    handle.Cancel();
            }

            _activeHandles.Clear();
        }
    }
}