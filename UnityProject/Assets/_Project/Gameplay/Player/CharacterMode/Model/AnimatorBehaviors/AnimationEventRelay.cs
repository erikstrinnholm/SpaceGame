using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRelay : MonoBehaviour {
    [SerializeField] private Rifle rifle;
    [SerializeField] private Wrench wrench;
    [SerializeField] private Unarmed unarmed;

    private CharacterCombatController combat;


    private void Awake() {
        combat = GetComponentInParent<CharacterCombatController>();
    }




    // ================= ANIMATION EVENTS (CharacterCombatController) =================
    public void Anim_ReloadAudio()              => combat?.OnReloadAudio();
    public void Anim_UnequipFinished()          => combat?.OnUnequipAnimationFinished();
    public void Anim_EquipMiddle()              => combat?.OnEquipAnimationMiddle();
    public void Anim_EquipFinished()            => combat?.OnEquipAnimationFinished();
    
    // ================= ANIMATION EVENTS (Wrench) =====================================
    public void Anim_WrenchDealLightDamage()    => wrench?.DealLightDamage();
    public void Anim_WrenchDealHeavyDamage()    => wrench?.DealHeavyDamage();
    public void Anim_WrenchEndAttack()          => wrench?.EndAttack();
    public void Anim_WrenchLightSwingSound()    => wrench?.PlayLightSwingSound();
    public void Anim_WrenchHeavySwingSound()    => wrench?.PlayHeavySwingSound();

    // ================= ANIMATION EVENTS (Wrench) =====================================
    public void Anim_UnarmedDealDamage()        => unarmed?.DealDamage();
    public void Anim_UnarmedEndAttack()         => unarmed?.EndAttack();
    public void Anim_UnarmedSwingSound()        => unarmed?.PlaySwingSound();
}

