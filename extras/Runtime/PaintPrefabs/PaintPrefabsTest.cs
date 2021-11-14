using System.Collections.Generic;
using UnityEngine;

namespace Needle.Timeline
{
    public class PaintPrefabsTest : MonoBehaviour, IAnimated
    {
        // [Animate] public List<Vector3> Points;

        [Animate]
        private List<GameObject> Prefabs;

    }
}
