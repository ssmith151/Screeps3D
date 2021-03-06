﻿using System;
using System.Collections;
using System.Text;
using Common;
using Screeps3D;
using UnityEngine;
using UnityEngine.Networking;

namespace Screeps_API
{
    public class ScreepsHTTP : MonoBehaviour
    {
        public string Token { get; private set; }

        public void Request(string requestMethod, string path, RequestBody body = null,
            Action<string> onSuccess = null, Action onError = null)
        {
            // Debug.Log(string.Format("HTTP: attempting {0} to {1}", requestMethod, path));
            UnityWebRequest www;
            var fullPath = ScreepsAPI.Cache.Address.Http(path);
            if (requestMethod == UnityWebRequest.kHttpVerbGET)
            {
                if (body != null)
                {
                    fullPath = fullPath + body.ToQueryString();
                }
                www = UnityWebRequest.Get(fullPath);
            } else if (requestMethod == UnityWebRequest.kHttpVerbPOST)
            {
                www = new UnityWebRequest(fullPath, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body.ToString());
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
            } else
            {
                Debug.Log(string.Format("HTTP: request method {0} unrecognized", requestMethod));
                return;
            }

            Action<UnityWebRequest> onComplete = (UnityWebRequest outcome) =>
            {
                if (outcome.isNetworkError || outcome.isHttpError)
                {
                    NotifyText.Message(string.Format("HTTP: error ({1}), reason: {0}", outcome.error,
                        outcome.responseCode));
                    Debug.Log(string.Format("HTTP: error ({1}), reason: {0}", outcome.error, outcome.responseCode));
                    if (onError != null)
                    {
                        onError();
                    } 
                    else
                    {
                        Auth((reply) =>
                        {
                            Request(requestMethod, path, body, onSuccess);
                        }, () =>
                        {
                            ScreepsAPI.Instance.AuthFailure();
                        });
                    }
                } else
                {
                    // Debug.Log(string.Format("HTTP: success, data: \n{0}", outcome.downloadHandler.text));
                    if (outcome.downloadHandler.text.Contains("token"))
                    {
                        var reply = new JSONObject(outcome.downloadHandler.text);
                        var token = reply["token"];
                        if (token != null)
                        {
                            Token = token.str;
                            Debug.Log(string.Format("HTTP: found a token! {0}", Token));
                        }
                    }

                    if (onSuccess != null) onSuccess.Invoke(outcome.downloadHandler.text);
                }
            };

            StartCoroutine(SendRequest(www, onComplete));
        }

        private IEnumerator SendRequest(UnityWebRequest www, Action<UnityWebRequest> onComplete)
        {
            if (Token != null)
            {
                www.SetRequestHeader("X-Token", Token);
                www.SetRequestHeader("X-Username", Token);
            }
            yield return www.Send();
            onComplete(www);
        }

        public void Auth(Action<string> onSuccess, Action onError = null)
        {
            if (!string.IsNullOrEmpty(ScreepsAPI.Cache.Credentials.Token))
            {
                Token = ScreepsAPI.Cache.Credentials.Token;
                Request("GET", "/api/auth/me", null, onSuccess, onError);
            }
            else
            {
                var body = new RequestBody();
                body.AddField("email", ScreepsAPI.Cache.Credentials.Email);
                body.AddField("password", ScreepsAPI.Cache.Credentials.Password);
                Request("POST", "/api/auth/signin", body, onSuccess, onError);
            }
        }

        public void GetUser(Action<string> onSuccess)
        {
            Request("GET", "/api/auth/me", null, onSuccess);
        }

        public void ConsoleInput(string message)
        {
            var body = new RequestBody();
            body.AddField("expression", message);
            body.AddField("shard", "shard0");
            Request("POST", "/api/user/console", body);
        }

        public void GetRoom(string roomName, string shard, Action<string> callback)
        {
            var body = new RequestBody();
            body.AddField("room", roomName);
            body.AddField("encoded", "0");
            body.AddField("shard", shard);

            Request("GET", "/api/game/room-terrain", body, callback);
        }

        public void GetRooms(string userId, Action<string> onSuccess)
        {
            var body = new RequestBody();
            body.AddField("id", userId);
            Request("GET", "/api/user/rooms", body, onSuccess);
        }
    }
}