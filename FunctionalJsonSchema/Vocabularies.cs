using System.Collections.Generic;
using System;

namespace FunctionalJsonSchema;

public static partial class Vocabularies
{
	private static readonly Dictionary<Uri, Vocabulary> _registry = new();

	static Vocabularies()
	{
		Register(Core201909,
			Applicator201909,
			Validation201909,
			MetaData201909,
			Format201909,
			Content201909,
			Core202012,
			Applicator202012,
			Unevaluated202012,
			Validation202012,
			MetaData202012,
			FormatAnnotation202012,
			FormatAssertion202012,
			Content202012,
			CoreNext,
			ApplicatorNext,
			UnevaluatedNext,
			ValidationNext,
			MetaDataNext,
			FormatAnnotationNext,
			FormatAssertionNext,
			ContentNext);
	}

	public static void Register(params Vocabulary[] vocabularies)
	{
		foreach (var vocabulary in vocabularies)
		{
			_registry[vocabulary.Id] = vocabulary;
		}
		KeywordRegistry.Register(vocabularies);
	}

	internal static Vocabulary Get(Uri vocabId)
	{
		return TryGet(vocabId) ?? throw new ArgumentException($"Required vocabulary '{vocabId}' not recognized");
	}

	internal static Vocabulary? TryGet(Uri vocabId)
	{
		return _registry.GetValueOrDefault(vocabId);
	}
}