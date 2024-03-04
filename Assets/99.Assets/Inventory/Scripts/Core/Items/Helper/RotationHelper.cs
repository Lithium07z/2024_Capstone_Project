using Inventory.Scripts.Core.Items.Enums;

namespace Inventory.Scripts.Core.Items.Helper
{
    public static class RotationHelper
    {
        public static float GetRotationByType(Rotation rotation)
        {
            if (rotation == Rotation.MinusNinety)
            {
                return -90f;
            }

            return 90f;
        }
    }
}