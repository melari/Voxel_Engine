using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Voxel_Engine
{
    class Key
    {
        bool _down; public bool down { get { return _down; } }
        bool _released; public bool released { get { return _released; } }
        bool _pressed; public bool pressed { get { return _pressed; } }

        public Key(Keys key, KeyboardState ks, KeyboardState prev_ks)
        {
            _down = ks.IsKeyDown(key);
            _released = !_down && prev_ks.IsKeyDown(key);
            _pressed = _down && !prev_ks.IsKeyDown(key);
        }
    }

    class InputHandle
    {
        public InputHandle()
        {
        }        

        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;                        

        /// <summary>
        /// Gets the Key object associated with a given Keys object
        /// </summary>
        /// <param name="key">The Keys object to search for.</param>
        /// <returns>The associated Key object.</returns>
        public Key getKey(Keys key)
        {
            return new Key(key, keyboardState, prevKeyboardState);
        }

        private MouseState prevMouseState;
        private MouseState mouseState;
        private bool _leftMouseDown = false;
        public bool leftMouseDown
        {
            get { return _leftMouseDown; }
        }
        private bool _leftMousePressed = false;
        public bool leftMousePressed
        {
            get { return _leftMousePressed; }
        }
        private bool _leftMouseReleased = false;
        public bool leftMouseReleased
        {
            get { return _leftMouseReleased; }
        }

        private bool _rightMouseDown = false;
        public bool rightMouseDown
        {
            get { return _rightMouseDown; }
        }
        private bool _rightMousePressed = false;
        public bool rightMousePressed
        {
            get { return _rightMousePressed; }
        }
        private bool _rightMouseReleased = false;
        public bool rightMouseReleased
        {
            get { return _rightMouseReleased; }
        }

        private Vector2 _mousePos = new Vector2();
        public Vector2 mousePos
        {
            get { return _mousePos; }
        }

        public bool enabled = true;
        /// <summary>
        /// Updates the InputHandle object. Calls all update methods of Key objects in the watch list.
        /// </summary>
        public void update()
        {
            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            if (!enabled) { keyboardState = new KeyboardState(); }            

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            _leftMouseDown = (mouseState.LeftButton == ButtonState.Pressed);
            _leftMousePressed = (_leftMouseDown && (prevMouseState.LeftButton == ButtonState.Released));
            _leftMouseReleased = (!_leftMouseDown && (prevMouseState.LeftButton == ButtonState.Pressed));

            _rightMouseDown = (mouseState.RightButton == ButtonState.Pressed);
            _rightMousePressed = (_rightMouseDown && (prevMouseState.RightButton == ButtonState.Released));
            _rightMouseReleased = (!_rightMouseDown && (prevMouseState.RightButton == ButtonState.Pressed));

            _mousePos = new Vector2((float)mouseState.X, (float)mouseState.Y);
        }
    }
}
