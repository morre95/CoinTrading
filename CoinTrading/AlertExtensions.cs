using Microsoft.AspNetCore.Http;

namespace CoinTrading
{
    public static class AlertExtensions
    {
        public static void SetAlert(this ISession session, string message, string type = "primary")
        {
            session.SetString("bootstrapAlert", 
                /*"<style>" +
                ".fade-out { animation: fade-out 0.7s ease-out forwards; }" +
                "@keyframes fade-out { 0% { opacity: 1; display: block; } 100% { opacity: 0; display: none; } }" +
                "</style>" +*/
                $"<div class=\"alert alert-{type} justify-content-between\" role=\"alert\">" +
                $"{message}" +
                "<button type=\"button\" class=\"close btn-close float-end\" aria-label=\"Close\">" +
                "</button>" +
                "</div>" +
                "<script>" +
                "document.querySelector('.close').addEventListener('click', () =>" +
                "{ document.querySelector('.alert').classList.add('fade-out'); });" +
                "</script>");
        }

        public static string GetAlert(this ISession session)
        {
            string ret = session.GetString("bootstrapAlert") ?? "";
            session.SetString("bootstrapAlert", "");
            return ret;
        }

        public static bool IsAlertEnabled(this ISession session) => !string.IsNullOrEmpty(session.GetString("bootstrapAlert"));

        public static void SetPrimaryAlert(this ISession session, string message)
        {
            session.SetAlert(message);
        }

        public static void SetSecondaryAlert(this ISession session, string message)
        {
            session.SetAlert(message, "secondary");
        }

        public static void SetSuccessAlert(this ISession session, string message)
        {
            session.SetAlert(message, "success");
        }

        public static void SetDangerAlert(this ISession session, string message)
        {
            session.SetAlert(message, "danger");
        }

        public static void SetWarningAlert(this ISession session, string message)
        {
            session.SetAlert(message, "warning");
        }

        public static void SetInfoAlert(this ISession session, string message)
        {
            session.SetAlert(message, "info");
        }

        public static void SetLightAlert(this ISession session, string message)
        {
            session.SetAlert(message, "light");
        }

        public static void SetDarkAlert(this ISession session, string message)
        {
            session.SetAlert(message, "dark");
        }
    }
}
