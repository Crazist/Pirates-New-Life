using UnityEngine;

namespace GameInit.Optimization
{
    public class ProjectileMotionCalculator
    {

        private const float GRAVITY = 9.81f; // стандартное ускорение свободного падения в юнити
        private const int POINTS_COUNT = 20; // количество точек на параболе

        // Рассчитываем параболу между двумя точками
        public Vector3[] CalculateParabola(Vector3 start, Vector3 end)
        {
            float distance = Vector3.Distance(start, end);
            float maxHeight = end.y + (distance / 2f) * Mathf.Tan(Mathf.PI / 4f);

            Vector3[] points = new Vector3[POINTS_COUNT];

            for (int i = 0; i < POINTS_COUNT; i++)
            {
                float t = (float)i / (float)(POINTS_COUNT - 1);
                Vector3 point = Vector3.Lerp(start, end, t);

                float height = maxHeight * (1f - 4f * (t - 0.5f) * (t - 0.5f));
                point.y += height;

                float time = Mathf.Sqrt(2f * height / GRAVITY);
                float speed = 0;

                // Рассчитываем скорость только в том случае, если начальная и конечная точки не находятся на одной высоте
                if (start.y != end.y)
                {
                    float heightDiff = end.y - point.y;
                    speed = distance / (time * Mathf.Sqrt(2f / (heightDiff / distance + 1f)));
                }

                Vector3 direction = end - start;
                direction.y = 0f;
                direction.Normalize();

                point += direction * speed * t;

                points[i] = point;
            }

            return points;
        }
    }
}
