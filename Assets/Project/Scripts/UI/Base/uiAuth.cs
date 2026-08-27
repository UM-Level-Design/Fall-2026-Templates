using System.Collections;
using System.Collections.Generic;
using LevelDesign.Async.Auth;
using UnityEngine;

namespace LevelDesign.UI {
    public class uiAuth : _InputAuth
    {
        [Header("Config")]
        public bool requestInput;
        public bool requestCursor;

        void OnEnable()
        {
            if(requestCursor) { CursorStateManager.Instance.RequestUnlock(this); }
            if(requestInput) { InputAuthManager.Instance.RequestInput(this); }
        }

        void OnDisable()
        {
            if(requestCursor) { CursorStateManager.Instance.RelinquishRequest(this); }
            if(requestInput) { InputAuthManager.Instance.RelinquishRequest(this); }
        }
    }
}