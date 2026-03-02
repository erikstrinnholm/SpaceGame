using UnityEngine;

//Responsibilities: Anything that can drop loot implements this
//Implemented by Enemies, Asteroids, Structures etc.
public interface ILootSource {
    DropTable DropTable { get; }
    Transform DropOrigin { get; }
}
