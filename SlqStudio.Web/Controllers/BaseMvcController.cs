using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SlqStudio.Controllers
{
    public abstract class BaseMvcController : Controller
    {
        private readonly ILogger<BaseMvcController> _logger;

        protected BaseMvcController(ILogger<BaseMvcController> logger)
        {
            _logger = logger;
        }

        protected void LogInfo(string message, object? data = null)
        {
            LogInternal(LogLevel.Information, message, data);
        }

        protected void LogWarning(string message, object? data = null)
        {
            LogInternal(LogLevel.Warning, message, data);
        }

        protected void LogError(string message, Exception? ex = null)
        {
            string controller = ControllerContext.ActionDescriptor.ControllerName;
            string action = ControllerContext.ActionDescriptor.ActionName;
            string file = ex != null ? GetExceptionFile(ex) : "N/A";
            int line = ex != null ? GetExceptionLine(ex) : 0;

            _logger.LogError(
                ex,
                "❌ ERROR | {Controller}.{Action} | {Message} | File: {File} | Line: {Line}",
                controller, action, message, file, line
            );
        }

        private void LogInternal(LogLevel level, string message, object? data = null)
        {
            string controller = ControllerContext.ActionDescriptor.ControllerName;
            string action = ControllerContext.ActionDescriptor.ActionName;

            _logger.Log(level,
                "{Level} | {Controller}.{Action} | {Message} | Data: {@Data}",
                level, controller, action, message, data);
        }

        private static string GetExceptionFile(Exception ex)
        {
            var frame = new StackTrace(ex, true).GetFrame(0);
            return frame?.GetFileName() ?? "Неизвестный файл";
        }

        private static int GetExceptionLine(Exception ex)
        {
            var frame = new StackTrace(ex, true).GetFrame(0);
            return frame?.GetFileLineNumber() ?? 0;
        }
    }
}
