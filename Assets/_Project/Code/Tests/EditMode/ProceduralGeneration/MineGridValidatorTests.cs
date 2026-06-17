using DeepSeal.Core;
using DeepSeal.Mining;
using DeepSeal.ProceduralGeneration;
using NUnit.Framework;

namespace DeepSeal.Tests.ProceduralGeneration
{
    public sealed class MineGridValidatorTests
    {
        [Test]
        public void Validate_ReturnsValidForGeneratedGrid()
        {
            MineGenerationSettings settings = CreateSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.None));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidForNullGrid()
        {
            MineGridValidationResult validationResult = MineGridValidator.Validate(null, CreateSettings());

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.GridIsNull));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidForDefaultSettings()
        {
            var grid = new MineGrid(3, 3, TerrainCell.Wall(1));

            MineGridValidationResult validationResult = MineGridValidator.Validate(
                grid,
                default(MineGenerationSettings));

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.InvalidSettings));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidForSizeMismatch()
        {
            MineGenerationSettings settings = CreateSettings();
            var grid = new MineGrid(settings.Width + 1, settings.Height, TerrainCell.Wall(3));

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.SizeMismatch));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidWhenBoundaryIsNotWall()
        {
            MineGenerationSettings settings = CreateSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);
            var brokenPosition = new GridPosition(0, 2);

            generationResult.Grid.TrySetCell(brokenPosition, TerrainCell.Floor);

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.BoundaryNotWall));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(brokenPosition));
        }

        [Test]
        public void Validate_ReturnsInvalidWhenStartAreaIsBlocked()
        {
            MineGenerationSettings settings = CreateSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            generationResult.Grid.TrySetCell(settings.StartPosition, TerrainCell.Wall(3));

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.StartAreaBlocked));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(settings.StartPosition));
        }

        private static MineGenerationSettings CreateSettings()
        {
            return new MineGenerationSettings(
                7,
                7,
                1234,
                new GridPosition(3, 3),
                1,
                3,
                35);
        }
    }
}