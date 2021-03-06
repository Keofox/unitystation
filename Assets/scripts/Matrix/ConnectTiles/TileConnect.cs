﻿using Sprites;
using UnityEngine;
using UnityEngine.Events;

namespace Matrix {

    public enum SpritePosition {
        UpperRight = 0, LowerRight = 1, LowerLeft = 2, UpperLeft = 3
    }

    [ExecuteInEditMode]
    public class TileConnect: MonoBehaviour {
        public SpritePosition spritePosition;
        public bool ConnectToAll { get; set; }
        public TileType TileType {
            get {
                return transform.parent.GetComponent<RegisterTile>().tileType;
            }
        }

        public string spriteName;
        private SpritePosition currentSpritePosition;

        private int[] c = new int[3];

        private Sprite[] sprites;
        private SpriteRenderer spriteRenderer;

        private int[] offsets = { 0, 1, 1, 1, 0, -1, -1, -1 };
        private int[,] adjacentTiles = new int[3, 2];

        private int x = -1, y = -1;
        private int offsetIndex;

        private UnityAction<TileType>[] listeners = new UnityAction<TileType>[3];

        void Awake() {
            sprites = SpriteManager.ConnectSprites[spriteName];
            spriteRenderer = GetComponent<SpriteRenderer>();
            offsetIndex = (int) spritePosition * 2;
        }

        void OnValidate() {
            if(spritePosition != currentSpritePosition) {
                currentSpritePosition = spritePosition;
                offsetIndex = (int) spritePosition * 2;
                UpdateListeners();
            }
        }

        public void ChangeParameter(int index) {
            bool connected;
            if(ConnectToAll) {
                connected = !Matrix.IsSpaceAt(adjacentTiles[index, 0], adjacentTiles[index, 1]);
            } else {
                connected = Matrix.HasTypeAt(adjacentTiles[index, 0], adjacentTiles[index, 1], TileType);
            }
            c[index] = connected ? 1 : 0;
            UpdateSprite();
        }

        public void UpdatePosition(int new_x, int new_y) {
            x = new_x;
            y = new_y;

            UpdateListeners();
            CheckAdjacentTiles();
        }

        private void UpdateListeners() {
            for(int i = 0; i < 3; i++) {
                if(listeners[i] != null) {
                    Matrix.RemoveListener(adjacentTiles[i, 0], adjacentTiles[i, 1], listeners[i]);
                } else {
                    int i2 = i;
                    listeners[i] = new UnityAction<TileType>(x => ChangeParameter(i2));
                }

                if(x >= 0) {
                    adjacentTiles[i, 0] = x + offsets[(offsetIndex + i) % 8];
                    adjacentTiles[i, 1] = y + offsets[(offsetIndex + i + 2) % 8];

                    Matrix.AddListener(adjacentTiles[i, 0], adjacentTiles[i, 1], listeners[i]);
                }
            }
        }

        void OnDestroy() {
            for(int i = 0; i < 3; i++) {
                Matrix.RemoveListener(adjacentTiles[i, 0], adjacentTiles[i, 1], listeners[i]);
            }
        }

        private void CheckAdjacentTiles() {
            for(int i = 0; i < 3; i++) {
                ChangeParameter(i);
            }
        }

        private void UpdateSprite() {
            int code = c[0] * 3 + c[2] * 1 + (c[0] * c[1] * c[2]) * -2;
            int index = (int) spritePosition * 5 + code;
            spriteRenderer.sprite = sprites[index];
        }
    }
}