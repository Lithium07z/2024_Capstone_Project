namespace Inventory.Scripts.Core.Items.Grids.Helper
{
    public static class GridValidatorHelper
    {
        public static bool BoundaryCheck(this GridTable gridTable, int posX, int posY, int width, int height)
        {
            if (!PositionCheck(posX, posY, gridTable.Width, gridTable.Height))
            {
                return false;
            }

            posX += width - 1;
            posY += height - 1;

            return PositionCheck(posX, posY, gridTable.Width, gridTable.Height);
        }

        private static bool PositionCheck(int posX, int posY, int gridWidth, int gridHeight)
        {
            if (posX < 0 || posY < 0)
            {
                return false;
            }

            return posX < gridWidth && posY < gridHeight;
        }
    }
}