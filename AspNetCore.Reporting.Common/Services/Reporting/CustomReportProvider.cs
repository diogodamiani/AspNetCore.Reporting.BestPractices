using System.IO;
using System.Threading.Tasks;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

namespace AspNetCore.Reporting.Common.Services.Reporting {
    public class CustomReportProvider : IReportProvider {
        readonly ReportStorageWebExtension reportStorageWebExtension;
        readonly IObjectDataSourceInjector dataSourceInjector;

        public CustomReportProvider(ReportStorageWebExtension reportStorageWebExtension, IObjectDataSourceInjector dataSourceInjector) {
            this.reportStorageWebExtension = reportStorageWebExtension;
            this.dataSourceInjector = dataSourceInjector;
        }
        public XtraReport GetReport(string id, ReportProviderContext context)
        {
            var reportIdentifier = id.Split('@');

            var reportId = reportIdentifier[0];
            var isFromDesigner = reportIdentifier.Length > 1;

            var reportLayoutBytes = reportStorageWebExtension.GetData(reportId);

            if (reportLayoutBytes == null)
                return new XtraReport();
            
            using(var ms = new MemoryStream(reportLayoutBytes)) {
                var report = XtraReport.FromXmlStream(ms);
                
                if (!isFromDesigner)
                    dataSourceInjector.Process(report);
                
                return report;
            }
        }
    }

    public class CustomReportProviderAsync : IReportProviderAsync {
        readonly ReportStorageWebExtension reportStorageWebExtension;
        readonly IObjectDataSourceInjector dataSourceInjector;

        public CustomReportProviderAsync(ReportStorageWebExtension reportStorageWebExtension, IObjectDataSourceInjector dataSourceInjector) {
            this.reportStorageWebExtension = reportStorageWebExtension;
            this.dataSourceInjector = dataSourceInjector;
        }
        public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext context) {
            var reportLayoutBytes = await reportStorageWebExtension.GetDataAsync(id);
            using(var ms = new MemoryStream(reportLayoutBytes)) {
                var report = XtraReport.FromXmlStream(ms);
                dataSourceInjector.Process(report);
                return report;
            }
        }
    }
}
