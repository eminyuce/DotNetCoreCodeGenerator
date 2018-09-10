using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelpersProject;

namespace DotNetCoreCodeGenerator.Controllers
{
    public abstract class BaseController : Controller
    {
        protected  ILoggerFactory LoggerFactory { get; set; }
        public BaseController(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }
    }
}