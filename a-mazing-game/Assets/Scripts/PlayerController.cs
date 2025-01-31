﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Private Members
    
    private bool mCanTakeDamage = true;
    
    private InventoryItemBase mCurrentItem = null;

    #endregion

    #region Public Members

    public Inventory inventory;

    public GameObject Hand;

    public HUD Hud;

    public bool isDead;

    #endregion

    // Use this for initialization
    void Start()
    {
        isDead = GetComponent<PlayerCombat>().isDead;
        inventory.ItemUsed += Inventory_ItemUsed;
        inventory.ItemRemoved += Inventory_ItemRemoved;
    }


    #region Inventory

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;

        if (item == mCurrentItem)
            mCurrentItem = null;

    }

    private void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }


    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        if (e.Item.ItemType != EItemType.Consumable)
        {
            // If the player carries an item, un-use it (remove from player's hand)
            if (mCurrentItem != null)
            {
                SetItemActive(mCurrentItem, false);
            }

            InventoryItemBase item = e.Item;

            // Use item (put it to hand of the player)
            SetItemActive(item, true);

            mCurrentItem = e.Item;
        }

    }

    // private int Attack_1_Hash = Animator.StringToHash("Base Layer.Attack_1");
    //
    // public bool IsAttacking
    // {
    //     get
    //     {
    //         AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    //         if (stateInfo.fullPathHash == Attack_1_Hash)
    //         {
    //             return true;
    //         }
    //         return false;
    //     }
    // }

    public void DropCurrentItem()
    {
        mCanTakeDamage = false;

        // _animator.SetTrigger("tr_drop");

        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        inventory.RemoveItem(mCurrentItem);

        // Throw animation
        Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
        if (rbItem != null)
        {
            rbItem.AddForce(transform.forward * 2.0f, ForceMode.Impulse);

            Invoke("DoDropItem", 0.25f);
        }
    }

    public void DropAndDestroyCurrentItem()
    {
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        inventory.RemoveItem(mCurrentItem);

        Destroy(goItem);

        mCurrentItem = null;
    }

    public void DoDropItem()
    {
        mCanTakeDamage = true;
        if (mCurrentItem != null)
        {
            // Remove Rigidbody
            Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());

            mCurrentItem = null;

            mCanTakeDamage = true;
        }
    }

    #endregion

    // #region Health & Hunger
    //
    // [Tooltip("Amount of health")]
    // public int Health = 100;
    //
    // [Tooltip("Amount of food")]
    // public int Food = 100;
    //
    // [Tooltip("Rate in seconds in which the hunger increases")]
    // public float HungerRate = 0.5f;
    //
    // public void IncreaseHunger()
    // {
    //     Food--;
    //     if (Food < 0)
    //         Food = 0;
    //
    //     mFoodBar.SetValue(Food);
    //
    //     if (Food == 0)
    //     {
    //         CancelInvoke();
    //         Die();
    //     }
    // }
    //
    // public bool IsDead
    // {
    //     get
    //     {
    //         return Health == 0 || Food == 0;
    //     }
    // }

    public bool CarriesItem(string itemName)
    {
        if (mCurrentItem == null)
            return false;

        return (mCurrentItem.Name == itemName);
    }

    public InventoryItemBase GetCurrentItem()
    {
        return mCurrentItem;
    }

    public bool IsArmed
    {
        get
        {
            if (mCurrentItem == null)
                return false;

            return mCurrentItem.ItemType == EItemType.Weapon;
        }
    }


    // public void Eat(int amount)
    // {
    //     Food += amount;
    //     if (Food > startFood)
    //     {
    //         Food = startFood;
    //     }
    //
    //     mFoodBar.SetValue(Food);
    //
    // }
    //
    // public void Rehab(int amount)
    // {
    //     Health += amount;
    //     if (Health > startHealth)
    //     {
    //         Health = startHealth;
    //     }
    //
    //     mHealthBar.SetValue(Health);
    // }
    //
    // public void TakeDamage(int amount)
    // {
    //     if (!mCanTakeDamage)
    //         return;
    //
    //     Health -= amount;
    //     if (Health < 0)
    //         Health = 0;
    //
    //     mHealthBar.SetValue(Health);
    //
    //     if (IsDead)
    //     {
    //         Die();
    //     }
    //
    // }
    //
    //
    // private void Die()
    // {
    //     _animator.SetTrigger("death");
    //
    //     if (PlayerDied != null)
    //     {
    //         PlayerDied(this, EventArgs.Empty);
    //     }
    // }
    //
    // #endregion
    //
    //
    // public void Talk()
    // {
    //     _animator.SetTrigger("tr_talk");
    // }
    //
    // private bool mIsControlEnabled = true;
    //
    // public void EnableControl()
    // {
    //     mIsControlEnabled = true;
    // }
    //
    // public void DisableControl()
    // {
    //     mIsControlEnabled = false;
    // }
    //
    // private Vector3 mExternalMovement = Vector3.zero;
    //
    // public Vector3 ExternalMovement
    // {
    //     set
    //     {
    //         mExternalMovement = value;
    //     }
    // }

    void FixedUpdate()
    {
        if (!isDead)
        {
            // Drop item
            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }
        }
    }

    // void LateUpdate()
    // {
    //     if (mExternalMovement != Vector3.zero)
    //     {
    //         _characterController.Move(mExternalMovement);
    //     }
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     if (!isDead) // && mIsControlEnabled)
    //     {
    //         // Interact with the item
    //         // if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
    //         // {
    //         //     // Interact animation
    //         //     mInteractItem.OnInteractAnimation(_animator);
    //         // }
    //
    //         // Execute action with item
    //         if (mCurrentItem != null && Input.GetMouseButtonDown(0))
    //         {
    //             // Dont execute click if mouse pointer is over uGUI element
    //             if (!EventSystem.current.IsPointerOverGameObject())
    //             {
    //                 // TODO: Logic which action to execute has to come from the particular item
    //                 _animator.SetTrigger("attack_1");
    //             }
    //         }
    //
    //         // Get Input for axis
    //         float h = Input.GetAxis("Horizontal");
    //         float v = Input.GetAxis("Vertical");
    //
    //         // Calculate the forward vector
    //         Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
    //         Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;
    //
    //         if (move.magnitude > 1f) move.Normalize();
    //
    //
    //         // Calculate the rotation for the player
    //         move = transform.InverseTransformDirection(move);
    //
    //         // Get Euler angles
    //         float turnAmount = Mathf.Atan2(move.x, move.z);
    //
    //         transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);
    //
    //         if (_characterController.isGrounded || mExternalMovement != Vector3.zero)
    //         {
    //             _moveDirection = transform.forward * move.magnitude;
    //
    //             _moveDirection *= Speed;
    //
    //             if (Input.GetButton("Jump"))
    //             {
    //                 _animator.SetBool("is_in_air", true);
    //                 _moveDirection.y = JumpSpeed;
    //
    //             }
    //             else
    //             {
    //                 _animator.SetBool("is_in_air", false);
    //                 _animator.SetBool("run", move.magnitude > 0);
    //             }
    //         }
    //         else
    //         {
    //             Gravity = 20.0f;
    //         }
    //
    //
    //         _moveDirection.y -= Gravity * Time.deltaTime;
    //
    //         _characterController.Move(_moveDirection * Time.deltaTime);
    //     }
    // }

    public void InteractWithItem()
    {
        if (mInteractItem != null)
        {
            mInteractItem.OnInteract();

            if (mInteractItem is InventoryItemBase)
            {
                InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
                inventory.AddItem(inventoryItem);
                inventoryItem.OnPickup();

                if (inventoryItem.UseItemAfterPickup)
                {
                    inventory.UseItem(inventoryItem);
                }
                Hud.CloseMessagePanel();
                mInteractItem = null;
            }
            //else
            //{
            //    if (mInteractItem.ContinueInteract())
            //    {
            //        Hud.OpenMessagePanel(mInteractItem);
            //    }
            //    else
            //    {
            //        Hud.CloseMessagePanel();
            //        mInteractItem = null;
            //    }
            //}
        }
    }

    private InteractableItemBase mInteractItem = null;

    private void OnTriggerEnter(Collider other)
    {
        TryInteraction(other);
    }

    private void TryInteraction(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        if (item != null)
        {
            if (item.CanInteract(other))
            {
                mInteractItem = item;

                Hud.OpenMessagePanel(mInteractItem);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();
        if (item != null)
        {
            Hud.CloseMessagePanel();
            mInteractItem = null;
        }
    }
}
