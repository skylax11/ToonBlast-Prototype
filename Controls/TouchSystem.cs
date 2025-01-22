using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TouchSystem : MonobehaviourSingleton<TouchSystem>
{
    [SerializeField] private PlayerInput _inputSystem;  
    private InputAction touchAction;  

    private Camera mainCamera;

    private bool isTouching = false;

    public bool LetTouch = true;

    void Start()
    {
        touchAction = _inputSystem.actions["TouchInput"]; 
        mainCamera = Camera.main; 
    }

    void Update()
    {
        if (!LetTouch)
            return;

        if (touchAction.IsPressed())
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began && !isTouching)
            {
                isTouching = true; 

                Vector2 touchPosition = touch.position.ReadValue(); 

                Vector2 worldPos = mainCamera.ScreenToWorldPoint(touchPosition);

                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent(out Brick clickedBrick))
                        clickedBrick.Click();
                }
            }
        }
        else
            isTouching = false;
    }
}
