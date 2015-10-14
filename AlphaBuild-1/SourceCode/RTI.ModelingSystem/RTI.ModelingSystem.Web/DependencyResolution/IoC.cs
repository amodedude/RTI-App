// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using RTI.ModelingSystem.Core.Interfaces.Repository;
using RTI.ModelingSystem.Core.Interfaces.Services;
using RTI.ModelingSystem.Infrastructure.Implementation.Repository;
using RTI.ModelingSystem.Infrastructure.Implementation.Services;
using StructureMap;
using StructureMap.Graph;

namespace RTI.ModelingSystem.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            /*
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                    });
            //                x.For<IExample>().Use<Example>();
                            x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                            x.For<IUnitOfWork>().Use<UnitOfWork>();
                        });
            return ObjectFactory.Container;
            */
            var container = new Container(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });
                //                x.For<IExample>().Use<Example>();
                x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                x.For(typeof(ICustomerRepository)).Use(typeof(CustomerRepository));
                x.For(typeof(IConductivityRepository)).Use(typeof(ConductivityRepository));
                x.For(typeof(IConductivityService)).Use(typeof(ConductivityService));
                x.For(typeof(IVesselRepository)).Use(typeof(VesselRepository));
                x.For(typeof(ITrainRepository)).Use(typeof(TrainRepository));
                x.For(typeof(IPredictiveModelRepository)).Use(typeof(PredictiveModelRepository));
                x.For(typeof(IPredictiveModelService)).Use(typeof(PredictiveModelService));
                x.For(typeof(IResinProductsRepository)).Use(typeof(ResinProductsRepository));
                x.For(typeof(ICostAnalyzerService)).Use(typeof(CostAnalyzerService));
            });

            return container;
        }
    }
}