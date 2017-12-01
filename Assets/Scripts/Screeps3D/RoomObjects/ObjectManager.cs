﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using Utils;

namespace Screeps3D {
    [DisallowMultipleComponent]
    public class ObjectManager : BaseSingleton<ObjectManager> {
        [SerializeField] private GameObject[] prefabs;
        public Dictionary<string, RoomObject> Cache { get; private set; }
        private Dictionary<string, ObjectView> prototypes = new Dictionary<string, ObjectView>();
        private Dictionary<string, ObjectView> viewCache = new Dictionary<string, ObjectView>();
        private ObjectFactory factory = new ObjectFactory();

        public List<ObjectView> GetViews()
        {
            return viewCache.Values.ToList();
        }

        private void Start() {
            Cache = new Dictionary<string, RoomObject>();

            foreach (var prefab in prefabs) {
                var go = Instantiate(prefab);
                go.name = prefab.name;
                var view = go.GetComponent<ObjectView>();
                if (view == null) continue;
                prototypes[prefab.name] = view;
                go.SetActive(false);
                go.transform.SetParent(transform);
            }
        }

        internal RoomObject GetInstance(string id, JSONObject data, EntityView entityView) {
            if (Cache.ContainsKey(id)) {
                var existingRoomObject = Cache[id]; 
                var existingView = existingRoomObject.View;
                if (existingView != null) {
                    existingView.Show();
                    existingView.transform.SetParent(entityView.transform, false);
                }
                existingRoomObject.Init(data, existingView);
                
                return existingRoomObject;
            }

            var type = data["type"].str;
            
            var view = GetView(id, type);
            if (view != null) {
                view.transform.SetParent(entityView.transform, false);   
            }
            
            var roomObject = factory.Get(type);
            roomObject.Init(data, view);
            Cache[id] = roomObject;
            
            return roomObject;
        }
        
        private ObjectView GetView(string id, string type) {
            if (viewCache.ContainsKey(id)) 
                return viewCache[id];
            if (!prototypes.ContainsKey(type))
                return null;
            
            var so = Instantiate(prototypes[type].gameObject).GetComponent<ObjectView>();
            so.gameObject.SetActive(true);
            viewCache[id] = so;
            return so;
        }

        public void Remove(string id, string roomName) {
            if (!Cache.ContainsKey(id)) {
                return;
            }
            var roomObject = Cache[id];
            var creep = roomObject as Creep;
            if (creep != null) {
                if (creep.RoomName == roomName) {
                    creep.View.Hide();
                }
            } else if (roomObject.View != null) {
                roomObject.View.Hide();
            }
        }
    }
}