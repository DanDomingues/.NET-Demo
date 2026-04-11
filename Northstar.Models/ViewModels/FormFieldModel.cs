using System.Text.RegularExpressions;

namespace Northstar.Models.ViewModels
{
    public class FormFieldModel
    {
        public string FieldName { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public IEnumerable<string>? Options { get; set; }
        public bool RequiresRoleValidation { get; set; }
        public bool IsReadOnly { get; set; }

        public FormFieldModel(
            string fieldName,
            string? value,
            string? label = null,
            IEnumerable<string>? options = null,
            bool requiresRoleValidation = true,
            bool isReadOnly = false)
        {
            FieldName = fieldName;
            Value = value;
            
            if(string.IsNullOrEmpty(label))
            {
                label = fieldName.Split('.').Last();
                label = Regex.Replace(label, @"(?<!^)([A-Z])", " $1");
            }
            
            Label = label;
            Options = options;
            RequiresRoleValidation = requiresRoleValidation;
            IsReadOnly = isReadOnly;
        }
    }
}
