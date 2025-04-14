using System.Collections.Generic;

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
        public static string SUBJECT_KRA_UPDATED_MANAGER = "Evaluation Completed";
        public static string SUBJECT_KRA_UPDATED_SRMANAGER = "Final Approval for Evaluation ";
        public static string SUBJECT_KRA_CREATED = "Self-Evaluation Released";

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
        public static string KRA_HEADER_REJECT_TEMPLATE_NAME = "kra-header-reject.html";
        public static string KRA_HEADER_APPROVED_TEMPLATE_NAME = "kra-header-approved.html";
        public static string KRA_HEADER_SR_APPROVED_TEMPLATE_NAME = "kra-header-sr-approval.html";

        public static string APPLICATION_NAME = "Performance Accelerator";
        public static string ROLE_NAME = "Developer";        
        public static List<string> NO_MAIL_DESIGNATION = new List<string>();
        public static string EMPLOYEE_PREFIX = "VDF";//Contract to hire employees, employeeid start with VDF
        public static string ERROR_MESSAGE = "{0} {1}";
        public static int LOG_DELETION_DAYS= 10;

        public static int DAYS_TO_LOOK_BACK = -7;
    }
}
