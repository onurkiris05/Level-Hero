using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VP.Nest.Haptic;

public class Collectable : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      Player player = other.GetComponentInParent<Player>();

      if (player.isActive)
      {
         HapticManager.Haptic(HapticType.SoftImpact);
         player.IncreaseSizeAndLevel(1);
         player.PlayCollectableParticle();
         gameObject.SetActive(false);
      }
   }
}
