using UnityEngine;

public class DebugSkip : MonoBehaviour
{
    public void Skip(string landingScene)
    {
        AuthManager.Instance.StartLogin("dipgrp6@gmail.com", "Dip12345_", landingScene);
    }

    public void Skip2(string landingScene)
    {
        AuthManager.Instance.StartLogin("aloysiusgohkw@gmail.com", "aaaaaa", landingScene);
    }
}
