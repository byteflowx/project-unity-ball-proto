using System.Linq;
using App.Scripts.Game;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace App.Scripts.Tests.Game
{
    public sealed class LevelBalanceTests
    {
        private const string CONFIG_PATH = "Assets/App/Addressables/Levels/LevelConfig.asset";
        private const float REQUIRED_MARGIN = 0.2f;

        private LevelConfig _config;

        [SetUp]
        public void SetUp() => _config = AssetDatabase.LoadAssetAtPath<LevelConfig>(CONFIG_PATH);

        [Test]
        public void StartSizeCoversTheLevelWithTwentyPercentToSpare()
        {
            float spent = SolveGreedily(_config);
            float budget = _config.StartMass - _config.CriticalMass;

            TestContext.WriteLine($"spent {spent:F3} of {budget:F3}, margin {budget / spent - 1f:P0}");

            Assert.That(budget, Is.GreaterThanOrEqualTo(spent * (1f + REQUIRED_MARGIN)));
        }

        private static float SolveGreedily(LevelConfig config)
        {
            Level level = new(config);
            float mass = config.StartMass;
            float along = 0f;
            float spent = 0f;

            while (true)
            {
                float radius = Mathf.Sqrt(mass);
                along = level.StopDistance(along, radius);
                int blocker = level.FirstBlocker(along, radius);

                if (blocker < 0)
                    return spent;

                (float shotMass, int hit) best = (0f, -1);
                float bestScore = -1f;

                for (float shotMass = config.MinShotMass; shotMass < 0.4f; shotMass += 0.01f)
                {
                    float shotRadius = Mathf.Sqrt(shotMass);
                    Vector2 from = level.At(along) + level.Direction * (radius + shotRadius);
                    int hit = level.FirstHit(from, level.Point(blocker), shotRadius);

                    level.Spread(hit, config.Power(shotMass));

                    float score = level.Reach.Keys.Count(i => level.Blocks(i, along, radius)) / shotMass;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = (shotMass, hit);
                    }
                }

                level.Spread(best.hit, config.Power(best.shotMass));
                level.KillReached();

                mass -= best.shotMass;
                spent += best.shotMass;
            }
        }
    }
}
