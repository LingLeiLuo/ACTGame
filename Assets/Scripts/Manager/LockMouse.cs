using UnityEngine;

public class LockMouse : MonoBehaviour
{
    private void Start()
    {
        LockMouseAtGameStart();
    }

    private void LockMouseAtGameStart()
    {
        //将光标锁定在屏幕中间
        Cursor.lockState = CursorLockMode.Locked;
        //隐藏光标
        Cursor.visible = false;
    }
}