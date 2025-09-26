namespace StilSepetiApp.Enums
{
    public class EnumHelper
    {
        public static List<string> GetRoles()
        {
            return Enum.GetNames(typeof(Role)).ToList();
        }

    }
}
