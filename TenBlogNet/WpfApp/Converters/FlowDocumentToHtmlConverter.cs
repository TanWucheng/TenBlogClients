using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Xsl;

namespace TenBlogNet.WpfApp.Converters
{
    public class FlowDocumentToHtmlConverter : IValueConverter
    {
        private static XslCompiledTransform _toHtmlTransform;
        private static XslCompiledTransform _toXamlTransform;

        public FlowDocumentToHtmlConverter()
        {
            _toHtmlTransform ??= LoadTransformResource("FlowDocumentToXhtml.xslt");
            _toXamlTransform ??= LoadTransformResource("XhtmlToFlowDocument.xslt");
        }

        private static XslCompiledTransform LoadTransformResource(string fileName)
        {
            //var uri = new Uri(path, UriKind.Relative);
            //var stream = Application.GetResourceStream(uri)?.Stream!;
            var path = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/Resources/Xslt/{fileName}";
            var stream = new FileStream(path, FileMode.Open);
            var xr = XmlReader.Create(stream);
            var xslt = new XslCompiledTransform();
            xslt.Load(xr);
            return xslt;

        }

        #region IValueConverter Members

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not FlowDocument flowDocument) return null;
            if (targetType == typeof(FlowDocument)) return flowDocument;

            if (targetType != typeof(string))
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert back from a FlowDocument to a string.");

            using var ms = new MemoryStream();
            // write XAML out to a MemoryStream
            var tr = new TextRange(
                flowDocument.ContentStart,
                flowDocument.ContentEnd);
            tr.Save(ms, DataFormats.Xaml);
            ms.Seek(0, SeekOrigin.Begin);

            // transform the contents of the MemoryStream to HTML
            var stringBuilder = new StringBuilder();
            using var sw = new StringWriter(stringBuilder);
            var xws = new XmlWriterSettings { OmitXmlDeclaration = true };
            var xr = XmlReader.Create(ms);
            var xw = XmlWriter.Create(sw, xws);
            _toHtmlTransform.Transform(xr, xw);

            return stringBuilder.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return new FlowDocument();
                case FlowDocument:
                    return value;
            }

            if (targetType != typeof(FlowDocument))
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert to a FlowDocument.");
            if (value is not string s)
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert from a string or FlowDocument.");

            FlowDocument d;

            using (var ms = new MemoryStream())
            using (var sr = new StringReader(s))
            {
                var xws = new XmlWriterSettings { OmitXmlDeclaration = true };
                using (var xr = XmlReader.Create(sr))
                using (var xw = XmlWriter.Create(ms, xws))
                {
                    _toXamlTransform.Transform(xr, xw);
                }

                ms.Seek(0, SeekOrigin.Begin);

                d = XamlReader.Load(ms) as FlowDocument;
            }

            XamlWriter.Save(d!, Console.Out);
            return d;
        }

        #endregion
    }
}