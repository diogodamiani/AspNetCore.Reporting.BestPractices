using System.ComponentModel;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;

namespace AspNetCore.Reporting.Common.Services
{
    public class ResultTypedListProvider : IResultTypedListProvider
    {
        public ResultTypedList GetResult(object dataSource, string dataMember, ParameterList dataMemberParameters, bool ctorIsNull,
            ParameterList ctorParameters)
        {
            ObjectDataSourceFillHelper.FindPropertiesHelper propertiesHelper = 
                new ObjectDataSourceFillHelper.FindPropertiesHelper(
                    dataSource, 
                    dataMember + "Schema", 
                    dataMemberParameters,
                    ctorIsNull, 
                    ctorParameters);
            PropertyDescriptorCollection pdc = propertiesHelper.PDC;

            return ObjectDataSourceFillHelper.CreateTypedList(
                typeof(ExpandoObjectLoader),
                "ExpandoObjectLoaderSchema",
                pdc);
        }
    }
}