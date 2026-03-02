
/// <summary>
/// Helper class for global enums and extension methods related to them.
/// </summary>
public static class GlobalEnums {
    public static string ToKey(this AudioGroups group) {
        return group switch {
            AudioGroups.Master   => "MasterVol",
            AudioGroups.Music    => "MusicVol",
            AudioGroups.UI       => "UIVol",
            AudioGroups.SFX      => "SFXVol",
            AudioGroups.Dialogue => "DialogueVol",
            _ => group.ToString()
        };        
    }
}


// Input System
public enum ActionMapType { Universal, Menu, Inventory, Ship, Character, Unknown }

// Audio System
public enum AudioGroups {Master, Music, UI, SFX, Dialogue}

//////////////////////////////////////////////////////////////////////////



// DAMAGE
public enum DamageType {Energy, ION, Kinetic, Impact, Explosive}
public enum TargetType {Shield, Normal, Armored}



//Inventory & Equipment
public enum Rarity {Common, Uncommon, Rare, Epic, Legendary}




public enum ItemType {
    None,
    Consumable,
    SystemResource,
    QuestItem,
    KeyItem,
    UpgradeMaterial
}

//Weapons 
public enum LockOnMode {None, Optional, Required}
public enum DroneRole {Attack, Defense, Repair, Utility}


//Indicators
public enum LockState {Locking, Locked, Incoming}   //MissileLockIndicator
public enum DamageLayer {Shield, Hull}              //DamageIndicator
public enum ThreatLevel {Normal, Elite, Boss}       //EnemyPresenceIndicator










