using SHSM_ServerApp.SHSMDataModel;

namespace SHSM_ServerApp
{
    public static class RegisteredUsersHelper
    {
        public static Dictionary<string, RegisteredUserModel> users = new();
        public static Dictionary<string, int> usersindices = new();
    }
}
