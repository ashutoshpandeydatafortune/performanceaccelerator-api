namespace DF_EvolutionAPI.Utils
{
    public class Constant
    {
        public static string CONNECTION_STRING = "";

        public static string AZURE_DOMAIN = "";
        public static string AZURE_INSTANCE = "";
        public static string AZURE_TENANT_ID = "";
        public static string AZURE_CLIENT_ID = "";
        public static string AZURE_CALLBACK_PATH = "";
        public static string AZURE_STORAGE_CONNECTION_STRING = "";

        public static string EMAIL_FROM = "";
        public static string SUBJECT_KRA_UPDATED = "KRA Updated";
        public static string SUBJECT_KRA_CREATED = "New KRA Assigned";

        public static string KRA_UPDATE_ADMIN_APPROVED = "Admin approved KRA";
        public static string KRA_UPDATE_ADMIN_REJECTED = "Admin rejected KRA";
        public static string KRA_UPDATE_MANAGER_APPROVED = "Manager approved KRA";
        public static string KRA_UPDATE_MANAGER_REJECTED = "Manager rejected KRA";

        public static int SMTP_PORT = 587;
        public static string SMTP_HOST = "";
        public static string SMTP_USERNAME = "";
        public static string SMTP_PASSWORD = "";

        public static string KRA_FOOTER_TEMPLATE_NAME = "kra-footer.html";
        public static string KRA_HEADER_TEMPLATE_NAME = "kra-header.html";
        public static string KRA_CREATED_TEMPLATE_NAME = "kra-created.html";
    }
}
