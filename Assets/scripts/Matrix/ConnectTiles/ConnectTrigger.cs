﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matrix {

    [ExecuteInEditMode]
    public class ConnectTrigger: MonoBehaviour {

        public bool connectToAll;

        private Vector3 currentPosition;
        private TileConnect[] corners;
        
        void Start() {
            currentPosition = transform.position;

            corners = GetComponentsInChildren<TileConnect>();

            UpdatePosition();
        }
        
        //void LateUpdate() {
        //    if(currentPosition != transform.position) {
        //        currentPosition = transform.position;
        //        UpdatePosition();
        //    }
        //}

        public void UpdatePosition() {
            currentPosition = transform.position;

            int x = Mathf.RoundToInt(currentPosition.x);
            int y = Mathf.RoundToInt(currentPosition.y);

            foreach(var c in corners) {
                if(c.ConnectToAll != connectToAll)
                    c.ConnectToAll = connectToAll;

                c.UpdatePosition(x, y);
            }
        }
    }
}