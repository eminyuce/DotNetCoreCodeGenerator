using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelpersProject;
using DotNetCoreCodeGenerator.Domain;

namespace DotNetCoreCodeGenerator.Controllers
{
    public abstract class BaseController : Controller
    {
        protected  ILoggerFactory LoggerFactory { get; set; }
        protected MyAppSetttings MyAppSetttings { get; set; }
        public BaseController(ILoggerFactory loggerFactory, MyAppSetttings myAppSetttings)
        {
            LoggerFactory = loggerFactory;
            MyAppSetttings = myAppSetttings;
        }
    }
}