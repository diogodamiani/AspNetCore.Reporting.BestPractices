using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Reporting.Common.Services {
    public interface IObjectDataSourceInjector {
        void Process(XtraReport report);
    }

    class ObjectDataSourceInjector : IObjectDataSourceInjector {
        IServiceProvider ServiceProvider { get; }

        public ObjectDataSourceInjector(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Process(XtraReport report) {
            var dse = new UniqueDataSourceEnumerator();
            foreach(var dataSource in dse.EnumerateDataSources(report, true)) {
                if(dataSource is ObjectDataSource ods && ods.DataSource is Type dataSourceType) {
                    var ds = ServiceProvider.GetService(dataSourceType);
                    if (ds != null)
                        ods.DataSource = ds;
                    else
                    {
                        
                        //ConstructorInfo constructor = ObjectDataSourceFillHelper.FindConstructor(typeof(ExpandoObjectLoader), ods.Constructor.Parameters);
                        //var instance  = constructor.Invoke(GetValues(constructor.GetParameters(), ods.Constructor.Parameters));

                        ods.DataSourceType = typeof(ExpandoObjectLoader);
                        ods.DataMember = "GetData";
                    }
                }
            }
        }
        
        private static object[] GetValues(
            IEnumerable<ParameterInfo> ctorParameters,
            IEnumerable<Parameter> parameterList)
        {
            return ctorParameters
                .Select(info => 
                    parameterList.First(
                        parameter => string.Equals(parameter.Name, info.Name, StringComparison.Ordinal)).Value)
                .ToArray();
        }
    }
}
