using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public void SelectWeapon(int weaponIndex)
    {
        int i = 0;
        
        //take all the transforms that are children to this game object
        foreach (Transform weapon in transform)
        {
            if (i == weaponIndex)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
