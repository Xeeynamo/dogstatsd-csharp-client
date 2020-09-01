using System.Collections.Generic;
using System.Globalization;

namespace StatsdClient
{
    internal class MetricSerializer
    {
        private static readonly Dictionary<MetricType, string> _commandToUnit = new Dictionary<MetricType, string>
                                                                {
                                                                    { MetricType.Count, "c" },
                                                                    { MetricType.Timing, "ms" },
                                                                    { MetricType.Gauge, "g" },
                                                                    { MetricType.Histogram, "h" },
                                                                    { MetricType.Distribution, "d" },
                                                                    { MetricType.Meter, "m" },
                                                                    { MetricType.Set, "s" },
                                                                };

        private readonly SerializerHelper _serializerHelper;
        private readonly string _prefix;

        internal MetricSerializer(SerializerHelper serializerHelper, string prefix)
        {
            _serializerHelper = serializerHelper;
            _prefix = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + ".";
        }

        public SerializedMetric Serialize<T>(
            MetricType metricType,
            string name,
            T value,
            double sampleRate = 1.0,
            string[] tags = null)
        {
            var serializedMetric = _serializerHelper.GetOptionalSerializedMetric();
            if (serializedMetric == null)
            {
                return null;
            }

            var builder = serializedMetric.Builder;
            var unit = _commandToUnit[metricType];

            builder.Append(_prefix);
            builder.Append(name);
            builder.Append(':');
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}", value);
            builder.Append('|');
            builder.Append(unit);

            if (sampleRate != 1.0)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "|@{0}", sampleRate);
            }

            _serializerHelper.AppendTags(builder, tags);
            return serializedMetric;
        }
    }
}
