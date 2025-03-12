namespace HRM.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HRM.Core.Services;
using Criteria.Log;
using Models.Log;

public interface IActionLogService
{
    Task<IEnumerable<LogObjectDetail>> GetLogDetailByIdAsync(byte objectTypeId, int objectId);
}

public class ActionLogService(IBaseService baseService) : IActionLogService
{
    public async Task<IEnumerable<LogObjectDetail>> GetLogDetailByIdAsync(byte objectTypeId, int objectId)
    {
        var criteria = new GetLogObjectByIdCriteria
        {
            ObjectTypeId = objectTypeId,
            ObjectId = objectId
        };
        var logs = await baseService.GetDataAsync<LogObjectDetail>(criteria);
        GetFieldChanged(logs);

        return logs;
    }

    private void GetFieldChanged(IEnumerable<LogObjectDetail> logs)
    {
        foreach (var log in logs)
        {
            var oldValues = ReadXmlString(log.OldData);
            var newValues = ReadXmlString(log.Data);
            var fieldNames = oldValues.Select(x => x.FieldName).Union(newValues.Select(x => x.FieldName));

            foreach (var fieldName in fieldNames)
            {
                var oldValue = oldValues.FirstOrDefault(x => x.FieldName == fieldName)?.Value;
                var newValue = newValues.FirstOrDefault(x => x.FieldName == fieldName)?.Value;

                if ((!string.IsNullOrWhiteSpace(oldValue) || !string.IsNullOrWhiteSpace(newValue))
                    && !string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
                {
                    log.Fields.Add(new FieldValue
                    {
                        FieldName = fieldName,
                        OldValue = oldValue,
                        Value = newValue
                    });
                }
            }
        }
    }

    private static IEnumerable<FieldValue> ReadXmlString(string xml)
    {
        var results = new List<FieldValue>();
        TextReader sr = new StringReader(xml);

        using (var reader = XmlReader.Create(sr))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var fields = ReadXmlAttribute(reader);
                    results.AddRange(fields);
                }
            }
        }

        return results;
    }

    private static IEnumerable<FieldValue> ReadXmlAttribute(XmlReader reader)
    {
        var results = new List<FieldValue>();

        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                var field = new FieldValue
                {
                    FieldName = reader.Name,
                    Value = reader.Value
                };

                results.Add(field);
            }

            reader.MoveToElement();
        }

        return results;
    }
}