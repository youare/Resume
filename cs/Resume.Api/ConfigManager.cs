using Common;
using Common.Extensions;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Resume.Api
{
    public interface IConfigManager
    {
        AppConfig AppConfig { get;  }
    }
    public class ConfigManager: IConfigManager
    {
        public ConfigManager()
        {
            var schema = new XmlSchemaSet();
            schema.Add("http://dd.com/resume_api.xsd", $"{Constants.ConfigFileName}.xsd");
            var rd = XmlReader.Create($"{Constants.ConfigFileName}.xml");
            var doc = XDocument.Load(rd);
            doc.Validate(schema, (s, e) =>
            {
                if (e?.Exception != null) throw e.Exception;
            });
            AppConfig = GetConfig<AppConfig>(doc.Root);
        }

        private T GetConfig<T>(XElement rootElement)
        {
            var configInstance = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == prop.Name);
                if (string.IsNullOrEmpty(element?.Value)) continue;
                if(prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    var value = ConvertValue(prop, configInstance, element?.Value);
                    prop.SetValue(configInstance, value);
                }
                else
                {
                    var isList = prop.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                    if (isList)
                    {
                        var listArgumentType = prop.PropertyType.GetGenericArguments()[0];
                        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(listArgumentType));
                        var listItems = element.Elements().ToList();
                        foreach(var listItem in listItems)
                        {
                            if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                            {
                                var result = ConvertValue(prop, configInstance, element?.Value);
                                list.GetType().GetMethod("Add").Invoke(list, new[] { result });
                            }
                            else
                            {
                                var result = GetType().GetMethod(nameof(GetConfig), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(listArgumentType).Invoke(this, new[] { listItem });
                                list.GetType().GetMethod("Add").Invoke(list, new[] { result });
                            }
                        }
                        prop.SetValue(configInstance, list);
                    }
                    else
                    {
                        var result = GetType().GetMethod(nameof(GetConfig), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType).Invoke(this, new[] { element });
                        prop.SetValue(configInstance, result);
                    }
                }
            }
            return configInstance;
        }
        private object ConvertValue(PropertyInfo property, object source, string value)
        {
            if (property.PropertyType == typeof(TimeSpan)) return TimeSpan.Parse(value);
            else if (property.PropertyType == typeof(int)) return Convert.ToInt32(value);
            else if (property.PropertyType == typeof(decimal)) return Convert.ToDecimal(value);
            else if (property.PropertyType == typeof(DateTime)) return DateTime.ParseExact(value, Constants.DateFormat, CultureInfo.InvariantCulture);
            else return value;
        }
        public AppConfig AppConfig { get; private set; }
    }
    public class AppConfig
    {
        public string ApplicationName { get; private set; }
        public string InstanceId { get; private set; }
        public string ConnectionString { get; private set; }
        public LogConfig LogConfig { get; private set; }
        public UriConfig UriConfig { get; private set; }
        public HealthCheckConfig HealthCheckConfig { get; private set; }
    }
    public class LogConfig 
    {
        public List<LogEntry> LogEntryList { get; set; }
    }
    public class LogEntry
    {
        public string Source { get; private set; }
        public string MinimumLevel { get; private set; }
    }
    public class UriConfig
    {
        public UriEntry Authority { get; private set; }
        public UriEntry Self { get; private set; }
        public UriEntry Guardian { get; private set; }
    }
    public class UriEntry
    {
        public string Scheme { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Address
        {
            get
            {
                var portStr = Port == 0 || Port == 80 || Port == 443 ? "" : $":{Port}";
                if (string.IsNullOrEmpty(Scheme)) return $"{Host}{portStr}";
                return $"{Scheme}://{Host}{Port}";
            }
        }
    }
    public class HealthCheckConfig {
        public int DelayInSeconds { get; private set; }
        public int IntervalInSeconds { get; private set; }
        public int TolenranceInSeconds { get; private set; }
        public HealthCheckEntry Memory { get; private set; }
        public HealthCheckEntry ServerCpu { get; private set; }
        public HealthCheckEntry ServerMemory { get; private set; }
        public HealthCheckEntry ServerDisk { get; private set; }
    }
    public class HealthCheckEntry
    {
        public decimal DegradeThreshold { get; private set; }
        public decimal UnhealthyThreshold { get; private set; }
        public string IgnoreCommaSeperated { get; private set; }
    }
}
