using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CrossInput
{
    public static class InputManager
    {
        static Dictionary<string, int> mouseButtons = new Dictionary<string, int>()
        {
            {"Left", 0},
            {"Right", 1}
        };

        static InputManager() {}

        /// <summary>
        /// Alternative way to check if is mobile: SystemInfo.deviceType == DeviceType.Handheld
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="callback"></param>
        public static void DownHeld(string buttonName, Action<Vector3> callback)
        {
            Vector3 position = GetTouchOrClick();

            if (position != Vector3.zero)
            {
                callback(position);
            }
            else if (Input.GetButton(buttonName))
            {
                Vector3 axis = Vector3.zero;
                if (buttonName.Equals("Horizontal"))
                {
                    axis = new Vector3(Input.GetAxis(buttonName), 0.0f);
                }
                else if (buttonName.Equals("Vertical"))
                {
                    axis = new Vector3(0.0f, Input.GetAxis(buttonName));
                }

                callback(axis);
            }
            else
            {
                callback(Vector3.zero);
            }
        }

        public static void Down(string buttonName, Action<Vector3> callback, GameObject objReference = null)
        {
            var result = CheckInput(buttonName, objReference);
            if (result != Vector3.zero)
            {
                callback(result);
            }
        }

        public static Task<Vector3> Down(string buttonName, GameObject objReference = null)
        {
            return Task.Run(() =>
            {
                return CheckInput(buttonName, objReference);
            });
        }

        public static Vector3 GetTouchOrClick(string buttonName = "Horizontal", GameObject objReference = null)
        {

            Vector3 position = Vector3.zero;

            if (Input.touches.Length > 0)
            {

                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    position = touch.position;
                }

                //TODO: Refactor this to allow GetMouseButton too (For Horizontal movement)
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                position = Input.mousePosition;
            }

            if (buttonName.Equals("Horizontal"))
            {
                if (position != Vector3.zero)
                {
                    if (position.x < Screen.width / 2)
                    {
                        Debug.Log("LEFT");
                        position = new Vector3(-1, position.y);
                    }
                    else if (position.x > Screen.width / 2)
                    {
                        Debug.Log("RIGHT");
                        position = new Vector3(1, position.y);
                    }
                }
            }
            else if (buttonName.Equals("Vertical"))
            {
                if (position != Vector3.zero)
                {
                    if (objReference != null)
                    {
                        var worldPosition = Camera.main.ScreenToWorldPoint(position);
                        if (worldPosition.y < objReference.transform.position.y)
                        {
                            Debug.Log("DOWN");
                            position = new Vector3(position.x, -1);
                        }
                        else if (worldPosition.y > objReference.transform.position.y)
                        {
                            Debug.Log("UP");
                            position = new Vector3(position.x, 1);
                        }
                    }
                }
            }

            return position;
        }

        public static void OnTouch(Action<Vector3> callback) {
            int btnValue = GetMouseBtn("Left");

            if (Input.touches is Array && Input.touches.Length > 0)
            {
                Touch touch = Input.GetTouch(btnValue);
                if (touch.phase == TouchPhase.Began)
                {
                    callback(touch.position);
                }
            } if (Input.GetMouseButtonDown(btnValue)) {
                callback(Input.mousePosition);
            }
        }

        public static Vector3 CheckInput(string buttonName, GameObject objReference = null)
        {
            Vector3 position = GetTouchOrClick(buttonName, objReference);

            if (position != Vector3.zero)
            {
                return position;
            }
            else if (Input.GetButtonDown(buttonName))
            {
                Vector3 axis = Vector3.zero;
                if (buttonName.Equals("Horizontal"))
                {
                    axis = new Vector3(Input.GetAxis(buttonName), 0.0f);
                }
                else if (buttonName.Equals("Vertical"))
                {
                    axis = new Vector3(0.0f, Input.GetAxis(buttonName));
                }

                return axis;
            }

            return Vector3.zero;
        }

        public static void OnClick(string btnType, GameObject target, Action<Vector3> callback = null) {

            OnTouch((position) => {

                int btnValue = GetMouseBtn(btnType);

                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
                
                /// <todo> Verify why this DrawRay is not working
                Debug.DrawRay(hit2D.point, hit2D.normal, Color.red, 100f);

                if (hit2D.collider != null)
                {
                    GameObject touchedObject = hit2D.transform.gameObject;

                    if (touchedObject != null 
                        && (touchedObject.Equals(target) || touchedObject.transform.parent.gameObject.Equals(target)))
                    {
                        if (callback != null)
                        {
                            callback(position);
                        }
                    }
                
                    Debug.Log("Touched " + touchedObject.transform.name);
                }
            });
        }

        public static int GetMouseBtn(string type) {
            int btnValue = 0;
            mouseButtons.TryGetValue(type, out btnValue);
            
            return btnValue;
        }
    }
}