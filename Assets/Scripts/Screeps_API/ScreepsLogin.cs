﻿using System;
using System.Collections.Generic;
using Common;
using Screeps3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps_API
{
    public class ScreepsLogin : MonoBehaviour
    {
        [SerializeField] private ScreepsAPI _api;
        [SerializeField] private Toggle _save;
        [SerializeField] private Toggle _ssl;
        [SerializeField] private TMP_InputField _port;
        [SerializeField] private TMP_InputField _email;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_InputField _token;
        [SerializeField] private TMP_Dropdown _serverSelect;
        [SerializeField] private Button _connect;
        [SerializeField] private FadePanel _panel;
        [SerializeField] private Button _addServer;
        [SerializeField] private Button _removeServer;
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";
        private CacheList _servers;
        private int _serverIndex;
        private string _savePath = "servers";

        private void Start()
        {
            GameManager.OnModeChange += OnModeChange;
            
            LoadCache();
            UpdateServerDropdown();
            UpdateFieldVisibility();
            UpdateFieldContent();
            
            _connect.onClick.AddListener(OnClick);
            _serverSelect.onValueChanged.AddListener(OnServerChange);
            _addServer.onClick.AddListener(OnAddServer);
            _removeServer.onClick.AddListener(OnRemoveServer);
        }

        private void OnModeChange(GameMode mode)
        {
            if (mode == GameMode.Login)
                _panel.Show();
            else 
                _panel.Hide();
        }

        private void OnRemoveServer()
        {
            if (_serverIndex == 0)
                return;
            
            _servers.RemoveAt(_serverIndex);
            OnServerChange(_serverIndex - 1);
            UpdateServerDropdown();
            SaveManager.Save(_savePath, _servers);
        }

        private void UpdateServerDropdown()
        {
            _serverSelect.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var server in _servers)
            {
                options.Add(new TMP_Dropdown.OptionData(server.Address.HostName));
            }
            _serverSelect.AddOptions(options);
            _serverSelect.value = _serverIndex;
        }

        private void OnAddServer()
        {
            PlayerInput.Get("Server Hostname\n<size=12>example: 127.0.0.1</size>", OnSubmitServer);
        }

        private void OnSubmitServer(string hostName)
        {
            if (hostName == null)
                return;
            
            var server = new ServerCache();
            server.Address.HostName = hostName;
            _servers.Add(server);
            OnServerChange(_servers.IndexOf(server));
            UpdateServerDropdown();
            SaveManager.Save(_savePath, _servers);
        }

        private void OnServerChange(int serverIndex)
        {
            PlayerPrefs.SetInt("serverIndex", serverIndex);
            _serverIndex = serverIndex;
            UpdateFieldVisibility();
            UpdateFieldContent();
        }

        private void UpdateFieldVisibility()
        {
            var isPublic = _servers[_serverIndex].Address.HostName.ToLowerInvariant() == "screeps.com";
            _ssl.gameObject.SetActive(!isPublic);
            _port.gameObject.SetActive(!isPublic);
            _email.gameObject.SetActive(!isPublic);
            _password.gameObject.SetActive(!isPublic);
            _removeServer.gameObject.SetActive(_serverIndex != 0);
            _token.gameObject.SetActive(isPublic);
        }

        private void UpdateFieldContent()
        {
            var cache = _servers[_serverIndex];
            _port.text = cache.Address.Port ?? "";
            _email.text = cache.Credentials.Email ?? "";
            _token.text = cache.Credentials.Token ?? "";
            _password.text = cache.Credentials.Password ?? "";
            _ssl.isOn = cache.Address.Ssl;
            _save.isOn = cache.SaveCredentials;
        }

        private void LoadCache()
        {
            _servers = SaveManager.Load<CacheList>(_savePath);
            if (_servers == null)
            {
                _servers = new CacheList();
                var publicServer = new ServerCache();
                publicServer.Address.HostName = "Screeps.com";
                publicServer.Address.Ssl = true;
                _servers.Add(publicServer);
            }
        }

        private void OnClick()
        {
            var cache = _servers[_serverIndex];
            cache.SaveCredentials = _save.isOn;
            cache.Address.Port = _port.text;
            cache.Address.Ssl = _ssl.isOn;
            
            cache.SaveCredentials = _save.isOn;
            if (cache.SaveCredentials)
            {
                cache.Credentials.Email = _email.text;
                cache.Credentials.Password = _password.text;
                cache.Credentials.Token = _token.text;
            }

            SaveManager.Save(_savePath, _servers);
            NotifyText.Message("Connecting...");
            _api.Connect(cache);
        }
    }

    [Serializable]
    public class Credentials
    {
        public string Token;
        public string Email;
        public string Password;
    }

    [Serializable]
    public class Address
    {
        public bool Ssl;
        public string HostName;
        public string Port;
        public string Path = "/";

        public string Http(string path = "")
        {
            if (path.StartsWith("/") && Path.EndsWith("/"))
            {
                path = path.Substring(1);
            }
            var protocol = Ssl ? "https" : "http";
            var port = HostName.ToLowerInvariant() == "screeps.com" ? "" : string.Format(":{0}", this.Port);
            // Debug.Log(string.Format("{0}://{1}{2}{3}{4}", protocol, hostName, port, this.path, path));
            return string.Format("{0}://{1}{2}{3}{4}", protocol, HostName, port, this.Path, path);
        }
    }
    
    [Serializable]
    public class CacheList : List<ServerCache> { } 
    // I'm not sure why it is necessary to use this class rather than just the list, but the binary formatter
    // seems to require it
}