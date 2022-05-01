namespace GLSPM.Domain
{
    public static class ApplicationConses
    {
        public const string AppName = "Glspm";
        public static class Apis
        {
            public const string Base = "/api";
            public static class Accounts
            {
                public static string Controller = Base + "/Accounts";
                public static string Login = Controller + "/Login";
            }
        }
    }
}
