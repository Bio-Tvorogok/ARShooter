using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    [SerializeField] private WeaponFire weapon;

    public void onFireClick() {
        Debug.Log("fire");

        weapon.Fire();
    }
}
