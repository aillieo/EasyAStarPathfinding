using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AillieoUtils.UI
{
    public class SimpleUILine : MaskableGraphic, ILayoutIgnorer
    {
        public float width = 2;
        private List<Vector2> points = new List<Vector2>();

        public Vector2[] GetAllPoints()
        {
            return points.ToArray();
        }

        public void GetAllPoints(List<Vector2> toFill)
        {
            toFill.Clear();
            toFill.AddRange(points);
        }

        public void AddPoint(Vector2 point)
        {
            points.Add(point);
            SetVerticesDirty();
        }

        public void AddPoints(IEnumerable<Vector2> points)
        {
            this.points.AddRange(points);
            SetVerticesDirty();
        }

        public void RemovePoint(int index)
        {
            points.RemoveAt(index);
            SetVerticesDirty();
        }

        public void RemoveAllPoints()
        {
            points.Clear();
            SetVerticesDirty();
        }

        public int GetPointCount()
        {
            return points.Count;
        }

        private List<UIVertex> buffer = new List<UIVertex>();
        public bool ignoreLayout => true;

        public bool drawConnection = true;
        public bool drawBody = true;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points == null || points.Count < 2)
            {
                return;
            }

            Rect rect = rectTransform.rect;
            Vector2 pivot = rectTransform.pivot;

            float baseX = -pivot.x * rect.width;
            float baseY = -pivot.y * rect.height;
            float sizeX = rect.width;
            float sizeY = rect.height;
            float halfWidth = width / 2;

            buffer.Clear();

            Vector2 prevV1 = Vector2.zero;
            Vector2 prevV2 = Vector2.zero;

            for (int i = 1; i < points.Count; i++)
            {
                Vector2 first = points[i - 1];
                Vector2 second = points[i];
                first = new Vector2(first.x * sizeX + baseX, first.y * sizeY + baseY);
                second = new Vector2(second.x * sizeX + baseX, second.y * sizeY + baseY);

                float angle = Mathf.Atan2(second.y - first.y, second.x - first.x);

                Vector2 v1 = first + new Vector2(0, -halfWidth);
                Vector2 v2 = first + new Vector2(0, halfWidth);
                Vector2 v3 = second + new Vector2(0, halfWidth);
                Vector2 v4 = second + new Vector2(0, -halfWidth);

                v1 = RotateAround(v1, first, angle);
                v2 = RotateAround(v2, first, angle);
                v3 = RotateAround(v3, second, angle);
                v4 = RotateAround(v4, second, angle);

                if (drawConnection && i > 1)
                {
                    AppendQuad(prevV1, v1, prevV2, v2);
                }

                if (drawBody)
                {
                    AppendQuad(v1, v2, v3, v4);
                }

                prevV1 = v3;
                prevV2 = v4;
            }

            vh.AddUIVertexTriangleStream(buffer);
        }

        protected void AppendQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            UIVertex v0 = UIVertex.simpleVert;
            v0.position = p0;
            v0.color = this.color;
            UIVertex v1 = UIVertex.simpleVert;
            v1.position = p1;
            v1.color = this.color;
            UIVertex v2 = UIVertex.simpleVert;
            v2.position = p2;
            v2.color = this.color;
            UIVertex v3 = UIVertex.simpleVert;
            v3.position = p3;
            v3.color = this.color;

            buffer.Add(v0);
            buffer.Add(v1);
            buffer.Add(v2);

            buffer.Add(v0);
            buffer.Add(v2);
            buffer.Add(v3);
        }

        private static Vector2 RotateAround(Vector2 vector2, Vector2 pivot, float angle)
        {
            Vector2 dir = vector2 - pivot;
            dir = Rotate(dir, angle);
            vector2 = dir + pivot;
            return vector2;
        }

        private static Vector2 Rotate(Vector2 vector2, float angle)
        {
            // [[ cos -sin ]
            //  [ sin  cos ]]
            return new Vector2(
                    vector2.x * Mathf.Cos(angle) - vector2.y * Mathf.Sin(angle),
                    vector2.x * Mathf.Sin(angle) + vector2.y * Mathf.Cos(angle));
        }
    }
}
