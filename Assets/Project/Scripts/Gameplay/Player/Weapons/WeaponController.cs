using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LevelDesign.Async.Auth;

namespace LevelDesign.Systems.Player
{
    public enum Weapon { Mage, SwordShield, Unarmed }

    public class WeaponController : _InputAuth
    {
        public Weapon currentWeapon;

        [SerializeField] private RigInfo rigInfo;

        [SerializeField] private List<GameObject> mageItems;
        [SerializeField] private List<GameObject> swordShieldItems;

        [SerializeField] private _PlayerAnimation playerAnimation;

        public void OnEnable() {
            aInputInit(autoPopulateGame: true);
            InputAuthManager.Instance.RequestInput(this);
            
            if(rigInfo != null && playerAnimation == null) {
                playerAnimation = rigInfo.playerAnimation;
            }
        }
        
        public void Update() {
            ProcessInput();
        }
        
        public void ProcessInput() {
            if(!_inputAuthorized) { return; }
            if(_input.WeaponOne.WasPressedThisFrame()) {
                SetWeapon(Weapon.Mage);
            }
            if(_input.WeaponTwo.WasPressedThisFrame()) {
                SetWeapon(Weapon.SwordShield);
            }
            if(_input.WeaponThree.WasPressedThisFrame()) {
                SetWeapon(Weapon.Unarmed);
            }
        }

        public void SetWeapon(Weapon newWeapon) {
            currentWeapon = newWeapon;

            switch(newWeapon) {
                case Weapon.Mage:
                    ToggleObjects(mageItems, true);
                    ToggleObjects(swordShieldItems, false);

                    if(playerAnimation != null) { playerAnimation._SetWeapon(1); }
                    break; 
                case Weapon.SwordShield:
                    ToggleObjects(mageItems, false);
                    ToggleObjects(swordShieldItems, true);

                    if(playerAnimation != null) { playerAnimation._SetWeapon(2); }
                    break;
                case Weapon.Unarmed:
                    ToggleObjects(mageItems, false);
                    ToggleObjects(swordShieldItems, false);

                    if(playerAnimation != null) { playerAnimation._SetWeapon(0); }
                    break;
                default:
                    Debug.Log("Weapon was not recognized when settings a weapon.");
                    break;
            }
        }

        public void ToggleObjects(List<GameObject> itemsList, bool state) {
            foreach(GameObject item in itemsList) { item.SetActive(state); }
        }

        public void OnDestroy() {
            if(InputAuthManager.Instance != null) {
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }
    }
}
