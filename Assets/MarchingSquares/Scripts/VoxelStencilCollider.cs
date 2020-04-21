using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace MarchingSquares
{
    // TODO: Get rid of GC allocations
    public class VoxelStencilCollider : MonoBehaviour, IStencil 
    {
        public Collider2D _collider;
        private Transform gridTrans;
        LayerMask mask;

        public bool fillType;
        private Action carveFinishedCallback;
        readonly RaycastHit2D[] results = new RaycastHit2D[1];

        private void Awake()
        {
            mask = 1 << gameObject.layer;
            if (!_collider) _collider = GetComponent<Collider2D>();
        }

        public  bool IsOverlapping(Voxel vox)
        {
            if (!_collider) return false;
            return _collider.OverlapPoint(vox.WorldPos);
        }

        public void CarveOnce(Action finishedCallback = null)
        {
            this.carveFinishedCallback = finishedCallback;
            StopCoroutine(Carve());
            StartCoroutine(Carve());
        }

        IEnumerator Carve()
        {
            var temp = _collider.enabled;
            _collider.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            _collider.enabled = temp;
            carveFinishedCallback?.Invoke();
            carveFinishedCallback = null;
        }
        
        public void SetGridTransform(Transform gridTransform)
        {
            this.gridTrans = gridTransform;
        }

        private Validated<EdgeData> TryFindValidEdge(Voxel a, Voxel b, bool horizontal)
        {
            var invalidResult = new Validated<EdgeData>();

            if (InDifferentStates(a, b)) return invalidResult;
            var forward = PickDirection(a, b, out Vector2 from, out Vector2 to);

            var hit = FindValidWorldHitPoint(from, to);
            if (!hit.IsValid) return invalidResult;

            var r = hit.Item;
            var localPoint = gridTrans.InverseTransformPoint(r.point);

            var offset = horizontal ? a.xEdge : a.yEdge;
            var newOffset = horizontal ? localPoint.x : localPoint.y;

            if (!EdgeValidToChange(offset, newOffset, forward)) return invalidResult;
            var edge = new EdgeData
            {
                EdgeOffset = newOffset,
                Normal = r.normal
            };
            return new Validated<EdgeData>(edge);
        }

        protected  void FindHorizontalCrossing(Voxel xMin, Voxel xMax)
        {
            FindAndApplyEdgeCrossing(xMin, xMax, true);
        }

        protected  void FindVerticalCrossing(Voxel yMin, Voxel yMax)
        {
            FindAndApplyEdgeCrossing(yMin, yMax, false);
        }
        
        private void FindAndApplyEdgeCrossing(Voxel a, Voxel b, bool horizontal)
        {
            var edge = TryFindValidEdge(a, b, horizontal);
            if (edge.IsValid) ApplyEdge(a, edge, horizontal);
        }

        private static void ApplyEdge(Voxel voxel, Validated<EdgeData> edge, bool horizontal)
        {
            if (horizontal) voxel.SetHorizontalEdge(edge.Item);
            else voxel.SetVerticalEdge(edge.Item);
        }

        private bool InDifferentStates(Voxel a, Voxel b)
        {
            return (IsApplied(a) && IsApplied(b)) ||
            (!IsApplied(a) && !IsApplied(b));
        }

        private bool IsApplied(Voxel a)
        {
            return a.state == IsFilling();
        }

        private bool EdgeValidToChange(float curEdgeOffset, float newEdgeOffset, bool forward)
        {
            if (curEdgeOffset != float.MinValue)
            {
                if (!forward && newEdgeOffset < curEdgeOffset) return false;
                if (forward && newEdgeOffset > curEdgeOffset) return false;
            }
            if (newEdgeOffset == float.MinValue) return false;

            return true;
        }

        private Validated<RaycastHit2D> FindValidWorldHitPoint(Vector2 a, Vector2 b)
        {
            // TODO: This raycasts againts any collider in mask, and is wrong when there are multiple stencils on the same layer
            var hit = Physics2D.LinecastNonAlloc(a, b, results, mask);
            var r = results[0];

            return new Validated<RaycastHit2D>(r, hit >= 1);
        }

        private bool PickDirection(Voxel min, Voxel max, out Vector2 from, out Vector2 to)
        {
            var ltr = max.state == IsFilling();
            from = ltr ? min.WorldPos : max.WorldPos;
            to = ltr ? max.WorldPos : min.WorldPos;
            return ltr;
        }

        public bool IsFilling()
        {
            return fillType;
        }

        public void SetHorizontalCrossing(Voxel a, Voxel b)
        {
            if (a.state != b.state)
            {
                FindHorizontalCrossing(a,b);
            }
            else
            {
                a.xEdge = float.MinValue;
            }
        }

        public void SetVerticalCrossing(Voxel a, Voxel b)
        {
            if (a.state != b.state)
            {
                FindVerticalCrossing(a,b);
            }
            else
            {
                a.yEdge = float.MinValue;
            }
        }

        private struct Validated<T>
        {
            private readonly T item;
            public bool IsValid { get; }

            public T Item {
                get {
                    if (!IsValid) throw new InvalidValueException();
                    else return item;
                }
            }

            public Validated(T item, bool isValid = true)
            {
                this.item = item;
                this.IsValid = isValid;
            }
        }
    }

    public interface IStencil
    {
        bool IsOverlapping(Voxel voxel);
        bool IsFilling();
        void SetHorizontalCrossing(Voxel a, Voxel b);
        void SetVerticalCrossing(Voxel a, Voxel voxel);
        void SetGridTransform(Transform transform);
    }

    internal class InvalidValueException : Exception { }
}
