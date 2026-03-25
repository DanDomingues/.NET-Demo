namespace Demo.Models.ViewModels
{
    public class FormFieldModel(
        string fieldName,
        string? value,
        string? label = null,
        bool requiresRoleValidation = true,
        bool isReadOnly = false)
    {
        public string FieldName { get; set; } = fieldName;
        public string? Label { get; set; } = label;
        public string? Value { get; set; } = value;
        public bool RequiresRoleValidation { get; set; } = requiresRoleValidation;
        public bool IsReadOnly { get; set; } = isReadOnly;
    }
}
