﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Default,
    Consumable,
    Weapon,
}

public class InteractableItemBase : MonoBehaviour
{
    public string Name;

    public Sprite Image;

    public string InteractText = "Press F to pickup";

    public EItemType ItemType;

    public virtual void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("tr_pickup");
    }

    public virtual void OnInteract()
    {
    }

    public virtual bool CanInteract(Collider other)
    {
        return true;   
    }
}

public class InventoryItemBase : InteractableItemBase
{
    public MazeConstructor mz;
    public bool pickedUp = false;
    public InventorySlot Slot
    {
        get; set;
    }

    public virtual void OnUse()
    {
        transform.localPosition = PickPosition;
        transform.localEulerAngles = PickRotation;
    }

    public virtual void OnDrop()
    {
        gameObject.SetActive(true);
        gameObject.transform.rotation = Quaternion.Euler(DropRotation);
    }

    public virtual void OnPickup()
    {
        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<BoxCollider>());
        gameObject.SetActive(false);
    }

    public Vector3 PickPosition;

    public Vector3 PickRotation;

    public Vector3 DropRotation;

    public bool UseItemAfterPickup = false;


}
