using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace az203_core
{
    public class MyTelemetry : ITelemetryInitializer
    {
        private IHttpContextAccessor _context;
        public MyTelemetry(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor ?? throw new ArgumentNullException("httpContextAccessor");
        }
        public void Initialize(ITelemetry telemetry)
        {
            var current = _context.HttpContext;
            if (current != null)
            {
                if (current.Request.Headers.ContainsKey("UserId"))
                {
                    telemetry.Context.User.Id = current.Request.Headers["UserId"].ToString();
                    telemetry.Context.Session.Id = telemetry.Context.User.Id;
                    telemetry.Context.User.AuthenticatedUserId = telemetry.Context.User.Id;
                }
                else
                    telemetry.Context.User.Id = "1";
            }
        }
    }
}
