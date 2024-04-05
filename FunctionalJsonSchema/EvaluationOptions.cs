namespace FunctionalJsonSchema;

public class EvaluationOptions
{
	public static EvaluationOptions Default { get; } = new();

	public bool RequireFormatValidation { get; set; }

	public SchemaRegistry SchemaRegistry { get; } = new(MetaSchemas.Registry);
}