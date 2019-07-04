﻿using System;
using Common;
using Screeps_API;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Screeps3D
{
    public class GameManager : BaseSingleton<GameManager>
    {
        public static GameMode CurrentMode { get; private set; }
        public static event Action<GameMode> OnModeChange;

        [SerializeField] private GameMode _defaultMode;
        [SerializeField] private FadePanel _exitCue;

        public override void Awake()
        {
            if (_defaultMode != GameMode.Login && !ScreepsAPI.IsConnected)
                SceneManager.LoadScene(0);
            else
                OnModeChange = null;
            
            base.Awake();
        }

        private void Start()
        {
            if (ScreepsAPI.Console == null)
                SceneManager.LoadScene(0);
            
            ScreepsAPI.OnConnectionStatusChange += OnConnectionStatusChange;
            _exitCue.OnFinishedAnimation += OnExitCue;

            Init();
        }

        private void Init()
        {
            PoolLoader.Init();
            PrefabLoader.Init();
        }

        private void OnDestroy()
        {
            ScreepsAPI.OnConnectionStatusChange -= OnConnectionStatusChange;
        }

        private void OnExitCue(bool isVisible)
        {
            if (isVisible || _defaultMode == CurrentMode)
                return;
            
            if (CurrentMode == GameMode.Login)
                SceneManager.LoadScene(0);
            if (CurrentMode == GameMode.Room)
                SceneManager.LoadScene(1);
        }

        private void Update()
        {
            ChangeMode(_defaultMode);
            enabled = false;
        }

        private void OnConnectionStatusChange(bool isConnected)
        {
            if (isConnected)
            {
                ChangeMode(GameMode.Room);
            }
            else
            {
                ChangeMode(GameMode.Login);
            }
        }

        public static void ChangeMode(GameMode mode)
        {
            CurrentMode = mode;
            if (OnModeChange != null)
                OnModeChange(mode);
        }
    }
    
    public enum GameMode
    {
        Login,
        Options,
        Room,
        Map,
    }
}