using AccountManagementService.CrossCutting.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagementService.CrossCutting
{
    public abstract class AppControllerBase : ControllerBase
    {
        protected void SetRecordLog(string recIdentifier, object? recValue)
            => RecordLog.Set(recIdentifier, recValue);
    }
}
