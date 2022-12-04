using NoteEditor.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace NoteEditor.GLDrawing
{
    public class GLLineDrawer : SingletonMonoBehaviour<GLLineDrawer>
    {
        [SerializeField]
        Material mat = default;
        List<Line> drawData = new List<Line>();

        int size = 0;
        int maxSize = 0;

        void OnRenderObject()
        {
            DrawQuad();
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);

            if (size * 2 < maxSize)
            {
                drawData.RemoveRange(size, maxSize - size);
                maxSize = size;
            }

            for (int i = 0; i < size; i++)
            {
                var line = drawData[i];
                GL.Color(line.color);
                GL.Vertex(line.start);
                GL.Vertex(line.end);
            }

            GL.End();
            GL.PopMatrix();
            size = 0;
        }

        void DrawQuad()//加粗線的厚度 避免解析度太高就看不到了
        {
            //GL.Flush();
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);

            //if (size * 2 < maxSize)
            //{
            //    drawData.RemoveRange(size, maxSize - size);
            //    maxSize = size;
            //}
            float width = 1.5f;
            for (int i = 0; i < size; i++)
            {
                GL.Color(drawData[i].color);
                var line = drawData[i];

                for (int j = 0; j < 4; j++)
                {
                    var start = new Vector3(line.start.x - width, line.start.y, line.start.z);
                    var end = new Vector3(line.end.x - width, line.end.y, line.end.z);
                    var start2 = new Vector3(line.start.x + width, line.start.y, line.start.z);
                    var end2 = new Vector3(line.end.x + width, line.end.y, line.end.z);

                    GL.Vertex(end);
                    GL.Vertex(start);
                    GL.Vertex(start2);
                    GL.Vertex(end2);
                }


            }

            GL.End();
            GL.PopMatrix();
            //size = 0;
        }

        public static void Draw(Line[] lines)
        {
            foreach (var line in lines)
            {
                Draw(line);
            }
        }

        public static void Draw(Line line)
        {
            if (Instance.size < Instance.maxSize)
            {
                Instance.drawData[Instance.size] = line;
            }
            else
            {
                Instance.drawData.Add(line);
                Instance.maxSize++;
            }

            Instance.size++;
        }
    }
}
