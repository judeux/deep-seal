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
            MineGenerationSettings settings = CreateConnectedSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.None));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidForNullGrid()
        {
            MineGridValidationResult validationResult = MineGridValidator.Validate(null, CreateConnectedSettings());

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
            MineGenerationSettings settings = CreateConnectedSettings();
            var grid = new MineGrid(settings.Width + 1, settings.Height, TerrainCell.Wall(3));

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.SizeMismatch));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidWhenRandomScatterBoundaryIsNotWall()
        {
            MineGenerationSettings settings = CreateRandomScatterSettings();
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
            MineGenerationSettings settings = CreateConnectedSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            generationResult.Grid.TrySetCell(settings.StartPosition, TerrainCell.Wall(3));

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.StartAreaBlocked));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(settings.StartPosition));
        }

        [Test]
        public void Validate_ReturnsInvalidWhenConnectedCavernHasTooFewPassableCells()
        {
            var settings = new MineGenerationSettings(
                11,
                11,
                123,
                new GridPosition(5, 5),
                0,
                3,
                40,
                MineGenerationShapeMode.ConnectedCavern);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);
            grid.TrySetCell(settings.StartPosition, TerrainCell.Floor);

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.InsufficientPassableCells));
            Assert.That(validationResult.HasPosition, Is.False);
        }

        [Test]
        public void Validate_ReturnsInvalidWhenConnectedCavernHasDisconnectedPassableArea()
        {
            var settings = new MineGenerationSettings(
                7,
                7,
                123,
                new GridPosition(3, 3),
                0,
                3,
                0,
                MineGenerationShapeMode.ConnectedCavern);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);
            var disconnectedPosition = new GridPosition(1, 1);

            grid.TrySetCell(settings.StartPosition, TerrainCell.Floor);
            grid.TrySetCell(disconnectedPosition, TerrainCell.Floor);

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.DisconnectedPassableArea));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(disconnectedPosition));
        }

        [Test]
        public void Validate_AllowsDisconnectedPassableAreaForRandomScatter()
        {
            var settings = new MineGenerationSettings(
                7,
                7,
                123,
                new GridPosition(3, 3),
                0,
                3,
                0,
                MineGenerationShapeMode.RandomScatter);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Wall(settings.WallDurability));
            SetRectangularBoundaryWalls(grid);

            grid.TrySetCell(settings.StartPosition, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(1, 1), TerrainCell.Floor);

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.None));
        }

        [Test]
        public void Validate_ReturnsInvalidWhenConnectedCavernOuterFrameIsNotVoid()
        {
            MineGenerationSettings settings = CreateConnectedSettings();
            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);
            var brokenPosition = new GridPosition(0, 2);

            generationResult.Grid.TrySetCell(brokenPosition, TerrainCell.Wall(settings.WallDurability));

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.OuterFrameNotVoid));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(brokenPosition));
        }

        [Test]
        public void Validate_ReturnsInvalidWhenPassableCellTouchesVoid()
        {
            var settings = new MineGenerationSettings(
                7,
                7,
                123,
                new GridPosition(3, 3),
                0,
                3,
                0,
                MineGenerationShapeMode.ConnectedCavern);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);
            grid.TrySetCell(settings.StartPosition, TerrainCell.Floor);

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.PassableAreaTouchesVoid));
            Assert.That(validationResult.HasPosition, Is.True);
            Assert.That(validationResult.Position, Is.EqualTo(settings.StartPosition));
        }

        [Test]
        public void Validate_ReturnsValidForConnectedCavernWithInternalWalls()
        {
            var settings = new MineGenerationSettings(
                24,
                18,
                800,
                new GridPosition(12, 9),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern,
                10);

            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.None));
        }

        [Test]
        public void Validate_ReturnsValidForConnectedCavernWithUnmineableInternalWalls()
        {
            var settings = new MineGenerationSettings(
                24,
                18,
                801,
                new GridPosition(12, 9),
                1,
                3,
                45,
                MineGenerationShapeMode.ConnectedCavern,
                12,
                50);

            MineGenerationResult generationResult = MineGridGenerator.Generate(settings);

            MineGridValidationResult validationResult = MineGridValidator.Validate(generationResult);

            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.None));
        }

        private static MineGenerationSettings CreateConnectedSettings()
        {
            return new MineGenerationSettings(
                9,
                9,
                1234,
                new GridPosition(4, 4),
                1,
                3,
                35,
                MineGenerationShapeMode.ConnectedCavern);
        }

        private static MineGenerationSettings CreateRandomScatterSettings()
        {
            return new MineGenerationSettings(
                7,
                7,
                1234,
                new GridPosition(3, 3),
                1,
                3,
                35,
                MineGenerationShapeMode.RandomScatter);
        }

        private static void SetRectangularBoundaryWalls(MineGrid grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                grid.TrySetCell(new GridPosition(x, 0), TerrainCell.BoundaryWall);
                grid.TrySetCell(new GridPosition(x, grid.Height - 1), TerrainCell.BoundaryWall);
            }

            for (int y = 1; y < grid.Height - 1; y++)
            {
                grid.TrySetCell(new GridPosition(0, y), TerrainCell.BoundaryWall);
                grid.TrySetCell(new GridPosition(grid.Width - 1, y), TerrainCell.BoundaryWall);
            }
        }

        [Test]
        public void Validate_ReturnsInvalidWhenNonBoundaryTerrainTouchesVoid()
        {
            var settings = new MineGenerationSettings(
                9,
                9,
                900,
                new GridPosition(4, 4),
                0,
                3,
                0,
                MineGenerationShapeMode.ConnectedCavern,
                0,
                0,
                1);

            var grid = new MineGrid(settings.Width, settings.Height, TerrainCell.Void);

            grid.TrySetCell(settings.StartPosition, TerrainCell.Floor);
            grid.TrySetCell(new GridPosition(4, 5), TerrainCell.MineableWall(settings.WallDurability));
            grid.TrySetCell(new GridPosition(5, 4), TerrainCell.MineableWall(settings.WallDurability));
            grid.TrySetCell(new GridPosition(4, 3), TerrainCell.MineableWall(settings.WallDurability));
            grid.TrySetCell(new GridPosition(3, 4), TerrainCell.MineableWall(settings.WallDurability));

            MineGridValidationResult validationResult = MineGridValidator.Validate(grid, settings);

            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Issue, Is.EqualTo(MineGridValidationIssue.NonBoundaryTerrainTouchesVoid));
            Assert.That(validationResult.HasPosition, Is.True);
        }
    }
}