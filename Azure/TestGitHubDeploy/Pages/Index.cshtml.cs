using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebClientId.Pages
{
    public class IndexModel : PageModel
    {
        readonly MyConfig _myConfig;
        //public 
        private readonly ILogger<IndexModel> _logger;
        public string Id { get { return _myConfig.Id.ToString(); } }
        public string Time { get { return DateTime.UtcNow.ToLongTimeString(); } }

        public IndexModel(
            ILogger<IndexModel> logger,
            MyConfig myConfig)
        {
            _logger = logger;
            _myConfig = myConfig;
        }

        public void OnGet()
        {

        }
    }
}
