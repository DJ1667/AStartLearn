using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Test3
{
    [HelpURL("http://arongranberg.com/astar/docs/class_partial1_1_1_modifier_tutorial.php")]
    public class MyPathModifier : MonoModifier
    {
        [Header("平滑优化次数")] public int iterations = 5;
        [Header("每段路径的细分次数")] public int subdivision = 2;

        public override int Order
        {
            get => 60;
        }

        public override void Apply(Path path)
        {
            if (path.error || path.vectorPath == null || path.vectorPath.Count <= 2)
                return;

            if (subdivision > 12)
            {
                Debug.LogError("细分超过12次会造成性能问题，不建议这样修改");
                subdivision = 12;
                return;
            }

            //使用缓存池提高内存效率  降低GC压力
            List<Vector3> newPath = Pathfinding.Util.ListPool<Vector3>.Claim();
            List<Vector3> originalPath = path.vectorPath;

            //计算一段路径将被细分成多少段
            int subSegments = (int) Mathf.Pow(2, subdivision);
            //用于插值
            float fractionPerSegment = 1f / subSegments;

            for (int i = 0; i < originalPath.Count - 1; i++)
            {
                for (int j = 0; j < subSegments; j++)
                {
                    newPath.Add(Vector3.Lerp(originalPath[i], originalPath[i + 1], j * fractionPerSegment));
                }
            }

            newPath.Add(originalPath[originalPath.Count - 1]);

            //开始优化新路径 使其平滑
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 1; j < newPath.Count - 1; j++)
                {
                    //除头尾的点外  每个点修改为自身加上前后点取平均值
                    var newPoint = (newPath[j] + newPath[j + 1] + newPath[j - 1]) / 3f;
                    newPath[j] = newPoint;
                }
            }

            path.vectorPath = newPath;
            Pathfinding.Util.ListPool<Vector3>.Release(originalPath);
        }
    }
}