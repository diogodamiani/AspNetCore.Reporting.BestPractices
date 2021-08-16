using System;
using System.Collections.Generic;
using AspNetCore.Reporting.Common.Services;
using DevExpress.Compatibility.System.Web;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parameter = DevExpress.DataAccess.ObjectBinding.Parameter;

namespace AspNetCore.Reporting.Common.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReportDesignerSetupController : ControllerBase {
        [HttpPost("[action]")]
        public object GetReportDesignerModel([FromForm] string reportUrl)
        {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            //Fill a data source set if needed

            var expandoODS = new ObjectDataSource()
            {
                DataSourceType = typeof(ExpandoObjectLoader),
                DataMember = "GetData",
                Constructor = new ObjectConstructorInfo(
                    new Parameter("id", typeof(Guid), Guid.NewGuid())
                )
            };

            var compileTimeTypeODS = new ObjectDataSource()
            {
                DataSourceType = typeof(CompileTimeTypeLoader),
                DataMember = "GetData",
                Constructor = new ObjectConstructorInfo(
                    new Parameter("id", typeof(Guid), Guid.NewGuid())
                )
            };

            dataSources.Add("ExpandoObjectLoader", expandoODS);
            dataSources.Add("CompileTimeTypeLoader", compileTimeTypeODS);
            
            
            var modelGenerator = new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices);
            var model = modelGenerator.GetModel(reportUrl, dataSources, "/DXXRDAngular", "/DXXRDVAngular",
                "/DXXQBAngular");
            model.ReportPreviewSettings.ExportSettings.UseAsynchronousExport = true;
            model.ReportPreviewSettings.ExportSettings.UseSameTab = true;
            string modelJsonScript = modelGenerator.GetJsonModelScript(model);
            return new JavaScriptSerializer().Deserialize<object>(modelJsonScript);
        }
    }
}
