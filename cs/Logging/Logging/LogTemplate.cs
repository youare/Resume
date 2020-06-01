using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Logging
{
    public static class LogTemplate
    {
        public const string ResumeApi_PerformanceTracker_Stop_PerformanceTracking = "ResumeApi_PerformanceTracker_Stop/Complete {performance_tracker} in {elapsed_milliseconds} milliseconds";
        public const string ResumeApi_PerformanceTracker_Stop_PerformanceTracking_With_Params = "ResumeApi_PerformanceTracker_Stop/Complete {performance_tracker} in {elapsed_milliseconds} milliseconds with {action_params}";
    }
}
