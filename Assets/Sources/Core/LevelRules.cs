using System.Collections.Generic;
using ShrinkShot.Config;
using UnityEngine;

namespace ShrinkShot.Core
{
    public readonly struct Infection
    {
        public readonly int Index;
        /// <summary>Infection energy spent to reach this obstacle; drives the visual spread delay.</summary>
        public readonly float Distance;

        public Infection(int index, float distance)
        {
            Index = index;
            Distance = distance;
        }
    }

    /// <summary>
    /// Pure gameplay rules (blocking, shot hits, infection spread) on the obstacle positions of a level,
    /// kept apart from MonoBehaviours so they can be reasoned about and tested on their own.
    /// </summary>
    public sealed class LevelRules
    {
        private readonly GameConfig _config;
        private readonly Vector2[] _obstacles;
        private readonly List<int>[] _neighbours;

        public LevelRules(GameConfig config, Vector2[] obstacles, float pathLength)
        {
            _config = config;
            _obstacles = obstacles;
            PathLength = pathLength;
            _neighbours = BuildNeighbours(obstacles, config.InfectionMaxHop);
        }

        public float PathLength { get; }
        public int ObstacleCount => _obstacles.Length;
        public Vector2 ObstaclePosition(int index) => _obstacles[index];

        public float InfectionEnergy(float shotVolume) =>
            _config.InfectionScale * Mathf.Pow(shotVolume, _config.InfectionVolumePower);

        /// <summary>Nearest alive obstacle in front of the ball that doesn't let it pass along the path centre.</summary>
        public int FindFirstBlocker(bool[] alive, float playerZ, float playerRadius)
        {
            float halfWidth = playerRadius + _config.ObstacleRadius + _config.PassMargin;
            int best = -1;
            float bestZ = float.MaxValue;

            for (int i = 0; i < _obstacles.Length; i++)
            {
                if (!alive[i])
                    continue;

                Vector2 p = _obstacles[i];
                if (p.y <= playerZ || p.y >= PathLength || p.y >= bestZ || Mathf.Abs(p.x) >= halfWidth)
                    continue;

                best = i;
                bestZ = p.y;
            }

            return best;
        }

        /// <summary>
        /// The shot flies straight to the door along the path centre (x = 0) and hits the first alive
        /// obstacle it touches. Returns -1 if it reaches the door; <paramref name="contactZ"/> is where the shot stops.
        /// </summary>
        public int FindShotHit(bool[] alive, float fromZ, float shotRadius, out float contactZ)
        {
            float reach = shotRadius + _config.ObstacleRadius;
            int best = -1;
            contactZ = PathLength;

            for (int i = 0; i < _obstacles.Length; i++)
            {
                if (!alive[i])
                    continue;

                Vector2 p = _obstacles[i];
                if (p.y <= fromZ || p.y >= PathLength || Mathf.Abs(p.x) >= reach)
                    continue;

                // Shot centre position on the line when it first touches this obstacle.
                float z = p.y - Mathf.Sqrt(reach * reach - p.x * p.x);
                if (z >= contactZ)
                    continue;

                best = i;
                contactZ = z;
            }

            return best;
        }

        /// <summary>Where the ball stops in front of the given blocker (never moves backwards).</summary>
        public float StopPosition(int blocker, float playerZ, float playerRadius)
        {
            float z = _obstacles[blocker].y - _config.ObstacleRadius - playerRadius - _config.StopGap;
            return Mathf.Max(playerZ, z);
        }

        /// <summary>
        /// Spreads infection from the hit obstacle. Hop cost grows with the squared distance,
        /// so dense clusters burn through cheaply while lonely obstacles stop the chain.
        /// The hit obstacle is always infected.
        /// </summary>
        public List<Infection> Infect(bool[] alive, int hit, float energy)
        {
            var result = new List<Infection>();

            // Energy left on arrival at each obstacle; -1 = not reached yet.
            var left = new float[_obstacles.Length];
            for (int i = 0; i < left.Length; i++)
                left[i] = -1f;

            var frontier = new List<int> { hit };
            left[hit] = energy;

            while (frontier.Count > 0)
            {
                int pick = 0;
                for (int i = 1; i < frontier.Count; i++)
                {
                    if (left[frontier[i]] > left[frontier[pick]])
                        pick = i;
                }

                int current = frontier[pick];
                frontier.RemoveAt(pick);
                result.Add(new Infection(current, energy - left[current]));

                foreach (int next in _neighbours[current])
                {
                    if (!alive[next])
                        continue;

                    float d = Vector2.Distance(_obstacles[current], _obstacles[next]);
                    float arrived = left[current] - d * d / _config.InfectionHopReference;
                    if (arrived <= 0f || arrived <= left[next])
                        continue;

                    if (left[next] < 0f)
                        frontier.Add(next);

                    left[next] = arrived;
                }
            }

            foreach (Infection infection in result)
                alive[infection.Index] = false;

            return result;
        }

        private static List<int>[] BuildNeighbours(Vector2[] points, float maxDistance)
        {
            var neighbours = new List<int>[points.Length];
            for (int i = 0; i < points.Length; i++)
                neighbours[i] = new List<int>(8);

            float maxSqr = maxDistance * maxDistance;
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    if ((points[i] - points[j]).sqrMagnitude > maxSqr)
                        continue;

                    neighbours[i].Add(j);
                    neighbours[j].Add(i);
                }
            }

            return neighbours;
        }
    }
}
