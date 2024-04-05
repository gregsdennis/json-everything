namespace FunctionalJsonSchema;

public class EvaluationOptions
{
	public static EvaluationOptions Default { get; } = new();

	public SchemaRegistry SchemaRegistry { get; } = new(MetaSchemas.Registry);
}