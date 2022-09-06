using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public GameObject[] weapons;
    public GameObject[] grenades;
    public bool[] hasWeapons;
    public int coin;
    public int health;
    public int hasGrenades;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    float hAxis;
    float vAxis;
    bool wDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool jDown;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Rigidbody rigid;

    Vector3 moveVec;

    Animator anim;
    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   

        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interaction();
        Swap();

    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * (wDown?0.4f:1f) * Time.deltaTime;
        if(isSwap)
            moveVec = Vector3.zero;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

        }



    }
    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);

        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void Interaction()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }
        }
    }

     void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex==0))
        return;
        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex==1))
        return;
        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex==2))
        return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.SetActive(false);
                
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
         anim.SetBool("isJump", false);
         isJump = false;

        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Item"){
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;           
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                    hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }

    }

    //플레이어의 무기 접근 인식
    void OnTriggerStay(Collider other)
    {

        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;

            Debug.Log(nearObject.gameObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }

}