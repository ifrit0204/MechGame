// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MechPlayerData : MonoBehaviour
// {
//     public int credits = 50000;

//     public List<MechPartData> ownedParts = new List<MechPartData>();

//     public bool IsOwned(MechPartData part)
//     {
//         return ownedParts.Contains(part);
//     }

//     public bool BuyPart(MechPartData part, int price)
//     {
//         if (part == null)
//             return false;

//         if (ownedParts.Contains(part))
//             return false;

//         if (credits < price)
//             return false;

//         credits -= price;

//         ownedParts.Add(part);

//         return true;
//     }
// }
