// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="RTI">
// RTI
// </copyright>
// <summary>Startup</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(RTI.ModelingSystem.Web.Startup))]

namespace RTI.ModelingSystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
