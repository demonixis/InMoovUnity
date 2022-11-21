using UnityEngine;

namespace Demonixis.InMoov
{
    public sealed class PawnContextPanel : MonoBehaviour
    {
        public void SwitchPawnContext()
        {
            var rigs = FindObjectsOfType<XRRig>();

            if (rigs.Length != 2)
            {
                Debug.LogError("Only 2 rigs, one robot, one external player are supported for now.");
                return;
            }

            var rig0Active = rigs[0].IsActive;
            rigs[0].SetActive(!rig0Active);
            rigs[1].SetActive(rig0Active);
        }
    }
}
